using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dikubot.Database.Models.VoiceChannel;
using Dikubot.Permissions;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners
{
    public class ExpandableVoiceChatListener
    {
	    /// <Summary>Will delete all the empty channels.</Summary>
	    /// <param name="channel">The channel that will be deleted from.</param>
	    /// <return>Task.</return>
	    private async Task DeleteAllEmptyExpandMembers(SocketVoiceChannel channel)
	    {
		    if (channel == null)
			    return;

		    var guild = channel.Guild;
		    var voiceChannelServices = new VoiceChannelServices();
		    var permissionsService = new PermissionsService(guild);
		    var channelModel = voiceChannelServices.Get(m => m.DiscordId == channel.Id.ToString());

		    if (!channelModel.ExpandOnJoin)
			    return;

		    var children = voiceChannelServices.GetAll(m => m.DeleteOnLeave && m.ExpandId == channelModel.ExpandId);

		    var childrenArray = children.ToArray();
		    for (int i = 1; i < childrenArray.Length; i++)
		    {
			    var voiceChannel = guild.GetVoiceChannel(Convert.ToUInt64(childrenArray[i].DiscordId));
			    if (voiceChannel?.Users?.Count != null && voiceChannel.Users.Count == 0)
				    await permissionsService.RemoveVoiceChannel(voiceChannel);
		    }
	    }
    
	    /// <Summary>Create a channel if no channel is availbe.</Summary>
	    /// <param name="channel">The channel that will be used to create from.</param>
	    /// <return>Task.</return>
	    private async Task AddChannelIfNonIsAvaiable(SocketVoiceChannel channel)
	    {
		    var voiceChannelServices = new VoiceChannelServices();
		    
		    // If channel is null return.
		    if (channel == null)
			    return;
		    var model = voiceChannelServices.Get(m => m.DiscordId == channel.Id.ToString());

		    // If channel isn't expandable return.
		    if (model?.ExpandOnJoin == null || !model.ExpandOnJoin)
			    return;
		    
		    // If a single voice channel is empty return.
		    var children = voiceChannelServices.GetAll(m => m.ExpandId == model.ExpandId);
		    foreach (var child in children)
		    {
			    var gottenChannel = channel.Guild.GetVoiceChannel(Convert.ToUInt64(child.DiscordId));
			    if (gottenChannel?.Users?.Count != null && gottenChannel.Users.Count == 0)
				    return;
		    }
		    // Makes a new voice chanel model
		    var childModel = new VoiceChannelModel();
		    childModel.ExpandOnJoin = true;
		    childModel.DeleteOnLeave = true;
		    childModel.Position = model.Position;
		    childModel.ExpandId = model.ExpandId;
		    childModel.Name = "Rum";
		    childModel.DiscordCategoryId = model.DiscordCategoryId;

		    // Adds the voice channel to the database and server.
		    var permissionsService = new PermissionsService(channel.Guild);
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
		    await AddChannelIfNonIsAvaiable(leaveChannel);
		    await DeleteAllEmptyExpandMembers(leaveChannel);
		    await AddChannelIfNonIsAvaiable(joinChannel);
		    
	    }
    }
}