using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Database.Guild.Models.Channel.VoiceChannel;
using Dikubot.DataLayer.Permissions;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners
{
    public class ExpandableVoiceChatListener
    {
        
        private readonly IGuildMongoFactory _guildMongoFactory;

        public ExpandableVoiceChatListener(IGuildMongoFactory guildMongoFactory)
        {
            _guildMongoFactory = guildMongoFactory;
        }

        /// <Summary>Will delete all the empty channels.</Summary>
        /// <param name="channel">The channel that will be deleted from.</param>
        /// <return>Task.</return>
        private async Task DeleteAllEmptyExpandMembers(SocketVoiceChannel channel)
        {
            if (channel == null)
                return;

            var guild = channel.Guild;
            var voiceChannelServices = _guildMongoFactory.Get<VoiceChannelMongoService>(guild);
            var permissionsService = _guildMongoFactory.Get<PermissionsService>(guild);
            var channelModel = voiceChannelServices.Get(m => m.DiscordId == channel.Id.ToString());

            if (!channelModel.ExpandOnJoin.GetValueOrDefault())
                return;

            // Get's childrens and parent.
            var children = voiceChannelServices.GetAll(m => m.DeleteOnLeave && m.ExpandId == channelModel.ExpandId);
            var parent = voiceChannelServices.Get(m => !m.DeleteOnLeave && m.ExpandId == channelModel.ExpandId);
            var deleteChildren = new List<SocketVoiceChannel>();
            var socketParent = guild.GetVoiceChannel(Convert.ToUInt64(parent.DiscordId));

            // Gets all the children that should be deleted.
            foreach (var child in children)
            {
                var voiceChannel = guild.GetVoiceChannel(Convert.ToUInt64(child.DiscordId));
                if (voiceChannel?.Users?.Count != null && voiceChannel.Users.Count == 0)
                    deleteChildren.Add(voiceChannel);
            }

            // If the parent is empty it will delete all children else it will delete alle children but one.
            if (socketParent.Users.Count == 0)
                foreach (var child in deleteChildren)
                    await permissionsService.RemoveVoiceChannel(child);
            else
                foreach (var child in deleteChildren.Skip(1))
                    await permissionsService.RemoveVoiceChannel(child);
        }

        /// <Summary>Create a channel if no channel is availbe.</Summary>
        /// <param name="channel">The channel that will be used to create from.</param>
        /// <return>Task.</return>
        private async Task AddChannelIfNonIsAvaiable(SocketVoiceChannel channel)
        {
            if (channel == null)
            {
                return;
            }
            
            // ToDo: får en fejl System.NullReferenceException: Object reference not set to an instance of an object.
            var voiceChannelServices = _guildMongoFactory.Get<VoiceChannelMongoService>(channel.Guild);

            // If channel is null return.
            
            var model = voiceChannelServices.Get(m => m.DiscordId == channel.Id.ToString());

            // If channel isn't expandable return. // || !model.ExpandOnJoin
            if (!model?.ExpandOnJoin ?? false) return;

            // If a single voice channel is empty return.
            var children = voiceChannelServices.GetAll(m => m.ExpandId == model.ExpandId);
            foreach (var child in children)
            {
                var gottenChannel = channel.Guild.GetVoiceChannel(Convert.ToUInt64(child.DiscordId));
                if (gottenChannel?.Users?.Count != null && gottenChannel.Users.Count == 0)
                    return;
            }

            // Makes a new voice chanel model
            var childModel = new VoiceChannelMainModel();
            childModel.ExpandOnJoin = true;
            childModel.DeleteOnLeave = true;
            if (model != null)
            {
                childModel.Position = model.Position;
                childModel.ExpandId = model.ExpandId;
                // todo : lav count rigtig da den lige nu hedder Channel navn 1 2 3... 
                childModel.Name =  $"{model.Name} {children.Count}";
                childModel.DiscordCategoryId = model.DiscordCategoryId;
            }

            // Adds the voice channel to the database and server.
            var permissionsService = _guildMongoFactory.Get<PermissionsService>(channel.Guild);
            await permissionsService.AddVoiceChannel(childModel);
        }

        /// <Summary>Makes a new voice chat when someone joins a expandable voice chat.</Summary>
        /// <param name="user">The user who joined.</param>
        /// <param name="leaveState">The state when the user leaves.</param>
        /// <param name="joinState">The state when the user joins.</param>
        /// <return>Task.</return>
        public async Task VoiceChannelExpand(SocketUser user, SocketVoiceState leaveState, SocketVoiceState joinState)
        {
            var leaveChannel = leaveState.VoiceChannel;
            var joinChannel = joinState.VoiceChannel;
            await DeleteAllEmptyExpandMembers(joinChannel);
            await DeleteAllEmptyExpandMembers(leaveChannel);
            await AddChannelIfNonIsAvaiable(joinChannel);
            await AddChannelIfNonIsAvaiable(leaveChannel);
        }
    }
}