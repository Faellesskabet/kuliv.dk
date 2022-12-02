using Discord.WebSocket;

namespace Dikubot.DataLayer.Permissions;

public interface IPermissionServiceFactory
{
    public PermissionsService Get(SocketGuild guild);
}