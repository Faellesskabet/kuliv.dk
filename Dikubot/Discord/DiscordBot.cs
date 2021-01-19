using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Dikubot.Discord
{
    public class DiscordBot
    {

		public void run()
		{
			MainAsync().GetAwaiter().GetResult();
		}

		public static DiscordSocketClient client;

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		private async Task MessageReceived(SocketMessage message)
		{

			Console.WriteLine($"({message.Timestamp.ToString()}) {message.Author.ToString()} > {message.ToString()}");
		}

		public async Task MainAsync()
		{

			var config = new DiscordSocketConfig
			{
				AlwaysDownloadUsers = true,
				MessageCacheSize = 1000
			};


			client = new DiscordSocketClient(config);
			string token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

			client.Log += Log;
			client.MessageReceived += MessageReceived;

			await client.LoginAsync(TokenType.Bot, token);
			await client.StartAsync();

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}
	}
}
