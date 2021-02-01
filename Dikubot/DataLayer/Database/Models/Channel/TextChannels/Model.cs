using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Dikubot.Database.Models.Channel;
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
    public class TextChannelModel : ChannelModel
    {
        [BsonElement("Topic")] public string Topic { get; set; }
        [BsonElement("IsNsfw")] public bool IsNsfw { get; set; }
        [BsonElement("SlowModeInterval")] public int SlowModeInterval { get; set; }
    }
}