using System;
using Cronos;
using Dikubot.DataLayer.Database.Global.User.DiscordUser;

namespace Dikubot.DataLayer.Cronjob.Cronjobs;

/// <summary>
///     The purpose of this task is only to fix some legacy duplication issues.
///     Can be removed after 1 run.
/// </summary>
public class FixForbiddenDuplicatesTask : CronTask
{
    private readonly DiscordUserGlobalMongoService _discordUserGlobalMongoService;

    public FixForbiddenDuplicatesTask(DiscordUserGlobalMongoService discordUserGlobalMongoService)
    {
        _discordUserGlobalMongoService = discordUserGlobalMongoService;
    }

    // * * */1 * *
    /// <summary>
    ///     Updates every day
    /// </summary>
    protected override CronExpression CronExpression()
    {
        return Cronos.CronExpression.Parse("* * */1 * *");
    }

    public override void RunTask()
    {
        Console.WriteLine("Removing legacy elements");
        foreach (DiscordUserGlobalModel user in _discordUserGlobalMongoService.GetAll())
        {
            System.Collections.Generic.List<DiscordUserGlobalModel> duplicates =
                _discordUserGlobalMongoService.GetAll(model => model.DiscordId == user.DiscordId);
            if (duplicates.Count <= 1) continue;

            foreach (DiscordUserGlobalModel duplicate in duplicates)
                if (!duplicate.Verified)
                {
                    Console.WriteLine("REMOVED ONE ELEMENT");
                    _discordUserGlobalMongoService.Remove(duplicate);
                }
        }

        Console.WriteLine("Removed legacy elements");
    }
}