using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Dikubot.DataLayer.Database.Global.Settings.Tags;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Calendar.Events
{
    public class EventModel : EventsAttributes
    {

        [BsonElement("Calendars")] public HashSet<Guid> Calendars { get; set; } = new HashSet<Guid>();
        [BsonIgnore]
        public IEnumerable<Guid> CalendarsEnumerable { get => Calendars; set => Calendars = new HashSet<Guid>(value); }

        [BsonElement("Hosts")] public HashSet<string> Hosts { get; set; } = new HashSet<string>();

        [BsonElement("HostServers")]
        public HashSet<Guid> HostServers{ get; set; } = new HashSet<Guid>();
        
        [BsonIgnore]
        public IEnumerable<Guid> HostServersEnumerable { get => HostServers; set => HostServers = new HashSet<Guid>(value); }
        
        public List<CalendarModel> GetHostServers(string GuildId)
        {
            var Sevice = new CalendarServices(GuildId);
            return HostServers.Select(t => Sevice.Get(t)).ToList();
        }

        [BsonElement("Tags")] public HashSet<Guid> Tags { get; set; } = new HashSet<Guid>();
        
        [BsonIgnore]
        public IEnumerable<Guid> TagsEnumerable { get => Tags; set => Tags = new HashSet<Guid>(value); }

        public override HashSet<Guid> GetTags()
        {
            return Tags;
        }

        public override List<string> GetSearchContent()
        {
            return new List<string>()
            {
                Subject,
                Description,
                Location
            };
        }

        [BsonElement("Accepted")]
        public HashSet<string> Accepted { get; set; } = new HashSet<string>();

        [BsonElement("Declined")] public HashSet<string> Declined { get; set; } = new HashSet<string>();

        
        public string Time()
        {

            if (!StartTime.Year.Equals(EndTime.Year))
            {
                if (IsAllDay)
                {
                    return $"{StartTime.ToString("'d.'dd/MM/yyyy", CultureInfo.InvariantCulture)} - " +
                           $"{EndTime.ToString("'d.'dd/MM/yyyy", CultureInfo.InvariantCulture)}";
                }
                return $"{StartTime.ToString("'Kl.'HH:mm 'd.'dd/MM/yyyy", CultureInfo.InvariantCulture)} - " +
                       $"{EndTime.ToString("'Kl.'HH:mm 'd.'dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            } if(!StartTime.Month.Equals(EndTime.Month))
            {
                if (IsAllDay)
                {
                    return $"{StartTime.ToString("'d.'dd/MM", CultureInfo.InvariantCulture)} - " +
                           $"{EndTime.ToString("'d.'dd/MM/yyyy", CultureInfo.InvariantCulture)}";
                }
                return $"{StartTime.ToString("'Kl.'HH:mm 'd.'dd/MM", CultureInfo.InvariantCulture)} - " +
                       $"{EndTime.ToString("'Kl.'HH:mm 'd.'dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            }
            if(!StartTime.Day.Equals(EndTime.Day))
            {
                if (IsAllDay)
                {
                    return $"{StartTime.ToString("'d.'dd", CultureInfo.InvariantCulture)} - " +
                           $"{EndTime.ToString("'d.'dd/MM/yyyy", CultureInfo.InvariantCulture)}";
                }
                return $"{StartTime.ToString("'Kl.'HH:mm 'd.'dd", CultureInfo.InvariantCulture)} - " +
                       $"{EndTime.ToString("'Kl.'HH:mm 'd.'dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            }
            if (IsAllDay)
            {
                return $"{StartTime.ToString("'d.'dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            }
            return $"{StartTime.ToString("'Kl.'HH:mm", CultureInfo.InvariantCulture)} - " +
                   $"{EndTime.ToString("'Kl.'HH:mm 'd.'dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            
                
        }
        
    }
}