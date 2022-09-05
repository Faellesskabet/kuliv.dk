using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.DataLayer.Permissions;
using Dikubot.DataLayer.Static;
using Dikubot.Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Cronjob.Cronjobs;

public class UpdateVerifiedTask : CronTask
{
    // 0 */5 * * *
    /// <summary>
    /// Updates user roles every five minute
    /// </summary>
    public UpdateVerifiedTask() : base(Cronos.CronExpression.Parse("*/5 * * * *"), Update)
    {
    }

    private static void Update()
    {
        Logger.Debug("Updating all verified user roles");
        foreach (var guild in DiscordBot.ClientStatic.Guilds)
        {
            PermissionsService permissionsService = new PermissionsService(guild);
            UserGuildServices userGuildServices = new UserGuildServices(guild);
            GuildSettingsModel guildSettingsModel = permissionsService.GetGuildSettingsService().Get(guild);
            SocketRole verifiedRole = guild.GetRole(guildSettingsModel.VerifiedRole);
            if (verifiedRole == null)
            {
                continue;
            }
            foreach (UserGuildModel user in userGuildServices.GetAll())
            {
                permissionsService.UpdateVerifyRole(user, verifiedRole);
            }
        }
        Logger.Debug("Finished updating all verified user roles");
    }
    
}