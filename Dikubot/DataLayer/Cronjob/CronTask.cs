using System;
using Cronos;

namespace Dikubot.DataLayer.Cronjob
{
    public class CronTask
    {
        private CronExpression _cronExpression;
        private Action _action;
        public CronTask(CronExpression cronExpression, Action action)
        {
            _action = action;
            _cronExpression = cronExpression;
        }

        public double GetInterval()
        {
            DateTime? nextOccurrence = _cronExpression.GetNextOccurrence(DateTime.UtcNow);
            if (!nextOccurrence.HasValue)
            {
                return 0;
            }
            return (nextOccurrence.Value - DateTime.UtcNow).TotalMilliseconds;
        }
        public CronExpression CronExpression => _cronExpression;

        public Action Action => _action;
    }
}