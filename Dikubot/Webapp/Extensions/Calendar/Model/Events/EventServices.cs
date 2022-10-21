using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Dikubot.DataLayer.Database.Global;
using Dikubot.DataLayer.Database.Global.Settings.Tags;
using Dikubot.Discord;
using Dikubot.Webapp.Extensions.Calendar.Model;
using Dikubot.Webapp.Extensions.Discovery.Links;
using Discord.WebSocket;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;


namespace Dikubot.DataLayer.Database.Guild.Models.Calendar.Events
{
    
    public class EventsServices : GlobalServices<EventModel>
    {
        private readonly Guid? _calendar;
        private string guildId;
        private CalendarLogic _calendarLogic;
        private UserService _user;
        private UnionModel? _Union; 
        
        public EventsServices(UserService userService, SocketGuild guildId, Guid? calendar =null) : base("Events")
        {
            _calendar = calendar;
            this.guildId = guildId?.Id.ToString();

            _calendarLogic = new CalendarLogic(userService.GetUserGlobalModel());
            _user = userService;
        }
        
        public EventsServices(UserService userService, string guildId = null, Guid? calendar = null) : base("Events")
        {
            _calendar = calendar;
            this.guildId = guildId;
            _calendarLogic = new CalendarLogic(userService.GetUserGlobalModel());
            _user = userService;
        }

        public EventsServices(UserService userService, UnionModel union) : base("Events")
        {
            _Union = union;
            _calendarLogic = new CalendarLogic(userService.GetUserGlobalModel());
            _user = userService;
        }
        
        public override List<EventModel> Get()
        {
            if (_calendar != null)
            {
                return base.Get();
            }
            return _models.Find(e => e.Calendars.Contains(_calendar ?? Guid.Empty)).ToList();
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
            
            if (_Union is not null)
            {
                result = result.Where(model => model.HostServers.Contains(_Union.Id)).ToList();
                result.Sort((x, y) => DateTime.Compare(x.StartTime, y.StartTime));
                return result;
            }
            
            
            HashSet<Guid> ViewCalenders = _calendarLogic.GetAllViewCalenders(CalendarModel.EnumCalendarType.Event).Result
                .SelectMany(m => m.Item2).Select(m => m.Id).ToHashSet();
            
            HashSet<Guid> PermisionsCalenders = _calendarLogic
                .GetAllPermisionsCalendars(CalendarModel.EnumCalendarType.Event).Result
                .SelectMany(m => m.Item2).Select(m => m.Id).ToHashSet(); 
            
            result.Where(model => model.Hosts.Contains(_user.GetUserGlobalModel().DiscordId)
                                  || PermisionsCalenders.Overlaps(model.Calendars) ||
                                  ViewCalenders.Overlaps(model.Calendars)
                                 ).Select(m =>
                {
                    m.IsReadonly = !(PermisionsCalenders.Overlaps(m.Calendars) || m.Hosts.Contains(_user.GetUserGlobalModel().DiscordId));
                    return m;
                })
                .ToList();
            
            result.Sort((x, y) => DateTime.Compare(x.StartTime, y.StartTime));

            return result;
        }
        
    }
}