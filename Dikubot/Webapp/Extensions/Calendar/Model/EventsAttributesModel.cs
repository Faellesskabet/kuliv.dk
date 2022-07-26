using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Calendar
{
    public class EventsAttributes : MainModel
    {
        
        
        [BsonElement("Subject")] 
        public string Subject { get; set; }
        
        [BsonElement("Location")] 
        public string Location { get; set; }

        [BsonElement("Description")] 
        public string Description { get; set; }

        [BsonElement("StartTime")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        //[DisplayFormat(DataFormatString = "{0:dddd, dd-MM-yyyy, HH:mm}")]
        [Required]
        public DateTime StartTime { get; set; } = new DateTime();

        [BsonElement("EndTime")]
        [Required]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        //[DisplayFormat(DataFormatString = "{0:dddd, dd-MM-yyyy, HH:mm}")]
        public DateTime EndTime {get; set; } = new DateTime();
        
        [BsonElement("RecurrenceRule")] 
        public string RecurrenceRule { get; set; }
        
        [BsonElement("RecurrenceException")] 
        public string RecurrenceException { get; set; }

        [BsonElement("IsAllDay")]
        public bool IsAllDay { get; set; }

        [BsonElement("CreateBy")] public string CreateBy { get; set; }

        [BsonIgnore]
        public bool IsReadonly { get; set; } = true;


    }
}