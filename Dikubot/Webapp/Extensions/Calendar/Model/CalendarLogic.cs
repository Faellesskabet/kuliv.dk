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


        async public Task<(List<(SocketGuild, List<CalendarModel>)>, List<(SocketGuild, List<CalendarModel>)>)>
            GetAllListListViewAndPermissionsCalenders(CalendarModel.EnumCalendarType calendarType)
        {
            (List<(SocketGuild,List<CalendarModel>)>,List<(SocketGuild,List<CalendarModel>)>) 
                result = (new List<(SocketGuild, List<CalendarModel>)>(),new List<(SocketGuild, List<CalendarModel>)>());

            if (!_user.Verified)
            {
                foreach (var service in _calendarServicesList)
                {
                    List<CalendarModel> value = service.GetAll(model => model.CalendarType == calendarType).Where(
                        model =>
                            model.Visible == CalendarModel.EnumAvailable.Public).ToList();
                    result.Item1.Add((service.Guild, value));
                }
                return result;
            }
            
            var roles = _user.GetRolesGuid() ?? new HashSet<Guid>();
            
            foreach (var service in _calendarServicesList) 
            {
                List<CalendarModel> views = new List<CalendarModel>();
                List<CalendarModel> permissions = new List<CalendarModel>();
                    foreach (var calendarModel in service
                        .GetAll(model => model.CalendarType == calendarType))
                    {
                        if (calendarModel.Permission.Overlaps(roles))
                        {
                            permissions.Add(calendarModel);
                            views.Add(calendarModel);
                        }
                        else if (calendarModel.Visible is CalendarModel.EnumAvailable.Public 
                            or CalendarModel.EnumAvailable.Externt 
                                 || (_user.Guilds.Contains(service.Guild) 
                                     && (calendarModel.Visible == CalendarModel.EnumAvailable.Internt 
                                         || calendarModel.View.Overlaps(roles))))
                        {
                            views.Add(calendarModel);
                        }
                    }
                    result.Item1.Add((service.Guild,views));
                    result.Item2.Add((service.Guild,permissions));
            }
            return result;
        }
        
        async public Task<(List<CalendarModel>, List<CalendarModel>)>
            GetAllListViewAndPermissionsCalenders(CalendarModel.EnumCalendarType calendarType)
        {
            (List<CalendarModel>, List<CalendarModel>)
                result = (new List<CalendarModel>(), new List<CalendarModel>());
                    
            if (!_user.Verified)
            {
                foreach (var service in _calendarServicesList)
                {
                    List<CalendarModel> value = service.GetAll(model => model.CalendarType == calendarType).Where(
                        model =>
                            model.Visible == CalendarModel.EnumAvailable.Public).ToList();
                    result.Item1.AddRange(value);
                }
                return result;
            }
            
            var roles = _user.GetRolesGuid() ?? new HashSet<Guid>();
            
            foreach (var service in _calendarServicesList) 
            {
                foreach (var calendarModel in service
                        .GetAll(model => model.CalendarType == calendarType))
                    {
                        if (calendarModel.Permission.Overlaps(roles))
                        {
                            result.Item2.Add(calendarModel);
                            result.Item1.Add(calendarModel);
                        }
                        else if (calendarModel.Visible is CalendarModel.EnumAvailable.Public 
                            or CalendarModel.EnumAvailable.Externt 
                                 || (_user.Guilds.Contains(service.Guild) 
                                     && (calendarModel.Visible == CalendarModel.EnumAvailable.Internt 
                                         || calendarModel.View.Overlaps(roles))))
                        {
                            result.Item1.Add(calendarModel);
                        }
                    }
            }
            return result;
        }
        

        async public Task<List<(SocketGuild, List<CalendarModel>)>> GetAllViewCalenders(CalendarModel.EnumCalendarType calendarType)
        {
            return GetAllListListViewAndPermissionsCalenders(calendarType).Result.Item1;
        }
        
        
        
        async public Task<List<(SocketGuild, List<CalendarModel>)>> GetAllPermisionsCalendars(CalendarModel.EnumCalendarType calendarType)
        {
            return GetAllListListViewAndPermissionsCalenders(calendarType).Result.Item2;
        }
        
        
        
    }
}