using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Database.Guild.Models.Calendar;
using Dikubot.Discord;
using Discord.WebSocket;


namespace Dikubot.Webapp.Extensions.Calendar.Model
{
    public class CalendarLogic
    {
        private readonly UserGlobalModel _user;

        private readonly List<CalendarServices> _calendarServicesList = new List<CalendarServices>();
        
        public CalendarLogic(UserGlobalModel user)
        {
            this._user = user;

            foreach (var socketGuild in DiscordBot.ClientStatic.Guilds)
            {
                _calendarServicesList.Add(new CalendarServices(socketGuild));
            }
        }


        public CalendarModel Get(Guid CalendarID)
        {
            foreach (var service in _calendarServicesList)
            {
                if (service.Get(CalendarID) != null)
                {
                    return service.Get(CalendarID); 
                }
                
            }
            return new CalendarModel();
        }
        

        async public Task<List<(SocketGuild, List<CalendarModel>)>> GetAllViewCalenders(CalendarModel.EnumCalendarType calendarType)
        {
            var roles = _user.GetRolesGuid() ?? new HashSet<Guid>();
            List<(SocketGuild,List<CalendarModel>)> result = new List<(SocketGuild, List<CalendarModel>)>();

            if (!_user.Verified)
            {
                foreach (var service in _calendarServicesList)
                {
                    List<CalendarModel> value = service.GetAll(model => model.CalendarType == calendarType).Where(
                        model =>
                            model.Visible == CalendarModel.EnumAvailable.Public).ToList();
                    result.Add((service.Guild, value));
                }
            }
            else
            {
                foreach (var service in _calendarServicesList)
                {
                    List<CalendarModel> value = service.GetAll(model => model.CalendarType == calendarType).Where(model => 
                        model.Visible is CalendarModel.EnumAvailable.Public or CalendarModel.EnumAvailable.Externt 
                        || (_user.Guilds.Contains(service.Guild) 
                            && model.Visible == CalendarModel.EnumAvailable.Internt) 
                        || model.Permission.Overlaps(roles) 
                        || model.View.Overlaps(roles)
                    ).ToList();
                    result.Add((service.Guild,value));
                }
            }
            
            
            return result;
        }
        
        
        
        async public Task<List<(SocketGuild, List<CalendarModel>)>> GetAllPermisionsCalendars(CalendarModel.EnumCalendarType calendarType)
        {
            
            if (_user.Verified is false)
            {
                return new List<(SocketGuild, List<CalendarModel>)>();
            }
            
            var roles = _user.GetRolesGuid();

            List<(SocketGuild,List<CalendarModel>)> result = new List<(SocketGuild, List<CalendarModel>)>();
            
            
            foreach (var socketGuild in _user.Guilds)
            {
                var service = new CalendarServices(socketGuild.Id.ToString());
            
                
                List<CalendarModel> value = service.GetAll(model => model.CalendarType == calendarType).Where(model => model.Permission.Overlaps(roles)).ToList();
                result.Add((socketGuild,value));
                 
                
            }

            return result;

        }
        
        
        
    }
}