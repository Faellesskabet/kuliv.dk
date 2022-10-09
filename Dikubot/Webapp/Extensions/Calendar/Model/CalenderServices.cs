using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Database.Guild.Models.Calendar.Events;
using Dikubot.Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild.Models.Calendar
{//https://stackoverflow.com/questions/13905359/cannot-provide-arguments-when-creating-an-instance-of-generic-type
    public class CalendarServices : GuildServices<CalendarModel>
    {
        private ulong? guildId = null;
        public CalendarServices(SocketGuild guild) : base("Calendar", guild)
        {
            guildId = guild.Id;
        }
        
        
        public CalendarServices(string guidId) : base("Calendar", DiscordBot.ClientStatic?.Guilds?.First(g => g.Id.ToString().Equals(guidId)))
        {
            guildId = Convert.ToUInt64(guidId);
        }
        
        public List<EventModel> GetEvents(Expression<Func<CalendarModel, bool>> calFilter = null, Expression<Func<EventModel, bool>> eventFilter = null)
        {
            var res = new List<EventModel>();
            var eventsServices = new EventsServices(Guild);
            var cal = this.GetAllAsDictionary(calFilter).SelectMany(c => c.Value);
            
            if (cal.Any())
            {
                foreach (var model in cal)
                {
                    res.AddRange(eventsServices.GetAllAsDictionary(eventFilter).SelectMany(x => x.Value).Where(e => e.Calendars.Contains(model.Id)));
                }
            }
            return res;
        }

        public List<CalendarModel> Get(CalendarModel.EnumCalendarType calendarType, UserGlobalModel user)
        {
            var roles = guildId.HasValue ? user.GetRolesGuid((ulong)guildId) : user.GetRolesGuid();
        
            return GetAll(model => model.CalendarType == calendarType)
                .FindAll(model => model.Visible != CalendarModel.EnumAvailable.Privat 
                                  || model.Permission.Overlaps(roles)
                                  || model.View.Overlaps(roles));
        }


        public new static List<(SocketGuild,List<CalendarModel>)> GetAllViewCalenders(CalendarModel.EnumCalendarType calendarType, UserGlobalModel user)
        {
            var roles = user?.GetRolesGuid() ?? new HashSet<Guid>();

            List<(SocketGuild,List<CalendarModel>)> result = new List<(SocketGuild, List<CalendarModel>)>();
            
            foreach (var socketGuild in DiscordBot.ClientStatic.Guilds)
            {
                var service = new CalendarServices(socketGuild);
            
                List<CalendarModel> value = service.GetAll(model => model.CalendarType == calendarType).Where(model => 
                    model.Visible == CalendarModel.EnumAvailable.Public 
                    || (user.Verified && model.Visible == CalendarModel.EnumAvailable.Externt)
                    || (user.Guilds.Contains(socketGuild) && model.Visible == CalendarModel.EnumAvailable.Internt)
                    || model.Permission.Overlaps(roles)
                    || model.View.Overlaps(roles)).ToList();
                result.Add((socketGuild,value));
            }
            return result;
        }
        
        
        public new static List<(SocketGuild,List<CalendarModel>)> GetAllPermisionsCalendars(CalendarModel.EnumCalendarType calendarType, UserGlobalModel user)
        {
            if (user is null)
            {
                return new List<(SocketGuild, List<CalendarModel>)>();
            }
            
            var roles = user.GetRolesGuid();

            List<(SocketGuild,List<CalendarModel>)> result = new List<(SocketGuild, List<CalendarModel>)>();
            
            
            foreach (var socketGuild in user.Guilds)
            {
                var service = new CalendarServices(socketGuild);
            
                List<CalendarModel> value = service.GetAll(model => model.CalendarType == calendarType).Where(model => model.Permission.Overlaps(roles)).ToList();
                result.Add((socketGuild,value));
                 
                
            }

            return result;

        }
        
    }

   


}