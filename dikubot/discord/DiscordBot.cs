using System;
using System.Threading.Tasks;
using dikubot.discord.Command;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace dikubot.discord
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

		private async Task MessageReceived(SocketMessage message)
		{
			Console.WriteLine($"({message.Timestamp.ToString()}) {message.Author.ToString()} > {message.ToString()}");
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

				if (Program.IS_DEV)
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
