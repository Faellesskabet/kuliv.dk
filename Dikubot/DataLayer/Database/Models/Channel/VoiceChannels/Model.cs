using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Dikubot.Database.Models.Channel;
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
    public class VoiceChannelModel : ChannelModel
    {
        [BsonElement("Bitrate")] public int Bitrate { get; set; }
        [BsonElement("UserLimit")] public int? UserLimit { get; set; }
        [BsonElement("DeleteOnLeave")] public bool DeleteOnLeave { get; set; }
        [BsonElement("Child")] public string Child { get; set; }
        [BsonElement("ExpandOnJoin")] public bool ExpandOnJoin { get; set; }
    }
}