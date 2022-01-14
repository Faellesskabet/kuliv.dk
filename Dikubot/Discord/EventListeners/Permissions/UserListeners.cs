using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.DataLayer.Permissions;
using Discord;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners.Permissions
{
    public partial class PermissionListeners
    {
        /// <Summary>When a user gets created it will be added to the database.</Summary>
        /// <param name="user">The user that has been created.</param>
        /// <return>Task.</return>
        public async Task UserJoined(SocketGuildUser user)
        {
            var util = new Util();

            if (await util.IsBotFirstEntryInAuditLog(user.Guild))
                return;

            var permissionsServices = new PermissionsService(user.Guild);
            permissionsServices.AddOrUpdateDatabaseUser(user);
            permissionsServices.SetDiscordUserRoles(user);
        }

        /// <Summary>When a user gets destroyed it will be removed to the database.</Summary>
        /// <param name="user">The user which will be destroyed.</param>
        /// <return>Task.</return>
        public async Task UserLeft(SocketGuild socketGuild, SocketUser socketUser)
        {
            var util = new Util();

            if (await util.IsBotFirstEntryInAuditLog(socketGuild))
                return;

            UserGuildServices userGuildServices = new UserGuildServices(socketGuild);
            userGuildServices.Remove(model => Equals(model.DiscordId, socketUser.Id));
        }

        /// <Summary>When a user gets edited it will be updated in the database.</Summary>
        /// <param name="userBefore">The user before it got edited.</param>
        /// <param name="userAfter">The user after it got edited.</param>
        /// <return>Task.</return>
        public async Task UserUpdated(Cacheable<SocketGuildUser, ulong> cacheable, SocketGuildUser userAfter)
        {
            var util = new Util();

            // Temporary solutions to an issue
            //if (await util.IsBotFirstEntryInAuditLog(userAfter.Guild))
            //    return;

            var permissionsServices = new PermissionsService(userAfter.Guild);
            permissionsServices.AddOrUpdateDatabaseUser(userAfter);
        }
    }
}