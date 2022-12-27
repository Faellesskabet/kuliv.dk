using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Database.Guild.Models.Channel.VoiceChannel;
using Dikubot.DataLayer.Permissions;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners;

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

        SocketGuild guild = channel.Guild;
        VoiceChannelMongoService voiceChannelServices = _guildMongoFactory.Get<VoiceChannelMongoService>(guild);
        PermissionsService permissionsService = _guildMongoFactory.Get<PermissionsService>(guild);
        VoiceChannelMainModel channelModel = voiceChannelServices.Get(m => m.DiscordId == channel.Id.ToString());

        if (!channelModel.ExpandOnJoin.GetValueOrDefault())
            return;

        // Get's childrens and parent.
        List<VoiceChannelMainModel> children =
            voiceChannelServices.GetAll(m => m.DeleteOnLeave && m.ExpandId == channelModel.ExpandId);
        VoiceChannelMainModel parent = voiceChannelServices.Get(m => !m.DeleteOnLeave && m.ExpandId == channelModel.ExpandId);
        List<SocketVoiceChannel> deleteChildren = new();
        SocketVoiceChannel socketParent = guild.GetVoiceChannel(Convert.ToUInt64(parent.DiscordId));

        // Gets all the children that should be deleted.
        foreach (VoiceChannelMainModel child in children)
        {
            SocketVoiceChannel voiceChannel = guild.GetVoiceChannel(Convert.ToUInt64(child.DiscordId));
            if (voiceChannel?.Users?.Count != null && voiceChannel.Users.Count == 0)
                deleteChildren.Add(voiceChannel);
        }

        // If the parent is empty it will delete all children else it will delete alle children but one.
        if (socketParent.Users.Count == 0)
            foreach (SocketVoiceChannel child in deleteChildren)
                await permissionsService.RemoveVoiceChannel(child);
        else
            foreach (SocketVoiceChannel child in deleteChildren.Skip(1))
                await permissionsService.RemoveVoiceChannel(child);
    }

    /// <Summary>Create a channel if no channel is availbe.</Summary>
    /// <param name="channel">The channel that will be used to create from.</param>
    /// <return>Task.</return>
    private async Task AddChannelIfNonIsAvaiable(SocketVoiceChannel channel)
    {
        if (channel == null) return;

        // ToDo: får en fejl System.NullReferenceException: Object reference not set to an instance of an object.
        VoiceChannelMongoService voiceChannelServices = _guildMongoFactory.Get<VoiceChannelMongoService>(channel.Guild);

        // If channel is null return.

        VoiceChannelMainModel model = voiceChannelServices.Get(m => m.DiscordId == channel.Id.ToString());

        // If channel isn't expandable return. // || !model.ExpandOnJoin
        if (!model?.ExpandOnJoin ?? false) return;

        // If a single voice channel is empty return.
        List<VoiceChannelMainModel> children = voiceChannelServices.GetAll(m => m.ExpandId == model.ExpandId);
        foreach (VoiceChannelMainModel child in children)
        {
            SocketVoiceChannel gottenChannel = channel.Guild.GetVoiceChannel(Convert.ToUInt64(child.DiscordId));
            if (gottenChannel?.Users?.Count != null && gottenChannel.Users.Count == 0)
                return;
        }

        // Makes a new voice chanel model
        VoiceChannelMainModel childModel = new VoiceChannelMainModel();
        childModel.ExpandOnJoin = true;
        childModel.DeleteOnLeave = true;
        if (model != null)
        {
            childModel.Position = model.Position;
            childModel.ExpandId = model.ExpandId;
            // todo : lav count rigtig da den lige nu hedder Channel navn 1 2 3... 
            childModel.Name = $"{model.Name} {children.Count}";
            childModel.DiscordCategoryId = model.DiscordCategoryId;
        }

        // Adds the voice channel to the database and server.
        PermissionsService permissionsService = _guildMongoFactory.Get<PermissionsService>(channel.Guild);
        await permissionsService.AddVoiceChannel(childModel);
    }

    /// <Summary>Makes a new voice chat when someone joins a expandable voice chat.</Summary>
    /// <param name="user">The user who joined.</param>
    /// <param name="leaveState">The state when the user leaves.</param>
    /// <param name="joinState">The state when the user joins.</param>
    /// <return>Task.</return>
    public async Task VoiceChannelExpand(SocketUser user, SocketVoiceState leaveState, SocketVoiceState joinState)
    {
        SocketVoiceChannel leaveChannel = leaveState.VoiceChannel;
        SocketVoiceChannel joinChannel = joinState.VoiceChannel;
        await DeleteAllEmptyExpandMembers(joinChannel);
        await DeleteAllEmptyExpandMembers(leaveChannel);
        await AddChannelIfNonIsAvaiable(joinChannel);
        await AddChannelIfNonIsAvaiable(leaveChannel);
    }
}