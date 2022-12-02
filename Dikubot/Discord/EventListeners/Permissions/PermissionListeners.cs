using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Permissions;

namespace Dikubot.Discord.EventListeners.Permissions;

public partial class PermissionListeners
{
    private readonly IGuildMongoFactory _guildMongoFactory;
    private readonly IPermissionServiceFactory _permissionServiceFactory;
    private readonly UserGlobalMongoService _userGlobalMongoService;

    public PermissionListeners(IGuildMongoFactory guildMongoFactory,
        IPermissionServiceFactory permissionServiceFactory,
        UserGlobalMongoService userGlobalMongoService)
    {
        _guildMongoFactory = guildMongoFactory;
        _permissionServiceFactory = permissionServiceFactory;
        _userGlobalMongoService = userGlobalMongoService;
    }
}