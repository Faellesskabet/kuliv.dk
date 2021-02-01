using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models.Channel
{
    public abstract class ChannelModel : Model
    {
        [BsonElement("DiscordId")] [BsonUnique] 
        public string DiscordId { get; set; }
        [BsonElement("Name")] public string Name { get; set; }
        [BsonElement("CreatedAt")] public DateTime CreatedAt { get; set; }
        [BsonElement("PermissionOverwrite")] public Dictionary<string, Dictionary<string, Dictionary<string, string?>>> 
            PermissionsOverwrites { get; set; }
        [BsonElement("Position")] public int Position { get; set; }
        [BsonElement("DiscordCategoryId")] public string DiscordCategoryId { get; set; }
    }
}