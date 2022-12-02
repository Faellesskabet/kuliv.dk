using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Dikubot.DataLayer.Database.Global.Calendar;
using Dikubot.DataLayer.Database.Global.Union;
using Dikubot.DataLayer.Database.Guild.Models.Calendar;
using Dikubot.DataLayer.Database.Guild.Models.Calendar.Events;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Global.Event
{
    
    public class EventsMongoService : GlobalMongoService<EventModel>
    {
        public EventsMongoService(Database database) : base(database)
        {
        }

        public override string GetCollectionName()
        {
            return "Events";
        }
        
        public List<EventModel> Get(DateTime currentDate)
        {
            return Get(new DateTime(currentDate.Year,currentDate.Month, 1), 
                new DateTime(currentDate.Year,currentDate.Month, 
                    DateTime.DaysInMonth(currentDate.Year,currentDate.Month),
                    23,59,59));
        }
        
        public List<EventModel> Get(DateTime startTime, DateTime endTime)
        {
            List<EventModel> result = GetAll(model => (model.StartTime.CompareTo(startTime) >= 0 &&
                                                       model.StartTime.CompareTo(endTime) <= 0)
                                                      || (model.EndTime.CompareTo(startTime) >= 0
                                                          && model.EndTime.CompareTo(endTime) <= 0) ||
                                                      (model.StartTime.CompareTo(startTime) <= 0 &&
                                                       model.EndTime.CompareTo(endTime) >= 0)
            );
            result.Sort((x, y) => DateTime.Compare(x.StartTime, y.StartTime));
            
            return result;
        }
        
    }
}