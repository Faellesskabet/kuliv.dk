using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Permissions;
using Dikubot.DataLayer.Static;
using Dikubot.Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Cronjob.Cronjobs
{
    public class UpdateUserRolesTask : CronTask
    {
        // 0 */2 * * *
        /// <summary>
        /// Updates user roles every other hour
        /// </summary>
        public UpdateUserRolesTask() : base(Cronos.CronExpression.Parse("0 */2 * * *"), Update)
        {
        }

        private static void Update()
        {
            Logger.Debug("Updating all user roles");
            foreach (var guild in DiscordBot.ClientStatic.Guilds)
            {
                PermissionsService permissionsService = new PermissionsService(guild);
                GuildSettingsModel guildSettingsModel = permissionsService.GetGuildSettingsService().Get(guild);
                foreach (SocketGuildUser user in guild.Users)
                {
                    permissionsService.SetDiscordUserRoles(user, guildSettingsModel);
                }
            }
            Logger.Debug("All user roles have been updated");

        }
    }
}