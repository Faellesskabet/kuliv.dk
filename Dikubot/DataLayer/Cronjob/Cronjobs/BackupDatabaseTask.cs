using Cronos;
using Dikubot.DataLayer.Static;

namespace Dikubot.DataLayer.Cronjob.Cronjobs
{
    public class BackupDatabaseTask : CronTask
    {

        private readonly Database.Database _database;

        public BackupDatabaseTask(Database.Database database)
        {
            _database = database;
        }

        // 0 0 */1 * *
        /// <summary>
        /// Takes a backup at 00:00 on every day-of-month.
        /// </summary>
        protected override CronExpression CronExpression()
        {
            return Cronos.CronExpression.Parse("0 0 */1 * *");
        }

        public override void RunTask()
        {
            Logger.Debug("Taking a backup of all databases");
            foreach (var database in _database.GetDatabaseDictionary())
            {
                _database.Backup(database.Value);
            }
            Logger.Debug("All databases have been backed up");

        }
    }
}