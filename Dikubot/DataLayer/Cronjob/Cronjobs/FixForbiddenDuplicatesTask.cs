using System;
using System.Collections.Generic;
using Cronos;
using Dikubot.DataLayer.Database.Global.User;
using Microsoft.Graph;

namespace Dikubot.DataLayer.Cronjob.Cronjobs;

/// <summary>
/// The purpose of this task is only to fix some legacy duplication issues.
/// Can be removed after 1 run.
/// </summary>
public class FixForbiddenDuplicatesTask : CronTask
{

    private readonly UserGlobalMongoService _userGlobalMongoService;

    public FixForbiddenDuplicatesTask(UserGlobalMongoService userGlobalMongoService)
    {
        _userGlobalMongoService = userGlobalMongoService;
    }

    // * * */1 * *
    /// <summary>
    /// Updates every day
    /// </summary>
    protected override CronExpression CronExpression()
    {
        return Cronos.CronExpression.Parse("* * */1 * *");
    }

    public override void RunTask()
    {
        Console.WriteLine("Removing legacy elements");
        foreach (var user in _userGlobalMongoService.GetAll())
        {
            List<UserGlobalModel> duplicates = _userGlobalMongoService.GetAll(model => model.DiscordId == user.DiscordId);
            if (duplicates.Count <= 1)
            {
                continue;
            }

            foreach (var duplicate in duplicates)
            {
                if (!duplicate.Verified)
                {
                    Console.WriteLine("REMOVED ONE ELEMENT");
                    _userGlobalMongoService.Remove(duplicate);
                }
            }
        }
        Console.WriteLine("Removed legacy elements");
    }

}