using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Graph;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Dikubot.DataLayer.Database.Guild.Models.Calendar;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Microsoft.IdentityModel.Tokens;

namespace Dikubot.DataLayer.Database.Guild.Models.Room
{
    public class RoomModel : EventsAttributes
    {

        public RoomModel()
        {
            
        }

        public RoomModel(Guid id,
            string description, 
            string subject,
            string location, 
            DateTime startTime, 
            DateTime endTime,
            bool isAllday,
            Guid equiqmentId,
            string recurrenceRule,
            string recurrenceException
            )
        {
            this.Id = id;
            this.Description = description;
            this.Location = location;
            this.Subject = subject;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.EquiqmentId = equiqmentId;
            this.IsAllDay = isAllday;
            this.RecurrenceRule = recurrenceRule;
            this.RecurrenceException = recurrenceException;
        }
        
        
        [BsonElement("EquiqmentId")] 
        public Guid EquiqmentId{ get; set; } 
        
        
        /*    
        public IEnumerable<CalendarModel> GetPermissionRoles(CalendarServices roleServices)
        {
            IEnumerable<CalendarModel> roles = EquiqmentId.IsNullOrEmpty() ? new List<CalendarModel>() : 
                EquiqmentId.Select(guid => roleServices.Get(model => model.Id == guid)).Where(model => model != null);
            
            // Replace by ForiegnKey system
            var roleMainModels = roles as CalendarModel[] ?? roles.ToArray();
            EquiqmentId = new HashSet<Guid>(roleMainModels.Select(model => model.Id));
            return roleMainModels;
        }
        
        [BsonIgnore]
        public IEnumerable<Guid> EquiqmentIdEnumerable { get => EquiqmentId; set => EquiqmentId = new HashSet<Guid>(value); }
*/
        
    }
}