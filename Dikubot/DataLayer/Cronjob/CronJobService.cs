using Dikubot.DataLayer.Cronjob.Cronjobs;

namespace Dikubot.DataLayer.Cronjob;

public class CronJobService
{
    private readonly Scheduler _scheduler;
    private readonly BackupDatabaseTask _backupDatabaseTask;
    private readonly ForceNameChangeTask _forceNameChangeTask;
    private readonly UpdateVerifiedTask  _updateVerifiedTask;
    private readonly CacheNewsMessagesTask _cacheNewsMessagesTask;

    public CronJobService(Scheduler scheduler, 
        BackupDatabaseTask backupDatabaseTask, 
        ForceNameChangeTask forceNameChangeTask, 
        UpdateVerifiedTask updateVerifiedTask, 
        CacheNewsMessagesTask cacheNewsMessagesTask)
    {
        _scheduler = scheduler;
        _backupDatabaseTask = backupDatabaseTask;
        _forceNameChangeTask = forceNameChangeTask;
        _updateVerifiedTask = updateVerifiedTask;
        _cacheNewsMessagesTask = cacheNewsMessagesTask;
    }

    public void Schedule()
    {
        _scheduler.ScheduleTask(_backupDatabaseTask, false);
        _scheduler.ScheduleTask(_forceNameChangeTask, false);
        _scheduler.ScheduleTask(_updateVerifiedTask, true);
        _scheduler.ScheduleTask(_cacheNewsMessagesTask, true);
    }
}