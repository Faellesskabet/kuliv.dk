using System;
using Cronos;
using Dikubot.DataLayer.Database.Global.Session;

namespace Dikubot.DataLayer.Cronjob.Cronjobs;

public class ClearExpiredSessionsTask : CronTask
{
    // 0 0 */1 * *
    /// <summary>
    /// Clears expired sessions at 00:00 on every day-of-month.
    /// </summary>
    public ClearExpiredSessionsTask() : base(Cronos.CronExpression.Parse("0 0 */1 * *"), Clear) { }

    private static void Clear()
    {
        new SessionMongoService().RemoveAll(model => model.IsExpired);
    }
}