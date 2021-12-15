using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Calendar.Equipment
{
    public class EquipmentModel : MainModel
    {
        [BsonElement("Tittle")]
        [Required]
        public string Title { get; set; }
        
        [BsonElement("EquiqmentId")] 
        public Guid EquiqmentId{ get; set; }
        
        [BsonElement("Decs")]
        [Required]
        public string Decs { get; set; }


        [BsonElement("Start")][BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)] 
        [DisplayFormat(DataFormatString = "{0:dddd, dd-MM-yyyy, HH:mm}")]
        [Required]
        public DateTime Start { get; set; }= DateTime.Today;

        
        [BsonElement("End")]
        [Required]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [DisplayFormat(DataFormatString = "{0:dddd, dd-MM-yyyy, HH:mm}")]
        public DateTime End {get; set; } = DateTime.Today;
        
        
    }
}