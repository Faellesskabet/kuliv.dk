using System;
using Cronos;

namespace Dikubot.DataLayer.Cronjob
{
    public abstract class CronTask
    {
        public double GetInterval()
        {
            DateTime? nextOccurrence = this.CronExpression().GetNextOccurrence(DateTime.UtcNow);
            if (!nextOccurrence.HasValue)
            {
                return 0;
            }
            return (nextOccurrence.Value - DateTime.UtcNow).TotalMilliseconds;
        }

        protected abstract CronExpression CronExpression();

        public abstract void RunTask();
    }
}