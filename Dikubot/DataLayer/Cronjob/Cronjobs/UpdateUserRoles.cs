using System;
using Cronos;
using Dikubot.DataLayer.Static;
using Dikubot.Discord;
using Dikubot.Permissions;

namespace Dikubot.DataLayer.Cronjob.Cronjobs
{
    public class UpdateUserRoles : CronTask
    {
        // 0 */2 * * *
        /// <summary>
        /// Updates user roles every other hour
        /// </summary>
        public UpdateUserRoles() : base(Cronos.CronExpression.Parse("0 */2 * * *"), Update)
        {
        }

        private static void Update()
        {
            Logger.Debug("Updating all user roles");
            foreach (var guild in DiscordBot.client.Guilds)
            {
                PermissionsService permissionsService = new PermissionsService(guild);
                foreach (var user in guild.Users)
                {
                    permissionsService.SetDiscordUserRoles(user);
                }
            }

        }
    }
}