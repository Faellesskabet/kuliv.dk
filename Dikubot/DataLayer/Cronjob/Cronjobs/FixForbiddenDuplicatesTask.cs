using System;
using System.Collections.Generic;
using Dikubot.DataLayer.Database.Global.User;
using Microsoft.Graph;

namespace Dikubot.DataLayer.Cronjob.Cronjobs;

/// <summary>
/// The purpose of this task is only to fix some legacy duplication issues.
/// Can be removed after 1 run.
/// </summary>
public class FixForbiddenDuplicatesTask : CronTask
{
    // * * */1 * *
    /// <summary>
    /// Updates every day
    /// </summary>
    public FixForbiddenDuplicatesTask() : base(Cronos.CronExpression.Parse("* * */1 * *"), Update)
    {
    }
    
    private static void Update()
    {
        Console.WriteLine("Removing legacy elements");
        UserGlobalServices userGlobalServices = new UserGlobalServices();
        foreach (var user in userGlobalServices.GetAll())
        {
            List<UserGlobalModel> duplicates = userGlobalServices.GetAll(model => model.DiscordId == user.DiscordId);
            if (duplicates.Count <= 1)
            {
                continue;
            }

            foreach (var duplicate in duplicates)
            {
                if (!duplicate.Verified)
                {
                    Console.WriteLine("REMOVED ONE ELEMENT");
                    userGlobalServices.Remove(duplicate);
                }
            }
        }
        Console.WriteLine("Removed legacy elements");
    }

}