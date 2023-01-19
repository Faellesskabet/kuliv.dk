using Cronos;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.DataLayer.Static;
using Dikubot.Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Cronjob.Cronjobs;

public class ForceNameChangeTask : CronTask
{
    private readonly IGuildMongoFactory _guildMongoFactory;
    private readonly GuildSettingsMongoService _guildSettingsMongoService;

    public ForceNameChangeTask(GuildSettingsMongoService guildSettingsMongoService,
        IGuildMongoFactory guildMongoFactory)
    {
        _guildSettingsMongoService = guildSettingsMongoService;
        _guildMongoFactory = guildMongoFactory;
    }

    public override void RunTask()
    {
        Logger.Debug("Forcing name changes for selected servers");
        foreach (SocketGuild guild in DiscordBot.ClientStatic.Guilds)
        {
            GuildSettingsModel guildSettingsModel = _guildSettingsMongoService.Get(guild);
            if (!guildSettingsModel.ForceNameChange) continue;

            foreach (SocketGuildUser user in guild.Users)
            {
                string name = _guildMongoFactory.Get<UserGuildMongoService>(guild).Get(user)?.Name ?? "";
                if (string.IsNullOrWhiteSpace(name)) continue;
                Util.UpdateDiscordName(user, name);
            }
        }

        Logger.Debug("Force name has taken place");
    }

    // 0 */2 * * *
    /// <summary>
    ///     Updates user names every other hour
    /// </summary>
    protected override CronExpression CronExpression()
    {
        return Cronos.CronExpression.Parse("0 */2 * * *");
    }
}