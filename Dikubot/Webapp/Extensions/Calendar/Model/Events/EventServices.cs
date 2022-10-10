using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Dikubot.DataLayer.Database.Global;
using Dikubot.DataLayer.Database.Global.Settings.Tags;
using Dikubot.Discord;
using Discord.WebSocket;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;


namespace Dikubot.DataLayer.Database.Guild.Models.Calendar.Events
{
    public class EventsServices : GlobalServices<EventModel>
    {
        private readonly Guid? _calendar;
        private string guildId;

        public EventsServices(SocketGuild guildId, Guid? calendar =null) : base("Events")
        {
            _calendar = calendar;
            this.guildId = guildId?.Id.ToString();

        }
        
        public EventsServices(string guildId = null, Guid? calendar = null) : base("Events")
        {
            _calendar = calendar;
            this.guildId = guildId;
        }
        
        public override List<EventModel> Get()
        {
            if (_calendar != null)
            {
                return base.Get();
            }
            return _models.Find(e => e.Calendars.Contains(_calendar ?? Guid.Empty)).ToList();
        }


        public List<EventModel> Get(DateTime currentDate, UserService userService = null)
        {
            return Get(new DateTime(currentDate.Year,currentDate.Month, 1), 
                new DateTime(currentDate.Year,currentDate.Month, 
                    DateTime.DaysInMonth(currentDate.Year,currentDate.Month),
                    23,59,59), 
                userService);
        }
        
        public List<EventModel> Get(DateTime startTime, DateTime endTime, UserService userService = null)
        {
            HashSet<Guid> roles = new HashSet<Guid>();
            if (userService != null)
            {
                roles = !guildId.IsNullOrEmpty()
                    ? userService.GetUserGlobalModel().GetRolesGuid(Convert.ToUInt64(guildId))
                    : userService.GetUserGlobalModel().GetRolesGuid();
            }

            List<CalendarModel> calendarModels = !guildId.IsNullOrEmpty()
                ? new CalendarServices(this.guildId)
                .Get(CalendarModel.EnumCalendarType.Event, userService.GetUserGlobalModel()) : new List<CalendarModel>();

            HashSet<Guid> ViewCalenders = userService.GetUserGlobalModel()
                .GetAllViewCalenders(CalendarModel.EnumCalendarType.Event).Result
                .SelectMany(m => m.Item2).Select(m => m.Id).ToHashSet();
            
            HashSet<Guid> PermisionsCalenders = userService.GetUserGlobalModel()
                .GetAllPermisionsCalendars(CalendarModel.EnumCalendarType.Event).Result
                .SelectMany(m => m.Item2).Select(m => m.Id).ToHashSet(); 
            
            List<EventModel> result = GetAll(model => (model.StartTime.CompareTo(startTime) >= 0 &&
                                                                model.StartTime.CompareTo(endTime) <= 0)
                                                                || (model.EndTime.CompareTo(startTime) >= 0
                                                                    && model.EndTime.CompareTo(endTime) <= 0) ||
                                                                (model.StartTime.CompareTo(startTime) <= 0 && 
                                                                 model.EndTime.CompareTo(endTime) >= 0)
                ).Where(model => model.Calendars.Count == 0
                                 || PermisionsCalenders.Overlaps(model.Calendars) ||
                                 ViewCalenders.Overlaps(model.Calendars)
                                 ).Select(m =>
                {
                    m.IsReadonly = userService != null && !( m.Hosts.Contains(userService.GetUserGlobalModel().DiscordId) 
                                                           || PermisionsCalenders.Overlaps(m.Calendars));
                    return m;
                })
                .ToList();
            result.Sort((x, y) => DateTime.Compare(x.StartTime, y.StartTime));

            return result;
        }
        
    }
}