using System.Threading.Tasks;
using Dikubot.DataLayer.Permissions;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners.Permissions;

public partial class PermissionListeners
{
    /// <Summary>When a role gets created it will be added to the database.</Summary>
    /// <param name="role">The role that has been created.</param>
    /// <return>Task.</return>
    public async Task RoleCreated(SocketRole role)
    {
        Util util = new Util();

        if (await util.IsBotFirstEntryInAuditLog(role.Guild))
            return;

        PermissionsService permissionsServices = _permissionServiceFactory.Get(role.Guild);
        permissionsServices.AddOrUpdateDatabaseRole(role);
    }

    /// <Summary>When a role gets destroyed it will be removed to the database.</Summary>
    /// <param name="role">The role which will be destroyed.</param>
    /// <return>Task.</return>
    public async Task RoleDeleted(SocketRole role)
    {
        Util util = new Util();

        if (await util.IsBotFirstEntryInAuditLog(role.Guild))
            return;

        PermissionsService permissionsServices = _permissionServiceFactory.Get(role.Guild);
        permissionsServices.RemoveDatabaseRole(role);
    }

    /// <Summary>When a role gets edited it will be updated in the database.</Summary>
    /// <param name="roleBefore">The role before it got edited.</param>
    /// <param name="roleAfter">The role after it got edited.</param>
    /// <return>Task.</return>
    public async Task RoleUpdated(SocketRole roleBefore, SocketRole roleAfter)
    {
        Util util = new Util();

        if (await util.IsBotFirstEntryInAuditLog(roleAfter.Guild))
            return;

        PermissionsService permissionsServices = _permissionServiceFactory.Get(roleAfter.Guild);
        permissionsServices.AddOrUpdateDatabaseRole(roleAfter);
    }
}