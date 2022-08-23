using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Microsoft.IdentityModel.Tokens;
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
        [BsonElement("Permission")]
        public HashSet<Guid> Permission { get; set; }

        public IEnumerable<RoleMainModel> GetPermissionRoles(RoleMongoService roleMongoService)
        {
            IEnumerable<RoleMainModel> roles = Permission.IsNullOrEmpty() ? new List<RoleMainModel>() : 
                Permission.Select(guid => roleMongoService.Get(model => model.Id == guid)).Where(model => model != null);
            
            // Replace by ForiegnKey system
            var roleMainModels = roles as RoleMainModel[] ?? roles.ToArray();
            Permission = new HashSet<Guid>(roleMainModels.Select(model => model.Id));
            return roleMainModels;
        }
        
        [BsonIgnore]
        public IEnumerable<Guid> PermissionEnumerable { get => Permission; set => Permission = new HashSet<Guid>(value); }
        
        /// <Summary>The Decscrition for the calendar</Summary>
        [BsonElement("Decs")]
        public string Decs { get; set; }
        
        
        /// <Summary>
        /// If private, which roles can see the events in the the Calaendar
        /// Other than those who has Permission
        /// </Summary>
        [BsonElement("View")]
        public HashSet<Guid> View { get; set; }
        
        public IEnumerable<RoleMainModel> GetViewRoles(RoleMongoService roleMongoService)
        {
            IEnumerable<RoleMainModel> roles = View.IsNullOrEmpty() ? new List<RoleMainModel>() : 
                View.Select(guid => roleMongoService.Get(model => model.Id == guid)).Where(model => model != null);
            
            // Replace by ForiegnKey system
            var roleMainModels = roles as RoleMainModel[] ?? roles.ToArray();
            View = new HashSet<Guid>(roleMainModels.Select(model => model.Id));
            return roleMainModels;
        }
        
        [BsonIgnore]
        public IEnumerable<Guid> ViewEnumerable { get => View; set => View = new HashSet<Guid>(value); }
        

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
            Externt,
            Public
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