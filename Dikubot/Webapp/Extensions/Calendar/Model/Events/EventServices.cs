using System.Collections.Generic;
using System.Linq;
using Dikubot.DataLayer.Database.Global;
using Dikubot.DataLayer.Database.Global.Settings.Tags;
using Dikubot.Discord;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Guild.Models.Calendar.Events
{
    public class EventsServices : GlobalServices<EventModel>
    {
        private string _calendar;
        
        public EventsServices(SocketGuild guildId, string calendar = null) : base("Events")
        {
            _calendar = calendar;
        }
        
        public EventsServices(string guildId = null, string calendar = null) : base("Events")
        {
            _calendar = calendar;
        }
        
        public override List<EventModel> Get()
        {
            if (string.IsNullOrWhiteSpace(_calendar))
            {
                return base.Get();
            }
            return _models.Find(e => e.Calendars.Equals(_calendar)).ToList();
        }
        
    }
}