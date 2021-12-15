using System;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Cronjob;
using Dikubot.DataLayer.Cronjob.Cronjobs;
using Dikubot.DataLayer.Database.Guild.Models.JoinRole;
using Dikubot.Discord.Command;
using Dikubot.Discord.EventListeners;
using Dikubot.Discord.EventListeners.CustomListner;
using Dikubot.Discord.EventListeners.Permissions;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Victoria;

namespace Dikubot.Discord
{
    public class DiscordBot
    {
        public static DiscordSocketClient Client;
        public static CommandHandler CommandHandler;
        public static InteractiveService Interactive;

        public void run()
        {
            Main().GetAwaiter().GetResult();
        }

        private Task Log(LogMessage msg)
        {
            #if DEBUG
            Console.WriteLine(msg.ToString());
            #endif

            return Task.CompletedTask;
        }

        public async Task Main()
        {
            var config = new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 1000,
            };
            using (var services = ConfigureServices())
            {
                Client = services.GetRequiredService<DiscordSocketClient>();
                var permissionListeners = new PermissionListeners();
                var expandableVoiceChatListener = new ExpandableVoiceChatListener();
                var guildDownloadListeners = new GuildDownloadListeners();
                #if DEBUG
                Client.Log += Log;
                #endif
                Client.RoleCreated += permissionListeners.RoleCreated;
                Client.RoleDeleted += permissionListeners.RoleDeleted;
                Client.RoleUpdated += permissionListeners.RoleUpdated;
                Client.ChannelCreated += permissionListeners.ChannelCreated;
                Client.ChannelDestroyed += permissionListeners.ChannelDestroyed;
                Client.ChannelUpdated += permissionListeners.ChannelUpdated;
                Client.UserVoiceStateUpdated += expandableVoiceChatListener.VoiceChannelExpand;
                Client.UserJoined += permissionListeners.UserJoined;
                Client.UserLeft += permissionListeners.UserLeft;
                Client.GuildMemberUpdated += permissionListeners.UserUpdated;
                Client.Ready += guildDownloadListeners.DownloadGuildOnBoot;
                Client.JoinedGuild += guildDownloadListeners.DownloadGuildOnJoin;
                Client.LeftGuild += guildDownloadListeners.DropGuildOnLeave;
                Client.GuildUpdated += guildDownloadListeners.UpdateGuildOnChange;
                Client.MessageReceived += new MessageListener().OnMessageReceived;

                    services.GetRequiredService<CommandService>().Log += Log;
                
                
                await Client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN"));
                await Client.StartAsync();
                
                CommandHandler = services.GetRequiredService<CommandHandler>();
                await CommandHandler.init();
                
                Scheduler scheduler = new Scheduler();

                Client.Ready += () =>
                {
                    if (!services.GetService<LavaNode>().IsConnected) {
                        services.GetService<LavaNode>().ConnectAsync();
                    }
                    
                    // minus 1 so it doesn't include itself
                    int users = Client.Guilds.Sum(guild => guild.MemberCount-1);
                    Client.SetGameAsync($"{users.ToString()} users", null, ActivityType.Watching);
                    
                    scheduler.ScheduleTask(new UpdateUserRoles());
                    scheduler.ScheduleTask(new BackupDatabase());
                    return Task.CompletedTask;
                };
                // Tilføjer CustomListner
                new CustomListener();

                Client.Ready += async () =>
                {
                    Interactive = new InteractiveService(Client);
                    await new JoinChannelCategoryServices(Client.Guilds.First()).OnStart();
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
                .AddSingleton<InteractiveService>()
                .AddLavaNode(x => {
                    x.SelfDeaf = false;
                })
                .BuildServiceProvider();
        }
    }
}
