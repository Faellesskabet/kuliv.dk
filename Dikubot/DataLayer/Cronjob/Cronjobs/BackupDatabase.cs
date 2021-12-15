using Dikubot.DataLayer.Static;

namespace Dikubot.DataLayer.Cronjob.Cronjobs
{
    public class BackupDatabase : CronTask
    {
        // 0 0 */1 * *
        /// <summary>
        /// Takes a backup at 00:00 on every day-of-month.
        /// </summary>
        public BackupDatabase() : base(Cronos.CronExpression.Parse("0 0 */1 * *"), Update)
        {
        }

        private static void Update()
        {
            Logger.Debug("Taking a backup of all databases");
            foreach (var database in Database.Database.GetDatabaseDictionary())
            {
                Database.Database.Backup(database.Value);
            }
            Logger.Debug("All databases have been backed up");

        }
    }
}