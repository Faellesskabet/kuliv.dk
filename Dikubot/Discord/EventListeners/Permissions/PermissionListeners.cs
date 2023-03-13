using Dikubot.DataLayer.Database.Global.User.DiscordUser;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Permissions;

namespace Dikubot.Discord.EventListeners.Permissions;

public partial class PermissionListeners
{
    private readonly IGuildMongoFactory _guildMongoFactory;
    private readonly IPermissionServiceFactory _permissionServiceFactory;
    private readonly DiscordUserGlobalMongoService _discordUserGlobalMongoService;

    public PermissionListeners(IGuildMongoFactory guildMongoFactory,
        IPermissionServiceFactory permissionServiceFactory,
        DiscordUserGlobalMongoService discordUserGlobalMongoService)
    {
        _guildMongoFactory = guildMongoFactory;
        _permissionServiceFactory = permissionServiceFactory;
        _discordUserGlobalMongoService = discordUserGlobalMongoService;
    }
}