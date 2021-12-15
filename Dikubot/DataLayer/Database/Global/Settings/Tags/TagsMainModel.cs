using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.Settings.Tags
{
    public class TagsMainModel : MainModel
    {
        [Required]
        [BsonElement("Name")] public string Name { get; set; }
        
        [Required]
        [BsonElement("Decs")] public string Decs { get; set; }
        
        [Required]
        [BsonElement("Color")] public string Color { get; set; }
        
        [Required]
        [BsonElement("TextColor")] public enumTextColor TextColor { get; set; }

        public enum enumTextColor
        {
            white,
            black
        }
    }
}