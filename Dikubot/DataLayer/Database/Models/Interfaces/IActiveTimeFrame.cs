using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models.Interfaces
{
    public interface IActiveTimeFrame
    {
        /*
        * NOTE: DateTime.MinValue is the default value for a DateTime
        */
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Check if the it has started (or should've started)
        /// </summary>
        /// <returns>Returns whether the current time is past the role's startdate</returns>
        public bool HasStarted()
        {
            return DateTime.Now > StartDate;
        }

        /// <summary>
        /// Check if the it has ended (or should've ended)
        /// </summary>
        /// <returns>Returns whether the current time is past the it's enddate</returns>
        public bool HasEnded()
        {
            /*
             * NOTE: DateTime.MinValue is the default value for a DateTime
             */
            return DateTime.Now > EndDate && EndDate != DateTime.MinValue;
        }
        /// <summary>
        /// Tells us whether the current time is in between the startdate and enddate
        /// </summary>
        /// <returns>Returns true if HasStarted() is true and HasEnded() is false</returns>
        public bool IsActive()
        {
            return HasStarted() && !HasEnded();
        }
    }
}