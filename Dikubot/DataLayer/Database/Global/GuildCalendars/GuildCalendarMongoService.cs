using System.Collections.Generic;
using System.Linq;
using Dikubot.DataLayer.Database.Global.Calendar;
using Dikubot.DataLayer.Database.Global.Event;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Global.GuildCalendars;

/// <summary>
/// Guilds can have a list of calendars. This service is used to keep track of which calendars each guild have.
/// </summary>
public class GuildCalendarMongoService : GlobalMongoService<GuildCalendarModel>
{
    private CalendarMongoService _calendarMongoService;
    private EventsMongoService _eventsMongoService;
    public GuildCalendarMongoService(Database database, 
        CalendarMongoService calendarMongoService, 
        EventsMongoService eventsMongoService) : base(database)
    {
        _calendarMongoService = calendarMongoService;
        _eventsMongoService = eventsMongoService;
    }

    public override string GetCollectionName()
    {
        return "GuildCalendars";
    }
    
    /// <summary>
    /// Returns a CalendarModel for each SocketGuild in guilds.
    /// The list does not contain any null values. If no CalendarModels are found, then an empty list is returned.
    /// </summary>
    /// <param name="guilds">A CalendarModel will be returned for each guild.</param>
    /// <returns>A list of CalendarModels. Will not contain any null values</returns>
    public List<CalendarModel> GetCalendarsForGuilds(List<SocketGuild> guilds)
    {
        List<GuildCalendarModel> guildCalendarModels = new List<GuildCalendarModel>();
        foreach (SocketGuild guild in guilds)
        {
            GuildCalendarModel guildCalendarModel = Get(model => model.GuildId == guild.Id);
            if (guildCalendarModel != null)
            {
                guildCalendarModels.Add(guildCalendarModel);
            }
        }

        List<CalendarModel> calendarModels = new List<CalendarModel>();
        foreach (GuildCalendarModel guildCalendarModel in guildCalendarModels)
        {
            CalendarModel calendarModel = _calendarMongoService.Get(guildCalendarModel.Id);
            if (calendarModel != null)
            {
                calendarModels.Add(calendarModel);
            }
        }

        return calendarModels;
    }

    /// <summary>
    /// A guild can have a collection of calendars. This method takes all the calendars from the provided guilds,
    /// and retrieves all of their events. It then puts it into a HashSet to ensure no duplicate values. There
    /// are no null values. If no events are found, then an empty HashSet is returned.
    /// </summary>
    /// <param name="guilds">The list of guilds which calendar's whill be fetched</param>
    /// <returns>A HashSet of EventModels, with no null values.</returns>
    public HashSet<EventModel> GetEventsForGuilds(List<SocketGuild> guilds)
    {
        List<CalendarModel> calendarModels = GetCalendarsForGuilds(guilds);
        HashSet<EventModel> eventModels = new HashSet<EventModel>();
        foreach (CalendarModel calendarModel in calendarModels)
        {
            _eventsMongoService.GetFromCalendar(calendarModel).ForEach(model => eventModels.Add(model));
        }

        return eventModels;
    }

    public GuildCalendarModel Get(SocketGuild guild)
    {
        return Get(model => model.GuildId == guild.Id);
    }
}