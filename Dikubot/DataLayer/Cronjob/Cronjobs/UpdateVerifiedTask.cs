using System;
using Cronos;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.DataLayer.Permissions;
using Dikubot.DataLayer.Static;
using Dikubot.Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Cronjob.Cronjobs;

public class UpdateVerifiedTask : CronTask
{

    private readonly IPermissionServiceFactory _permissionServiceFactory;
    private readonly IGuildMongoFactory _guildMongoFactory;
    private readonly DiscordSocketClient _discordSocketClient;
    
    public UpdateVerifiedTask(IPermissionServiceFactory permissionServiceFactory, IGuildMongoFactory guildMongoFactory, DiscordSocketClient discordSocketClient)
    {
        _permissionServiceFactory = permissionServiceFactory;
        _guildMongoFactory = guildMongoFactory;
        _discordSocketClient = discordSocketClient;
    }

    public override void RunTask()
    {
        Logger.Debug("Updating all verified user roles");
        foreach (var guild in _discordSocketClient.Guilds)
        {
            PermissionsService permissionsService = _permissionServiceFactory.Get(guild);
            UserGuildMongoService userGuildMongoService = _guildMongoFactory.Get<UserGuildMongoService>(guild);
            GuildSettingsModel guildSettingsModel = permissionsService.GetGuildSettingsService().Get(guild);
            SocketRole verifiedRole = guild.GetRole(guildSettingsModel.VerifiedRole);
            if (verifiedRole == null)
            {
                continue;
            }
            foreach (UserGuildModel user in userGuildMongoService.GetAll())
            {
                permissionsService.UpdateVerifyRole(user, verifiedRole);
            }
        }
        Logger.Debug("Finished updating all verified user roles");
    }

    // 0 */5 * * *
    /// <summary>
    /// Updates user roles every five minute
    /// </summary>
    protected override CronExpression CronExpression()
    {
        return Cronos.CronExpression.Parse("*/5 * * * *");
    }
    
}