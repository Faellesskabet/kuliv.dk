using System;
using System.Collections.Generic;
using System.Linq;
using Dikubot.DataLayer.Database;
using Dikubot.DataLayer.Database.Global.Settings.Tags;
using Discord.Commands;
using MongoDB.Bson.Serialization.Attributes;
using Dikubot.DataLayer.Database.Global.GuildSettings;

namespace Dikubot.Webapp.Extensions.Discovery.Links
{
    public class UnionModel : MainModel
    {

        public UnionModel()
        {
        }

        public UnionModel(GuildSettingsModel settingsModel)
        {
            Title = settingsModel.Name;
            Decs = settingsModel.Description;
            Discord = settingsModel.GuildId.ToString();
            TagsEnumerable = settingsModel.TagsEnumerable;
            LogoUrl = settingsModel.LogoUrl;
            Facebook = settingsModel.FacebookUrl;
            Href = settingsModel.Webpage;
        }

        public override List<string> GetSearchContent()
        {
            return new List<string>()
            {
                Title, Decs
            };
        }

        public override HashSet<Guid> GetTags()
        {
            return Tags;
        }

        [BsonElement("Title")] 
        public string Title { get; set; }
        
        [BsonElement("Decs")] 
        public string Decs { get; set; }
        
        
        [BsonElement("LogoUrl")] 
        public string LogoUrl { get; set; }

        [BsonElement("Href")] 
        public string Href { get; set; }
        
        [BsonElement("Facebook")] 
        public string Facebook { get; set; }
        
        [BsonElement("Mail")] 
        public string Mail { get; set; }
        
        [BsonElement("Discord")] 
        public string Discord { get; set; }
        
        
        /// <summary>
        /// The tags associated with the guild. They're used for searching and filtering
        /// </summary>
        [BsonElement("Tags")]
        public HashSet<Guid> Tags { get; set; } = new HashSet<Guid>();
    
        [BsonIgnore]
        public IEnumerable<Guid> TagsEnumerable { get => Tags; set => Tags = new HashSet<Guid>(value); }
        
        public List<TagsMainModel> GetTags(TagServices services)
        {
            return Tags.Select(t => services.Get(t)).ToList();
        }
        
    }
}