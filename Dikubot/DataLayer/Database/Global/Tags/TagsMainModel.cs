using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.Tags;

public class TagsMainModel : MainModel
{
    public enum enumTagType
    {
        tag,
        category
    }

    public enum enumTextColor
    {
        white,
        black
    }

    [Required] [BsonElement("Name")] public string Name { get; set; }

    [Required] [BsonElement("Decs")] public string Decs { get; set; }

    [Required] [BsonElement("Color")] public string Color { get; set; }

    [Required] [BsonElement("TextColor")] public enumTextColor TextColor { get; set; }

    [BsonElement("TagType")] public enumTagType TagType { get; set; } = enumTagType.tag;

    [BsonElement("Category")] public HashSet<Guid> Category { get; set; } = new();

    [BsonIgnore]
    public IEnumerable<Guid> CategoryEnumerable
    {
        get => Category;
        set => Category = new HashSet<Guid>(value);
    }
}