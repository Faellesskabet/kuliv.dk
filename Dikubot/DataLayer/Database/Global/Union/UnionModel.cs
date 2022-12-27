using System;
using System.Collections.Generic;
using System.Linq;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Global.Tags;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.Union;

public class UnionModel : MainModel
{
    public UnionModel()
    {
    }

    public UnionModel(GuildSettingsModel settingsModel)
    {
        Id = settingsModel.Id;
        Title = settingsModel.Name;
        Decs = settingsModel.Description;
        Discord = settingsModel.GuildId.ToString();
        TagsEnumerable = settingsModel.TagsEnumerable;
        LogoUrl = settingsModel.LogoUrl;
        Facebook = settingsModel.FacebookUrl;
        Href = settingsModel.Webpage;
        BannerUrl = settingsModel.BannerUrl;
    }

    [BsonElement("Title")] public string Title { get; set; }

    [BsonElement("Decs")] public string Decs { get; set; }


    [BsonElement("LogoUrl")] public string LogoUrl { get; set; }

    [BsonElement("BannerUrl")] public string BannerUrl { get; set; }

    [BsonElement("Href")] public string Href { get; set; }

    [BsonElement("Facebook")] public string Facebook { get; set; }

    [BsonElement("Mail")] public string Mail { get; set; }

    [BsonElement("Discord")] public string Discord { get; set; }


    /// <summary>
    ///     The tags associated with the guild. They're used for searching and filtering
    /// </summary>
    [BsonElement("Tags")]
    public HashSet<Guid> Tags { get; set; } = new();

    [BsonIgnore]
    public IEnumerable<Guid> TagsEnumerable
    {
        get => Tags;
        set => Tags = new HashSet<Guid>(value);
    }

    public override List<string> GetSearchContent()
    {
        return new List<string>
        {
            Title, Decs
        };
    }

    public override HashSet<Guid> GetTags()
    {
        return Tags;
    }

    public List<TagsMainModel> GetTags(TagMongoService mongoService)
    {
        return Tags.Select(t => mongoService.Get(t)).ToList();
    }
}