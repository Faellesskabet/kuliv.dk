using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.Calendar;

public class CalendarModel : MainModel
{
    public enum EnumDisplay
    {
        auto,
        background,
        [Display(Name = "inverse-Background")] inverseBackground,
        block,
        [Display(Name = "list-Item")] listItem,
        none
    }

    [StringLength(256)]
    [BsonElement("Name")]
    public string Name { get; set; }

    /// <Summary>Who can see the calendar.</Summary>
    [BsonElement("Visible")]
    public bool Visible { get; set; } = true;

    /// <summary>
    ///     A calendar contains a list of events. This hashset is contains the EventModels
    /// </summary>
    [BsonElement("Events")]
    public HashSet<Guid> Events { get; set; } = new();

    /// <Summary>The Description for the calendar</Summary>
    [BsonElement("Description")]
    [StringLength(4096)]
    public string Description { get; set; }

    /// <Summary>How the Calendar Displays</Summary>
    [BsonElement("Display")]
    public EnumDisplay Display { get; set; }

    [BsonElement("GoogleCalendarId")]
    [StringLength(2048)]
    public string GoogleCalendarId { get; set; }

    [BsonElement("Color")] public string Color { get; set; }
}