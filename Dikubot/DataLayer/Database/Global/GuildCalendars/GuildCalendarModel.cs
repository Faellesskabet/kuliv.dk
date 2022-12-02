using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.GuildCalendars;

public class GuildCalendarModel : MainModel
{
    [BsonElement("GuildId")] [BsonUnique]
    public ulong GuildId { get; set; }
    
    /// <summary>
    /// Calendars is a hashset of guids, each corrosponding to a CalendarModel
    /// </summary>
    [BsonElement("Calendars")]
    public HashSet<Guid> Calendars { get; set; } = new HashSet<Guid>();
    
    /// <summary>
    /// Calendars is a hashset of guids, each corrosponding to a EventModel
    /// </summary>
    [BsonElement("Events")]
    public HashSet<Guid> Events { get; set; } = new HashSet<Guid>();

}