using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Calendar
{
    public class CalendarModel : MainModel
    {
        [BsonElement("Name")] public string Name { get; set; }

        [BsonElement("GuildId")] public string GuildId { get; set; }


        /// <Summary>Who can see the calendar.</Summary>
        [BsonElement("Visible")]
        public EnumAvailable Visible { get; set; }

        /// <Summary>Who can made events in the the Calaendar</Summary>
        [BsonElement("Permission ")]
        public HashSet<Guid> Permission { get; set; }

        /// <Summary>
        /// If private, which roles can see the events in the the Calaendar
        /// Other than those who has Permission
        /// </Summary>
        [BsonElement("View")]
        public HashSet<Guid> View { get; set; }

        /// <Summary>How the Calendar Displays</Summary>
        [BsonElement("Display")]
        public EnumDisplay Display { get; set; }
        
        /// <Summary>How the Calendar googleCalendarId</Summary>
        [BsonElement("GoogleCalendarId")]
        public string GoogleCalendarId { get; set; }

        /// <Summary>How the Calendar color</Summary>
        [BsonElement("Color")]
        public string Color { get; set; }

        [BsonElement("CalendarType")] public EnumCalendarType CalendarType { get; set; }

        public enum EnumCalendarType
        {
            Event,
            Equipment,
            Room,
            Google
        }

        public enum EnumAvailable
        {
            Privat,
            Internt,
            Externt
        }

        public enum EnumDisplay
        {
            auto,
            background,
            [Display(Name = "inverse-Background")] inverseBackground,
            block,
            [Display(Name = "list-Item")] listItem,
            none
        }
    }
}