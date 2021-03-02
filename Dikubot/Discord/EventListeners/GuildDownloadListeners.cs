using System.Threading.Tasks;
using Dikubot.DataLayer.Static;
using Dikubot.Permissions;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners
{
    public class GuildDownloadListeners
    {
        public async Task DownloadGuildOnBoot()
        {
            foreach (var guild in DiscordBot.client.Guilds)
            {
                Logger.Debug($"Pulling Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
                PermissionsService permissionsService = new PermissionsService(guild);
                permissionsService.SetDatabase();
                Logger.Debug($"Successfully pulled Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
            }
        }

        public async Task DownloadGuildOnJoin(SocketGuild guild)
        {
            Logger.Debug($"Pulling Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
            PermissionsService permissionsService = new PermissionsService(guild);
            permissionsService.SetDatabase();
            Logger.Debug($"Successfully pulled Discord data into Database for {guild.Name} ({guild.Id.ToString()})");
        }
    }
}