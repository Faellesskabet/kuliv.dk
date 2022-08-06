using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dikubot.DataLayer.Database.Guild.Models.Calendar.Events;
using Dikubot.Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild.Models.Calendar
{//https://stackoverflow.com/questions/13905359/cannot-provide-arguments-when-creating-an-instance-of-generic-type
    public class CalendarServices : GuildServices<CalendarModel>
    {
        
        public CalendarServices(SocketGuild guild) : base("Calendar", guild)
        {
        }
        

        public CalendarServices(string guidId) : base("Calendar", DiscordBot.Client.Guilds?.FirstOrDefault(g => g.Id.ToString().Equals(guidId)))
        {
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
                    res.AddRange(eventsServices.GetAllAsDictionary(eventFilter).SelectMany(x => x.Value).Where(e => e.Calendars.Contains(model.Id.ToString())));
                }
            }
            return res;
        }
    }

   


}