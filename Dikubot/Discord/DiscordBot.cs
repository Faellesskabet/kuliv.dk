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
        
        [Obsolete]
        public static DiscordSocketClient ClientStatic;
        [Obsolete]
        public static CommandHandler CommandHandlerStatic;
        [Obsolete]
        public static CommandService CommandServiceStatic;

        
        public DiscordSocketClient Client;
        public CommandHandler CommandHandler;
        public CommandService CommandService;

        public DiscordBot()
        {
            #pragma warning disable CS4014
            Run();
            #pragma warning restore CS4014
        }
        
        private Task Log(LogMessage msg)
        {
            #if DEBUG
            Console.WriteLine(msg.ToString());
            #endif

            return Task.CompletedTask;
        }

        private async Task Run()
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

            MessageListener messageListener = new MessageListener();
            Client.MessageReceived += messageListener.OnMessageReceived;
            Client.MessageDeleted += messageListener.OnMessageRemoved;
            Client.UserJoined += new GreetingListener().UserJoined;

            CommandService = new CommandService();
            CommandService.Log += Log;
                
                
            await Client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN"));
            await Client.StartAsync();

            CommandHandler = new CommandHandler(CommandService, Client);
            await CommandHandler.init();
            ClientStatic = Client;
            CommandHandlerStatic = CommandHandler;
            CommandServiceStatic = CommandService;
                
            Scheduler scheduler = new Scheduler();

            Client.Ready += () =>
            {
                    
                // minus 1 so it doesn't include itself (also too lazy to care about dupes)
                int users = Client.Guilds.Sum(guild => guild.MemberCount-1);
                Client.SetGameAsync($"{users.ToString()} users", null, ActivityType.Watching);
                    
                //scheduler.ScheduleTask(new UpdateUserRolesTask());
                scheduler.ScheduleTask(new BackupDatabaseTask());
                scheduler.ScheduleTask(new ClearExpiredSessionsTask());
                scheduler.ScheduleTask(new ForceNameChangeTask());
                
                
                return Task.CompletedTask;
            };
        }
        
    }
}
