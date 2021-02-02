using System;
using System.Collections.Generic;
using Dikubot.Database.Models.Channel.SubModels;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models.Channel
{
    public abstract class ChannelModel : Model
    {
        [BsonElement("DiscordId")] [BsonUnique] 
        public string DiscordId { get; set; }
        [BsonElement("Name")] public string Name { get; set; }
        [BsonElement("CreatedAt")] public DateTime CreatedAt { get; set; }
        [BsonElement("PermissionOverwrites")] public List<OverwriteModel> PermissionOverwrites { get; set; }
        [BsonElement("Position")] public int Position { get; set; }
    }
}