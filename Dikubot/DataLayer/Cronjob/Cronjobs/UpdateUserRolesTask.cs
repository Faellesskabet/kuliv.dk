using Cronos;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Permissions;
using Dikubot.DataLayer.Static;
using Dikubot.Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Cronjob.Cronjobs
{
    public class UpdateUserRolesTask : CronTask
    {
        private readonly IPermissionServiceFactory _permissionServiceFactory;

        public UpdateUserRolesTask(IPermissionServiceFactory permissionServiceFactory)
        {
            _permissionServiceFactory = permissionServiceFactory;
        }
        
        // 0 */2 * * *
        /// <summary>
        /// Updates user roles every other hour
        /// </summary>
        protected override CronExpression CronExpression()
        {
            return Cronos.CronExpression.Parse("0 */2 * * *");
        }

        public override void RunTask()
        {
            Logger.Debug("Updating all user roles");
            foreach (var guild in DiscordBot.ClientStatic.Guilds)
            {
                PermissionsService permissionsService = _permissionServiceFactory.Get(guild);
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