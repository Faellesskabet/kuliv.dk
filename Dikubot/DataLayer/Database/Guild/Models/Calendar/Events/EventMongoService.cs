using System.Collections.Generic;
using System.Linq;
using Dikubot.Discord;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Guild.Models.Calendar.Events
{
    public class EventsMongoService : GuildMongoService<EventModel>
    {
        private string _calendar;
        
        public EventsMongoService(SocketGuild guild, string calendar = null) : base("Events", guild)
        {
            _calendar = calendar;
        }
        
        
        public EventsMongoService(string guidId, string calendar = null) : base("Events", DiscordBot.ClientStatic.Guilds?.FirstOrDefault(g => g.Id.ToString().Equals(guidId)))
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