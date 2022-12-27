using Cronos;
using Dikubot.DataLayer.Database.Global.Session;

namespace Dikubot.DataLayer.Cronjob.Cronjobs;

public class ClearExpiredSessionsTask : CronTask
{
    private readonly SessionMongoService _sessionMongoService;

    public ClearExpiredSessionsTask(SessionMongoService sessionMongoService)
    {
        _sessionMongoService = sessionMongoService;
    }


    // 0 0 */1 * *
    /// <summary>
    ///     Clears expired sessions at 00:00 on every day-of-month.
    /// </summary>
    protected override CronExpression CronExpression()
    {
        return Cronos.CronExpression.Parse("0 0 */1 * *");
    }

    public override void RunTask()
    {
        _sessionMongoService.RemoveAll(model => model.IsExpired);
    }
}