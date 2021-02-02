using System;
using System.Threading.Tasks;
using Dikubot.Permissions;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners.Permissions
{
	public partial class PermissionListeners
    {
        /// <Summary>When a channel gets created it will be added to the database.</Summary>
		/// <param name="channel">The channel that has been created.</param>
		/// <return>Task.</return>
		public async Task ChannelCreated(SocketChannel channel)
        {
	        var util = new Util();
			
			PermissionsService permissionsService;
			if (channel is SocketVoiceChannel) // Determines which type of channel has bee created.
			{
				var voiceChannel = channel as SocketVoiceChannel;
				
				if (await util.IsBotFirstEntryInAuditLog(voiceChannel.Guild))
					return; 
				
				permissionsService = new PermissionsService(voiceChannel.Guild);
				permissionsService.AddOrUpdateDatabaseVoiceChannel(voiceChannel);
			}
			else if (channel is SocketTextChannel)
			{
				var textChannel = channel as SocketTextChannel;

				if (await util.IsBotFirstEntryInAuditLog(textChannel.Guild))
					return;

				permissionsService = new PermissionsService(textChannel.Guild);
				permissionsService.AddOrUpdateDatabaseTextChannel(textChannel);
			}
			else if (channel is SocketCategoryChannel)
			{
				var categoryChannel = channel as SocketCategoryChannel;

				if (await util.IsBotFirstEntryInAuditLog(categoryChannel.Guild))
					return;

				permissionsService = new PermissionsService(categoryChannel.Guild);
				permissionsService.AddOrUpdateDatabaseCategoryChannel(categoryChannel);
			}
		}
		
		/// <Summary>When a channel gets destroyed it will be removed to the database.</Summary>
		/// <param name="channel">The channel which will be destroyed.</param>
		/// <return>Task.</return>
		public async Task ChannelDestroyed(SocketChannel channel)
		{
			var util = new Util();
			
			PermissionsService permissionsService;
			if (channel is SocketVoiceChannel)
			{
				var voiceChannel = channel as SocketVoiceChannel;
				
				if (await util.IsBotFirstEntryInAuditLog(voiceChannel.Guild))
					return; 
				
				permissionsService = new PermissionsService(voiceChannel.Guild);
				permissionsService.RemoveDatabaseVoiceChannel(voiceChannel);
			}
			else if (channel is SocketTextChannel)
			{
				var textChannel = channel as SocketTextChannel;
				
				if (await util.IsBotFirstEntryInAuditLog(textChannel.Guild))
					return; 
				
				permissionsService = new PermissionsService(textChannel.Guild);
				permissionsService.RemoveDatabaseTextChannel(textChannel);
			}
			else if (channel is SocketCategoryChannel)
			{
				var categoryChannel = channel as SocketCategoryChannel;

				if (await util.IsBotFirstEntryInAuditLog(categoryChannel.Guild))
					return;

				permissionsService = new PermissionsService(categoryChannel.Guild);
				permissionsService.RemoveDatabaseCategoryChannel(categoryChannel);
			}
		}
		
		/// <Summary>When a channel gets edited it will be updated in the database.</Summary>
		/// <param name="channelBefore">The channel before it got edited.</param>
		/// <param name="channelAfter">The channel after it got edited.</param>
		/// <return>Task.</return>
		public async Task ChannelUpdated(SocketChannel channelBefore, SocketChannel channelAfter)
		{
			var util = new Util();
			
			PermissionsService permissionsService;
			if (channelAfter is SocketVoiceChannel)
			{
				var voiceChannel = channelAfter as SocketVoiceChannel;
				
				if (await util.IsBotFirstEntryInAuditLog(voiceChannel.Guild))
					return; 
				
				permissionsService = new PermissionsService(voiceChannel.Guild);
				permissionsService.AddOrUpdateDatabaseVoiceChannel(voiceChannel);
			}
			else if (channelAfter is SocketTextChannel)
			{
				var textChannel = channelAfter as SocketTextChannel;
				
				if (await util.IsBotFirstEntryInAuditLog(textChannel.Guild))
					return; 
				
				permissionsService = new PermissionsService(textChannel.Guild);
				permissionsService.AddOrUpdateDatabaseTextChannel(textChannel);
			}
			else if (channelAfter is SocketCategoryChannel)
			{
				var categoryChannel = channelAfter as SocketCategoryChannel;
				Console.WriteLine(categoryChannel.Name);
				if (await util.IsBotFirstEntryInAuditLog(categoryChannel.Guild))
					return;

				permissionsService = new PermissionsService(categoryChannel.Guild);
				permissionsService.AddOrUpdateDatabaseCategoryChannel(categoryChannel);
			}
		}
    }
}