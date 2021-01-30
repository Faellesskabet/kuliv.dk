using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Discord;
using Discord.WebSocket;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace Dikubot.Database.Models.TextChannel
{
    /// <summary>
    /// Class for elements in the TextChannel collection.
    /// </summary>
    public class TextChannelModel : Model
    {
        [BsonElement("DiscordId")] [BsonUnique] 
        public string DiscordId { get; set; }
        [BsonElement("Name")] public string Name { get; set; }
        [BsonElement("CreatedAt")] public DateTime CreatedAt { get; set; }
        [BsonElement("PermissionOverwrite")] public Dictionary<string, Dictionary<string, Dictionary<string, string?>>> 
            PermissionsOverwrites { get; set; }
        [BsonElement("Position")] public int Position { get; set; }
        [BsonElement("Topic")] public string Topic { get; set; }
        [BsonElement("IsNsfw")] public bool IsNsfw { get; set; }
        [BsonElement("SlowModeInterval")] public int SlowModeInterval { get; set; }
        [BsonElement("DiscordCategoryId")] public string DiscordCategoryId { get; set; }
    }
}