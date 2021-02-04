using System;
using System.Collections.Generic;
using Dikubot.Database.Models.Role.SubModels;
using Dikubot.Discord;
using Discord;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace Dikubot.Database.Models.Role
{
    /// <summary>
    /// Class for elements in the User collection.
    /// </summary>
    public class RoleModel : Model
    {
        [BsonElement("DiscordId")] [BsonUnique] 
        public string DiscordId { get; set; }
        [BsonElement("Name")] public string Name { get; set; }
        [BsonElement("Color")] public ColorModel Color { get; set; }
        [BsonElement("CreatedAt")] public DateTime CreatedAt { get; set; }
        [BsonElement("IsEveryone")] public bool IsEveryone { get; set; }
        [BsonElement("IsHoisted")] public bool IsHoisted { get; set; }
        [BsonElement("IsManaged")] public bool IsManaged { get; set; }
        [BsonElement("IsMentionable")] public bool IsMentionable { get; set; }
        [BsonElement("Permissions")] public GuildPermissionsModel Permissions { get; set; }
        [BsonElement("Position")] public int Position { get; set; }
    }
}