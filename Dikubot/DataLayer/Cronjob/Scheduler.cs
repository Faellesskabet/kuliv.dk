using System;
using System.Collections.Generic;
using Cronos;
using Timer = System.Timers.Timer;

namespace Dikubot.DataLayer.Cronjob
{
    public class Scheduler
    {
        private Dictionary<CronTask, Timer> _tasks = new Dictionary<CronTask, Timer>();


        /// <summary>
        /// Creates a CronTask and calls ScheduleTask with the CronTask parameter
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <param name="action"></param>
        public void ScheduleTask(CronExpression cronExpression, Action action)
        {
            ScheduleTask(new CronTask(cronExpression, action));
        }
        
        /// <summary>
        /// Adds a CronTask to the Scheduler
        /// </summary>
        /// <param name="task">The CronTask that will be added</param>
        public void ScheduleTask(CronTask task)
        {
            Timer timer = new Timer();
            timer.Elapsed += (sender, args) =>
            {
                task.Action.Invoke();
                timer.Stop();
                timer.Interval = task.GetInterval();
                timer.Start();
            };
            timer.Interval = task.GetInterval();
            _tasks[task] = timer;
            timer.Start();
        }

        /// <summary>
        /// Removes a CronTask before its next cycle
        /// </summary>
        /// <param name="task">The CronTask that will get removed</param>
        /// <returns>Boolean indicating whether or not the item was removed</returns>
        public bool RemoveTask(CronTask task)
        {
            if (!_tasks.ContainsKey(task))
            {
                return false;
            }
            
            _tasks[task].Stop();
            _tasks.Remove(task);
            return true;
        }
        
    }
}