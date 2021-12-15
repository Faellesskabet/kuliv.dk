using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages.Quote
{
    public class QuoteModel : MainModel
    {
        [BsonElement("MessageId")]
        public string MessageId { get; set; }
        
        [BsonElement("ChannelId")]
        public string ChannelId { get; set; }
        
        [BsonElement("TimeStamp")]
        public DateTimeOffset TimeStamp { get; set; }
    }
}