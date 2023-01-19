using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dikubot.DataLayer.Database.Global.Calendar;
using Dikubot.DataLayer.Database.Interfaces;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Global.Event;

public class EventsMongoService : GlobalMongoService<EventModel>, IIndexed<EventModel>
{
    public EventsMongoService(Database database) : base(database)
    {
    }

    public override string GetCollectionName()
    {
        return "Events";
    }

    /// <summary>
    /// Get all events from the guids stored in the CalendarModel. Does not return any null elements.
    /// </summary>
    /// <param name="calendarModel">The HashSet<Guid> Events in CalendarModel is used to retrieve the event guids.</param>
    /// <returns>A list of events in the calendar. Does not contain null values. Returns an empty list if no events are found.</returns>
    public List<EventModel> GetFromCalendar(CalendarModel calendarModel)
    {
        return calendarModel.Events.Select(Get).Where(model => model != null).ToList();
    }

    public List<EventModel> Get(DateTime currentDate)
    {
        return Get(
            new DateTime(currentDate.Year, currentDate.Month, 1),
            new DateTime(currentDate.Year, currentDate.Month,
                DateTime.DaysInMonth(currentDate.Year, currentDate.Month),
                23, 59, 59));
    }

    public List<EventModel> Get(DateTime startTime, DateTime endTime)
    {
        List<EventModel> result = GetAll(BetweenDatesFilter(startTime, endTime));
        result.Sort((x, y) => DateTime.Compare(x.StartTime, y.StartTime));

        return result;
    }

    public List<EventModel> FilterByDate(List<EventModel> eventModels, DateTime startTime, DateTime endTime)
    {
        List<EventModel> events = eventModels.FindAll(BetweenDatesFilter(startTime, endTime).Compile().Invoke);
        events.Sort((x, y) => DateTime.Compare(x.StartTime, y.StartTime));
        return events;
    }

    public Expression<Func<EventModel, bool>> BetweenDatesFilter(DateTime startTime, DateTime endTime)
    {
        return model => (model.StartTime.CompareTo(startTime) >= 0 &&
                         model.StartTime.CompareTo(endTime) <= 0)
                        || (model.EndTime.CompareTo(startTime) >= 0
                            && model.EndTime.CompareTo(endTime) <= 0) ||
                        (model.StartTime.CompareTo(startTime) <= 0 &&
                         model.EndTime.CompareTo(endTime) >= 0);
    }

    public IEnumerable<CreateIndexModel<EventModel>> GetIndexes()
    {
        CreateIndexOptions options = new CreateIndexOptions { Unique = true };
        return new List<CreateIndexModel<EventModel>>
        {
            new(Builders<EventModel>.IndexKeys.Ascending(model => model.StartTime), options)
        };
    }
}