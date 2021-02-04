using System;
using System.Threading.Tasks;
using Dikubot.Database.Models;
using Dikubot.Discord.Command;
using Dikubot.Discord.EventListeners.Permissions;
using Dikubot.Discord.EventListeners;
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
                var permissionListeners = new PermissionListeners();
                var expandableVoiceChatListener = new ExpandableVoiceChatListener();
                client.Log += Log;
                client.RoleCreated += permissionListeners.RoleCreated;
                client.RoleDeleted += permissionListeners.RoleDeleted;
                client.RoleUpdated += permissionListeners.RoleUpdated;
                client.ChannelCreated += permissionListeners.ChannelCreated;
                client.ChannelDestroyed += permissionListeners.ChannelDestroyed;
                client.ChannelUpdated += permissionListeners.ChannelUpdated;
                client.UserVoiceStateUpdated += expandableVoiceChatListener.VoiceChannelExpand;
                client.UserJoined += permissionListeners.UserJoined;
                client.UserLeft += permissionListeners.UserLeft;
                client.GuildMemberUpdated += permissionListeners.UserUpdated;

                if (main.IS_DEV)
                {
                    client.MessageReceived += MessageReceived;
                }

                services.GetRequiredService<CommandService>().Log += Log;

                await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_TOKEN"));
                await client.StartAsync();
                
                await services.GetRequiredService<CommandHandler>().init();
                
                client.Connected += () =>
                {
                    DIKU = client.GetGuild(Convert.ToUInt64(Environment.GetEnvironmentVariable("DIKU_DISCORD_ID")));
                    return Task.CompletedTask;
                };

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
