using System;
using System.Threading.Tasks;
using Dikubot.Database.Models;
using Dikubot.Discord.Command;
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
				var client = services.GetRequiredService<DiscordSocketClient>();

				client.Log += Log;

				if (main.IS_DEV)
				{
					client.MessageReceived += MessageReceived;
				}

				services.GetRequiredService<CommandService>().Log += Log;

				await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_TOKEN"));
				await client.StartAsync();

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
	}
}
