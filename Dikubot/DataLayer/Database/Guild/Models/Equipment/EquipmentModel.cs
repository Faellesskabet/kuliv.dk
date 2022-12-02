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

namespace Dikubot.Extensions.Models.Equipment
{
    public class EquipmentModel : EventsAttributes
    {

        public EquipmentModel()
        {
            
        }

        public EquipmentModel(Guid id,
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

    }
}