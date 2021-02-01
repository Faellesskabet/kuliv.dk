using System;
using System.Threading.Tasks;
using Dikubot.Database.Models;
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

				if (main.IS_DEV)
				{
					client.MessageReceived += MessageReceived;
				}

				services.GetRequiredService<CommandService>().Log += Log;

				await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_TOKEN"));
				await client.StartAsync();

				DIKU = client.GetGuild(Convert.ToUInt64(Environment.GetEnvironmentVariable("DIKU_DISCORD_ID")));
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
		
		private Task RoleCreated(SocketRole role)
		{
			var permissionsServices = new PermissionsService(role.Guild);
			permissionsServices.AddOrUpdateDatabaseRole(role);
			return Task.CompletedTask;
		}
		
		private Task RoleDeleted(SocketRole role)
		{
			var permissionsServices = new PermissionsService(role.Guild);
			permissionsServices.RemoveDatabaseRole(role);
			return Task.CompletedTask;
		}
		
		private Task RoleUpdated(SocketRole roleBefore, SocketRole roleAfter)
		{
			var permissionsServices = new PermissionsService(roleAfter.Guild);
			permissionsServices.AddOrUpdateDatabaseRole(roleAfter);
			return Task.CompletedTask;
		}
		
		private Task ChannelCreated(SocketChannel channel)
		{
			PermissionsService permissionsService;
			if (channel is SocketVoiceChannel)
			{
				var voiceChannel = channel as SocketVoiceChannel;
				permissionsService = new PermissionsService(voiceChannel.Guild);
				permissionsService.AddOrUpdateDatabaseVoiceChannel(voiceChannel);
			}
			else if (channel is SocketTextChannel)
			{
				var textChannel = channel as SocketTextChannel;
				permissionsService = new PermissionsService(textChannel.Guild);
				permissionsService.AddOrUpdateDatabaseTextChannel(textChannel);
			}
			return Task.CompletedTask;
		}
		
		private Task ChannelDestroyed(SocketChannel channel)
		{
			PermissionsService permissionsService;
			if (channel is SocketVoiceChannel)
			{
				var voiceChannel = channel as SocketVoiceChannel;
				permissionsService = new PermissionsService(voiceChannel.Guild);
				permissionsService.RemoveDatabaseVoiceChannel(voiceChannel);
			}
			else if (channel is SocketTextChannel)
			{
				var textChannel = channel as SocketTextChannel;
				permissionsService = new PermissionsService(textChannel.Guild);
				permissionsService.RemoveDatabaseTextChannel(textChannel);
			}
			return Task.CompletedTask;
		}
		private Task ChannelUpdated(SocketChannel channelBefore, SocketChannel channelAfter)
		{
			PermissionsService permissionsService;
			if (channelAfter is SocketVoiceChannel)
			{
				var voiceChannel = channelAfter as SocketVoiceChannel;
				permissionsService = new PermissionsService(voiceChannel.Guild);
				permissionsService.AddOrUpdateDatabaseVoiceChannel(voiceChannel);
			}
			else if (channelAfter is SocketTextChannel)
			{
				var textChannel = channelAfter as SocketTextChannel;
				permissionsService = new PermissionsService(textChannel.Guild);
				permissionsService.AddOrUpdateDatabaseTextChannel(textChannel);
			}
			return Task.CompletedTask;
		}
	}
}
