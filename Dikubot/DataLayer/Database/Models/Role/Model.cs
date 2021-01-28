using System;
using System.Collections.Generic;
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
        [BsonElement("Name")] [BsonUnique] 
        public string Name { get; set; }
        [BsonElement("Color")] public Dictionary<string, int> Color { get; set; }
        [BsonElement("CreatedAt")] public DateTime CreatedAt { get; set; }
        [BsonElement("IsEveryone")] public bool IsEveryone { get; set; }
        [BsonElement("IsHoisted")] public bool IsHoisted { get; set; }
        [BsonElement("IsManaged")] public bool IsManaged { get; set; }
        [BsonElement("IsMentionable")] public bool IsMentionable { get; set; }
        [BsonElement("Permissions")] public Dictionary<string, bool> Permissions { get; set; }
        [BsonElement("Position")] public int Position { get; set; }
    }
}