using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Discord;
using Discord.WebSocket;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace Dikubot.Database.Models.VoiceChannel
{
    /// <summary>
    /// Class for elements in the VoiceChannel collection.
    /// </summary>
    public class VoiceChannelModel : Model
    {
        [BsonElement("DiscordId")] [BsonUnique] 
        public string DiscordId { get; set; }
        [BsonElement("Name")] public string Name { get; set; }
        [BsonElement("CreatedAt")] public DateTime CreatedAt { get; set; }
        [BsonElement("PermissionOverwrite")] public Dictionary<string, Dictionary<string, Dictionary<string, string?>>> PermissionsOverwrites { get; set; }
        [BsonElement("Position")] public int Position { get; set; }
        [BsonElement("Bitrate")] public int Bitrate { get; set; }
        [BsonElement("DiscordCategoryId")] public string DiscordCategoryId { get; set; }
        [BsonElement("UserLimit")] public int? UserLimit { get; set; }
    }
}