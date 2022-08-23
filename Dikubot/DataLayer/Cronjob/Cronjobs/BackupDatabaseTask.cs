using Dikubot.DataLayer.Static;

namespace Dikubot.DataLayer.Cronjob.Cronjobs
{
    public class BackupDatabaseTask : CronTask
    {
        // 0 0 */1 * *
        /// <summary>
        /// Takes a backup at 00:00 on every day-of-month.
        /// </summary>
        public BackupDatabaseTask() : base(Cronos.CronExpression.Parse("0 0 */1 * *"), Update)
        {
        }

        private static void Update()
        {
            Logger.Debug("Taking a backup of all databases");
            foreach (var database in Database.DatabaseService.GetDatabaseDictionary())
            {
                Database.DatabaseService.Backup(database.Value);
            }
            Logger.Debug("All databases have been backed up");

        }
    }
}