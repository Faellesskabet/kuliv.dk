using System;
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

        [BsonElement("Tags")] 
        public HashSet<Guid> Tags { get; set; }

        
        public List<TagsMainModel> getTags()
        {
            var tagSevice = new TagMongoService();
            return Tags.Select(t => tagSevice.Get(t)).ToList();
        }

        [BsonElement("Accepted")]
        public HashSet<string> Accepted { get; set; } = new HashSet<string>();

        [BsonElement("Declined")] 
        public HashSet<string> Declined { get; set; } = new HashSet<string>();
        
        
    }
}