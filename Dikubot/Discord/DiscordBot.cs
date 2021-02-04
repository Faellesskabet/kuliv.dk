using System;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.Database.Models;
using Dikubot.Database.Models.VoiceChannel;
using Dikubot.Discord.Command;
using Dikubot.Permissions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Dikubot.Discord
{
	public class DiscordBot
	{
		public static DiscordSocketClient client;
		public static CommandHandler commandHandler;
		public static SocketGuild DIKU;

		public void run()
		{
			Main().GetAwaiter().GetResult();
		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		private Task MessageReceived(SocketMessage message)
		{
			Console.WriteLine($"({message.Timestamp.ToString()}) {message.Author.ToString()} > {message.ToString()}");
			UserModel userModel = new UserServices().Get(message.Author);
			Console.WriteLine($"Context > Discord ID: {userModel.DiscordId} MongoDB ID: {userModel.Id}");
			return Task.CompletedTask;
		}

		public async Task Main()
		{
			using (var services = ConfigureServices())
			{
				var config = new DiscordSocketConfig
				{
					AlwaysDownloadUsers = true,
					MessageCacheSize = 1000
				};
				
				client = services.GetRequiredService<DiscordSocketClient>();
				client.Log += Log;
				client.RoleCreated += RoleCreated;
				client.RoleDeleted += RoleDeleted;
				client.RoleUpdated += RoleUpdated;
				client.ChannelCreated += ChannelCreated;
				client.ChannelDestroyed += ChannelDestroyed;
				client.ChannelUpdated += ChannelUpdated;
				client.UserVoiceStateUpdated += VoiceChannelExpand;

				if (main.IS_DEV)
				{
					client.MessageReceived += MessageReceived;
				}

				services.GetRequiredService<CommandService>().Log += Log;

				await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_TOKEN"));
				await client.StartAsync();

				client.Connected += () =>
				{
					DIKU = client.GetGuild(Convert.ToUInt64(Environment.GetEnvironmentVariable("DIKU_DISCORD_ID")));
					return Task.CompletedTask;
				};

				await services.GetRequiredService<CommandHandler>().init();

				await Task.Delay(-1);
			}

		}

		private ServiceProvider ConfigureServices()
		{
			return new ServiceCollection()
				.AddSingleton<DiscordSocketClient>()
				.AddSingleton<CommandService>()
				.AddSingleton<CommandHandler>()
				.BuildServiceProvider();
		}

		/// <Summary>Checks if the first entry in the audit log is a bot.</Summary>
		/// <param name="guild">The guild that is being checked for.</param>
		/// <return>Task<bool>.</return>
		private async Task<bool> IsBotFirstEntryInAuditLog(SocketGuild guild)
		{
			await foreach (var auditLogEntryCollection in guild.GetAuditLogsAsync(1))
				foreach (var auditLogEntry in auditLogEntryCollection)
					return auditLogEntry.User.IsBot;
			return false;
		}

		/// <Summary>When a role gets created it will be added to the database.</Summary>
		/// <param name="role">The role that has been created.</param>
		/// <return>Task.</return>
		private async Task RoleCreated(SocketRole role)
		{
			if (await IsBotFirstEntryInAuditLog(role.Guild))
				return; 
			
			var permissionsServices = new PermissionsService(role.Guild);
			permissionsServices.AddOrUpdateDatabaseRole(role);
		}
		
		/// <Summary>When a role gets destroyed it will be removed to the database.</Summary>
		/// <param name="role">The role which will be destroyed.</param>
		/// <return>Task.</return>
		private async Task RoleDeleted(SocketRole role)
		{
			if (await IsBotFirstEntryInAuditLog(role.Guild))
				return; 
			
			var permissionsServices = new PermissionsService(role.Guild);
			permissionsServices.RemoveDatabaseRole(role);
		}
		
		/// <Summary>When a role gets edited it will be updated in the database.</Summary>
		/// <param name="roleBefore">The role before it got edited.</param>
		/// <param name="roleAfter">The role after it got edited.</param>
		/// <return>Task.</return>
		private async Task RoleUpdated(SocketRole roleBefore, SocketRole roleAfter)
		{
			if (await IsBotFirstEntryInAuditLog(roleAfter.Guild))
				return; 
			
			var permissionsServices = new PermissionsService(roleAfter.Guild);
			permissionsServices.AddOrUpdateDatabaseRole(roleAfter);
		}
		
		/// <Summary>When a channel gets created it will be added to the database.</Summary>
		/// <param name="channel">The channel that has been created.</param>
		/// <return>Task.</return>
		private async Task ChannelCreated(SocketChannel channel)
		{
			PermissionsService permissionsService;
			if (channel is SocketVoiceChannel) // Determines which type of channel has bee created.
			{
				var voiceChannel = channel as SocketVoiceChannel;
				
				if (await IsBotFirstEntryInAuditLog(voiceChannel.Guild))
					return; 
				
				permissionsService = new PermissionsService(voiceChannel.Guild);
				permissionsService.AddOrUpdateDatabaseVoiceChannel(voiceChannel);
			}
			else if (channel is SocketTextChannel)
			{
				var textChannel = channel as SocketTextChannel;

				if (await IsBotFirstEntryInAuditLog(textChannel.Guild))
					return;

				permissionsService = new PermissionsService(textChannel.Guild);
				permissionsService.AddOrUpdateDatabaseTextChannel(textChannel);
			}
		}
		
		/// <Summary>When a channel gets destroyed it will be removed to the database.</Summary>
		/// <param name="channel">The channel which will be destroyed.</param>
		/// <return>Task.</return>
		private async Task ChannelDestroyed(SocketChannel channel)
		{
			PermissionsService permissionsService;
			if (channel is SocketVoiceChannel)
			{
				var voiceChannel = channel as SocketVoiceChannel;
				
				if (await IsBotFirstEntryInAuditLog(voiceChannel.Guild))
					return; 
				
				permissionsService = new PermissionsService(voiceChannel.Guild);
				permissionsService.RemoveDatabaseVoiceChannel(voiceChannel);
			}
			else if (channel is SocketTextChannel)
			{
				var textChannel = channel as SocketTextChannel;
				
				if (await IsBotFirstEntryInAuditLog(textChannel.Guild))
					return; 
				
				permissionsService = new PermissionsService(textChannel.Guild);
				permissionsService.RemoveDatabaseTextChannel(textChannel);
			}
		}
		
		/// <Summary>When a channel gets edited it will be updated in the database.</Summary>
		/// <param name="channelBefore">The channel before it got edited.</param>
		/// <param name="channelAfter">The channel after it got edited.</param>
		/// <return>Task.</return>
		private async Task ChannelUpdated(SocketChannel channelBefore, SocketChannel channelAfter)
		{
			PermissionsService permissionsService;
			if (channelAfter is SocketVoiceChannel)
			{
				var voiceChannel = channelAfter as SocketVoiceChannel;
				
				if (await IsBotFirstEntryInAuditLog(voiceChannel.Guild))
					return; 
				
				permissionsService = new PermissionsService(voiceChannel.Guild);
				permissionsService.AddOrUpdateDatabaseVoiceChannel(voiceChannel);
			}
			else if (channelAfter is SocketTextChannel)
			{
				var textChannel = channelAfter as SocketTextChannel;
				
				if (await IsBotFirstEntryInAuditLog(textChannel.Guild))
					return; 
				
				permissionsService = new PermissionsService(textChannel.Guild);
				permissionsService.AddOrUpdateDatabaseTextChannel(textChannel);
			}
		}

		/// <Summary>Makes a new voice chat when someone joins a expandable voice chat.</Summary>
		/// <param name="user">The user who joined.</param>
		/// <param name="leaveState">The state when the user leaves.</param>
		/// <param name="joinState">The state when the user joins.</param>
		/// <return>Task.</return>
		private async Task VoiceChannelExpand(SocketUser user, SocketVoiceState leaveState, SocketVoiceState joinState)
		{
			
			var voiceChannelServices = new VoiceChannelServices();
			if (leaveState.VoiceChannel != null)
			{
				var channel = leaveState.VoiceChannel;
				var guild = channel.Guild;
				var model = voiceChannelServices.Get(model => model.DiscordId == channel.Id.ToString());

				var childModel = voiceChannelServices.Get(m => m.DiscordId == model.Child);
				var childSocket = guild.GetVoiceChannel(Convert.ToUInt64(model.Child));
				if (childModel.DeleteOnLeave && childSocket.Users.Count == 0)
				{
					await childSocket.DeleteAsync();
				}
				if (model.DeleteOnLeave && channel.Users.Count == 0)
				{
					await channel.DeleteAsync();
				}
			}

			if (joinState.VoiceChannel != null)
			{
				var channel = joinState.VoiceChannel;
				var guild = channel.Guild;
				var model = voiceChannelServices.Get(model => model.DiscordId == channel.Id.ToString());
				if (model.ExpandOnJoin && channel.Users.Count == 1)
				{
					var restVoiceChannel = await guild.CreateVoiceChannelAsync("temp",
						properties =>
						{
							properties.CategoryId = channel.CategoryId;
						});
					
					var permissionsService = new PermissionsService(guild);
					permissionsService.AddOrUpdateDatabaseVoiceChannel(restVoiceChannel);
					
					var newModel = new VoiceChannelModel();
					newModel.DiscordId = restVoiceChannel.Id.ToString();
					newModel.ExpandOnJoin = true;
					newModel.DeleteOnLeave = true;
					voiceChannelServices.Upsert(newModel);

					model.Child = newModel.DiscordId;
					voiceChannelServices.Upsert(model);
				}
			}
		}
	}
}
