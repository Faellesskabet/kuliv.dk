using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.DataLayer.Static;
using Dikubot.Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Cronjob.Cronjobs;

public class ForceNameChangeTask: CronTask
{
    // 0 */2 * * *
    /// <summary>
    /// Updates user names every other hour
    /// </summary>
    public ForceNameChangeTask() : base(Cronos.CronExpression.Parse("0 */2 * * *"), Update) { }

    private static void Update()
    {
        Logger.Debug("Forcing name changes for selected servers");
        GuildSettingsMongoService guildSettingsMongoService = new GuildSettingsMongoService();
        UserGlobalMongoService userGlobalMongoService = new UserGlobalMongoService();
        foreach (var guild in DiscordBot.ClientStatic.Guilds)
        {
            GuildSettingsModel guildSettingsModel = guildSettingsMongoService.Get(guild);
            if (!guildSettingsModel.ForceNameChange)
            {
                continue;
            }
            
            foreach (SocketGuildUser user in guild.Users)
            {
                string name = userGlobalMongoService.Get(user)?.Name ?? "";
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }
                Util.UpdateDiscordName(user, name);
            }
        }
        Logger.Debug("Force name has taken place");
    }
}