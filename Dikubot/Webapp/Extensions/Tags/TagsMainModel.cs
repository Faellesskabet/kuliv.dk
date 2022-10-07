using System;
using System.Collections.Generic;
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

        [BsonElement("TagType")] public enumTagType TagType { get; set; } = enumTagType.tag;

        [BsonElement("Category")] public HashSet<Guid> Category { get; set; } = new HashSet<Guid>();
        
        [BsonIgnore]
        public IEnumerable<Guid> CategoryEnumerable { get => Category; set => Category = new HashSet<Guid>(value); }
        public enum enumTextColor
        {
            white,
            black
        }
        
        public enum enumTagType
        {
            tag,
            category
        }
    }
}