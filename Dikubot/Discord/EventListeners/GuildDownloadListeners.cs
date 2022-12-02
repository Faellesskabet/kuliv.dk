using System.Threading.Tasks;
using Dikubot.DataLayer.Database;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages.News;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages.Quote;
using Dikubot.DataLayer.Database.Guild.Models.Guild;
using Dikubot.DataLayer.Permissions;
using Dikubot.DataLayer.Static;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners
{
    public class GuildDownloadListeners
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IGuildMongoFactory _guildMongoFactory;
        private readonly IPermissionServiceFactory _permissionServiceFactory;
        private readonly Database _database;

        public GuildDownloadListeners(DiscordSocketClient discordSocketClient, IGuildMongoFactory guildMongoFactory, IPermissionServiceFactory permissionServiceFactory, Database database)
        {
            _discordSocketClient = discordSocketClient;
            _guildMongoFactory = guildMongoFactory;
            _permissionServiceFactory = permissionServiceFactory;
            _database = database;
        }

        public async Task DownloadGuildOnBoot(DiscordBot discordBot)
        {
            foreach (SocketGuild guild in _discordSocketClient.Guilds)
            {
                Logger.Debug($"Pulling Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
                PermissionsService permissionsService = _permissionServiceFactory.Get(guild);
                permissionsService.SetDatabase();
                permissionsService.AddOrUpdateDatabaseGuild(new GuildMainModel(guild));
                permissionsService.UpdateUserDiscordRoles();
                _guildMongoFactory.Get<NewsMongoServices>(guild).DownloadAllMessages();
                _guildMongoFactory.Get<QuoteMongoServices>(guild).DownloadAllMessages();
                Logger.Debug($"Successfully pulled Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
            }
        }

        public async Task DownloadGuildOnJoin(SocketGuild guild)
        {
            Logger.Debug($"Pulling Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
            PermissionsService permissionsService = _permissionServiceFactory.Get(guild);
            permissionsService.SetDatabase();
            permissionsService.AddOrUpdateDatabaseGuild(new GuildMainModel(guild));
            permissionsService.UpdateUserDiscordRoles();
            _guildMongoFactory.Get<NewsMongoServices>(guild).DownloadAllMessages();
            _guildMongoFactory.Get<QuoteMongoServices>(guild).DownloadAllMessages();
            Logger.Debug($"Successfully pulled Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
        }
        
        public async Task DropGuildOnLeave(SocketGuild guild)
        {
            Logger.Debug($"Dropping Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
            await _database.GetDatabase($"{guild.Id.ToString()}_Main").Client
                .DropDatabaseAsync($"{guild.Id.ToString()}_Main");
            Logger.Debug($"Successfully Drop Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
        }
        
        public async Task UpdateGuildOnChange(SocketGuild outGuild, SocketGuild inGuild)
        {
            Logger.Debug($"Update Discord data into Database for {outGuild.Name} ({outGuild.Id.ToString()})");
            var permissionsServices = _permissionServiceFactory.Get(inGuild);
            permissionsServices.AddOrUpdateDatabaseGuild(new GuildMainModel(inGuild));
            Logger.Debug($"Update Discord data into Database for {inGuild.Name} ({inGuild.Id.ToString()})");
        }
        
    }
}