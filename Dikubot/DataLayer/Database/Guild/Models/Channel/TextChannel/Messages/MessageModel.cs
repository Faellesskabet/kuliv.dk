using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages;

public class MessageModel : MainModel
{
    [BsonElement("MessageId")] public string MessageId { get; set; }

    [BsonElement("ChannelId")] public string ChannelId { get; set; }

    [BsonElement("TimeStamp")] public DateTimeOffset TimeStamp { get; set; }
}