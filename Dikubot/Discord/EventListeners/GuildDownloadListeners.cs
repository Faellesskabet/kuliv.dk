using System.Threading.Tasks;
using Dikubot.DataLayer.Database;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages.Quote;
using Dikubot.DataLayer.Database.Guild.Models.Guild;
using Dikubot.DataLayer.Permissions;
using Dikubot.DataLayer.Static;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners
{
    public class GuildDownloadListeners
    {
        public async Task DownloadGuildOnBoot()
        {
            foreach (SocketGuild guild in DiscordBot.Client.Guilds)
            {
                Logger.Debug($"Pulling Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
                PermissionsService permissionsService = new PermissionsService(guild);
                permissionsService.SetDatabase();
                permissionsService.AddOrUpdateDatabaseGuild(new GuildMainModel(guild));
                permissionsService.UpdateUserDiscordRoles();
                QuoteServices.DownloadAllQuotes(guild);
                Logger.Debug($"Successfully pulled Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
            }
        }

        public async Task DownloadGuildOnJoin(SocketGuild guild)
        {
            Logger.Debug($"Pulling Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
            PermissionsService permissionsService = new PermissionsService(guild);
            permissionsService.SetDatabase();
            permissionsService.AddOrUpdateDatabaseGuild(new GuildMainModel(guild));
            permissionsService.UpdateUserDiscordRoles();
            QuoteServices.DownloadAllQuotes(guild);
            Logger.Debug($"Successfully pulled Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
        }
        
        public async Task DropGuildOnLeave(SocketGuild guild)
        {
            Logger.Debug($"Dropping Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
            await Database.GetInstance.GetDatabase($"{guild.Id.ToString()}_Main").Client
                .DropDatabaseAsync($"{guild.Id.ToString()}_Main");
            Logger.Debug($"Successfully Drop Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
        }
        
        public async Task UpdateGuildOnChange(SocketGuild outGuild, SocketGuild inGuild)
        {
            Logger.Debug($"Update Discord data into Database for {outGuild.Name} ({outGuild.Id.ToString()})");
            var permissionsServices = new PermissionsService(inGuild);
            permissionsServices.AddOrUpdateDatabaseGuild(new GuildMainModel(inGuild));
            Logger.Debug($"Update Discord data into Database for {inGuild.Name} ({inGuild.Id.ToString()})");
        }
        
    }
}