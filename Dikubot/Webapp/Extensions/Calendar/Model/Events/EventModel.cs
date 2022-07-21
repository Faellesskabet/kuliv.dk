using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dikubot.DataLayer.Database.Global.Settings.Tags;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Calendar.Events
{
    public class EventModel : MainModel
    {
        [BsonElement("Tittle")]
        [Required]
        public string Title { get; set; }

        [BsonElement("Place")] 
        public string Place { get; set; }

        [BsonElement("Description")] 
        public string Description { get; set; }
        
        [BsonElement("Calendars")] 
        public string Calendars{ get; set; }

        [BsonElement("Hosts")] 
        public HashSet<string> Hosts{ get; set; }

        [BsonElement("Tags")] 
        public HashSet<Guid> Tags { get; set; }

        
        public List<TagsMainModel> getTags()
        {
            var tagSevice = new TagServices();
            return Tags.Select(t => tagSevice.Get(t)).ToList();
        }

        [BsonElement("Accepted")]
        public HashSet<string> Accepted { get; set; } = new HashSet<string>();

        [BsonElement("Declined")] public HashSet<string> Declined { get; set; } = new HashSet<string>();
        
     
        [BsonElement("Start")][BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)] 
        [DisplayFormat(DataFormatString = "{0:dddd, dd-MM-yyyy, HH:mm}")]
        [Required]
        public DateTime Start { get; set; }= DateTime.Today;

        
        [BsonElement("End")]
        [Required]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [DisplayFormat(DataFormatString = "{0:dddd, dd-MM-yyyy, HH:mm}")]
        public DateTime End {get; set; } = DateTime.Today;

            
        [BsonElement("AllDay")]
        public bool AllDay { get; set; }
        
    }
}