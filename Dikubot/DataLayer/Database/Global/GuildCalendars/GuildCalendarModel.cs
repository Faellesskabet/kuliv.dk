using System;
using System.Collections.Generic;
using Dikubot.DataLayer.Database.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.GuildCalendars;

public class GuildCalendarModel : MainModel
{
    [BsonElement("GuildId")] [BsonUnique] public ulong GuildId { get; set; }

    /// <summary>
    /// Calendars is a hashset of guids, each corrosponding to a CalendarModel
    /// </summary>
    [BsonElement("Calendars")]
    public HashSet<Guid> Calendars { get; set; } = new();
}