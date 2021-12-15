using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Guild.Submodels.GuildOptions
{
    public class GuildOptions : MainModel
    {
        [BsonElement("Tags")] public List<Guid> Tags { get; set; }
        
    }
}