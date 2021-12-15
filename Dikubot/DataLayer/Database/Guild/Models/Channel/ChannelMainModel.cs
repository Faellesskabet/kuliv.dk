using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dikubot.DataLayer.Database.Guild.Models.Channel.SubModels.Overwrite;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Channel
{
    public abstract class ChannelMainModel : MainModel
    {
        [BsonElement("DiscordId")]
        [BsonUnique]
        public string DiscordId { get; set; }

        [BsonElement("Name")] public string Name { get; set; }
        [BsonElement("CreatedAt")] public DateTime CreatedAt { get; set; }
        [BsonElement("PermissionOverwrites")] [Display(Order = -1)] public List<OverwriteMainModel> PermissionOverwrites { get; set; }
        [BsonElement("Position")] [Display(Order = -1)] public int Position { get; set; }
    }
}