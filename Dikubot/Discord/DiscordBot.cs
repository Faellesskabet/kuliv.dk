using System;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Cronjob;
using Dikubot.DataLayer.Cronjob.Cronjobs;
using Dikubot.Discord.Command;
using Dikubot.Discord.EventListeners;
using Dikubot.Discord.EventListeners.Permissions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Dikubot.Discord
{
    public class DiscordBot
    {
        public static DiscordSocketClient Client;
        public static CommandHandler CommandHandler;
        public static CommandService CommandService;

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
            var config = new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 1000,
                GatewayIntents = GatewayIntents.All,
            };

            Client = new DiscordSocketClient(config);
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
            Client.UserJoined += new GreetingListener().UserJoined;

            CommandService = new CommandService();
            CommandService.Log += Log;
                
                
            await Client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN"));
            await Client.StartAsync();

            CommandHandler = new CommandHandler(CommandService, Client);
            await CommandHandler.init();
                
            Scheduler scheduler = new Scheduler();

            Client.Ready += () =>
            {
                    
                // minus 1 so it doesn't include itself (also too lazy to care about dupes)
                int users = Client.Guilds.Sum(guild => guild.MemberCount-1);
                Client.SetGameAsync($"{users.ToString()} users", null, ActivityType.Watching);
                    
                scheduler.ScheduleTask(new UpdateUserRolesTask());
                scheduler.ScheduleTask(new BackupDatabaseTask());
                scheduler.ScheduleTask(new ClearExpiredSessionsTask());
                return Task.CompletedTask;
            };
            // Keeps the thread running
            await Task.Delay(-1);
        }
        
    }
}
