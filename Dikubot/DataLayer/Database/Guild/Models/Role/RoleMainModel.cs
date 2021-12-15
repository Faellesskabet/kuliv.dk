using System;
using System.ComponentModel.DataAnnotations;
using Dikubot.DataLayer.Database.Guild.Models.Role.SubModels.Color;
using Dikubot.DataLayer.Database.Guild.Models.Role.SubModels.GuildPermissions;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Role
{
    /// <summary>
    /// Class for elements in the User collection.
    /// </summary>
    public class RoleMainModel : MainModel
    {
        [BsonElement("DiscordId")]
        [BsonUnique]
        public string DiscordId { get; set; }

        [BsonElement("Name")] public string Name { get; set; }
        [BsonElement("Color")] [DisplayFormat(DataFormatString = "{0:A}")] public ColorModel  Color { get; set; }

        [BsonElement("CreatedAt")] [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")] public DateTime CreatedAt { get; set; }
        [BsonElement("IsEveryone")] public bool IsEveryone { get; set; }
        [BsonElement("IsHoisted")] public bool IsHoisted { get; set; }
        [BsonElement("IsManaged")] public bool IsManaged { get; set; }
        [BsonElement("IsMentionable")] public bool IsMentionable { get; set; }
        [BsonElement("Permissions")] public GuildPermissionsModel Permissions { get; set; }
        [BsonElement("Position")] public int Position { get; set; }
    }
}