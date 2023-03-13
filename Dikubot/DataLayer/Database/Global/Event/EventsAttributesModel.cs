using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.Event;

public class EventsAttributes : MainModel
{
    [BsonElement("Subject")]
    [BsonRequired]
    public string Subject { get; set; }

    [BsonElement("Location")] public string Location { get; set; }

    [BsonElement("Description")]
    [BsonRequired]
    public string Description { get; set; }

    [BsonElement("StartTime")]
    [BsonRepresentation(BsonType.DateTime)]
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    [Required]
    public DateTime StartTime { get; set; } = DateTime.Now;

    [BsonElement("EndTime")]
    [Required]
    [BsonRepresentation(BsonType.DateTime)]
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime EndTime { get; set; } = DateTime.Now;

    [BsonElement("RecurrenceRule")] public string RecurrenceRule { get; set; }

    [BsonElement("RecurrenceException")] public string RecurrenceException { get; set; }

    [BsonElement("IsAllDay")] public bool IsAllDay { get; set; }

    /// <summary>
    /// CreatedBy is a should be a UserGlobalModel guid
    /// </summary>
    [BsonElement("CreatedBy")]
    [BsonRequired]
    public Guid CreatedBy { get; set; }

    [BsonIgnore] public bool IsReadonly { get; set; } = true;
}