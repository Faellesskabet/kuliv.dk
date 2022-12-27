using System;
using System.Collections.Generic;
using System.Globalization;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.Event;

public class EventModel : EventsAttributes
{
    [BsonElement("Tags")] public HashSet<Guid> Tags { get; set; } = new();

    [BsonElement("Accepted")] public HashSet<string> Accepted { get; set; } = new();

    [BsonElement("Declined")] public HashSet<string> Declined { get; set; } = new();

    public override HashSet<Guid> GetTags()
    {
        return Tags;
    }
    
    [BsonIgnore]
    public IEnumerable<Guid> TagsEnumerable { get => Tags; set => Tags = new HashSet<Guid>(value); }


    public override List<string> GetSearchContent()
    {
        return new List<string>
        {
            Subject,
            Description,
            Location
        };
    }

    public string Time()
    {
        if (!StartTime.Year.Equals(EndTime.Year))
        {
            if (IsAllDay)
                return $"{StartTime.ToString("'d.'dd/MM/yyyy", CultureInfo.InvariantCulture)} - " +
                       $"{EndTime.ToString("'d.'dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            return $"{StartTime.ToString("HH:mm", CultureInfo.InvariantCulture)} - " +
                   $"{EndTime.ToString("HH:mm", CultureInfo.InvariantCulture)}" +
                   $"{StartTime.ToString("'d.'dd/MM/yyyy", CultureInfo.InvariantCulture)} - " +
                   $"{EndTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
        }

        if (!StartTime.Month.Equals(EndTime.Month))
        {
            if (IsAllDay)
                return $"{StartTime.ToString("'d.'dd/MM", CultureInfo.InvariantCulture)} - " +
                       $"{EndTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            return $"{StartTime.ToString("HH:mm", CultureInfo.InvariantCulture)} - " +
                   $"{EndTime.ToString("HH:mm ", CultureInfo.InvariantCulture)}"
                   + $"{StartTime.ToString("'d.'dd/MM", CultureInfo.InvariantCulture)} - "
                   + $"{EndTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
        }

        if (!StartTime.Day.Equals(EndTime.Day))
        {
            if (IsAllDay)
                return $"{StartTime.ToString("'d.'dd", CultureInfo.InvariantCulture)} - " +
                       $"{EndTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            return $"{StartTime.ToString("HH:mm", CultureInfo.InvariantCulture)} - " +
                   $"{EndTime.ToString("HH:mm", CultureInfo.InvariantCulture)}" +
                   $"{StartTime.ToString("'d' dd", CultureInfo.InvariantCulture)} - " +
                   $"{EndTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
        }

        if (IsAllDay) return $"{StartTime.ToString("'d.'dd/MM/yyyy", CultureInfo.InvariantCulture)}";
        return $"{StartTime.ToString("'Kl.'HH:mm", CultureInfo.InvariantCulture)} - " +
               $"{EndTime.ToString("HH:mm 'd.'dd/MM/yyyy", CultureInfo.InvariantCulture)}";
    }
}