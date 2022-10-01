﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dikubot.DataLayer.Database.Global.Settings.Tags;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Calendar.Events
{
    public class EventModel : EventsAttributes
    {
        
        [BsonElement("Calendars")] 
        public string Calendars{ get; set; }

        [BsonElement("Hosts")] 
        public HashSet<string> Hosts{ get; set; }
        

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
        public List<TagsMainModel> GetTags()
        {
            var tagSevice = new TagServices();
            return Tags.Select(t => tagSevice.Get(t)).ToList();
        }
        
        
        

        [BsonElement("Accepted")]
        public HashSet<string> Accepted { get; set; } = new HashSet<string>();

        [BsonElement("Declined")] public HashSet<string> Declined { get; set; } = new HashSet<string>();
        
        
        
    }
}