using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dikubot.DataLayer.Database.Global.Settings.Tags;
using Discord.WebSocket;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.GuildSettings;

public class GuildSettingsModel : MainModel
{
    public GuildSettingsModel() {}

    public GuildSettingsModel(SocketGuild guild)
    {
        GuildId = guild.Id;
        Name = guild.Name;
        LogoUrl = guild.IconUrl;
        BannerUrl = guild.BannerUrl;
        Description = guild.Description;
    }
    
    [BsonElement("GuildId")] [BsonUnique]
    public ulong GuildId { get; set; }
    
    /// <summary>
    /// Can the guild be seen by everyone
    /// If false then only members of the guild can see it
    /// </summary>
    [BsonElement("IsPublic")]
    public bool IsPublic { get; set; }
    
    /// <summary>
    /// The display name of the guild. Does not have to be the same as the guild name on Discord
    /// </summary>
    [BsonElement("Name")] [StringLength(32)]
    public string Name { get; set; }
    
    /// <summary>
    /// The logo of the guild. Does not have to be the same as the guild logo on Discord.
    /// </summary>
    [BsonElement("LogoUrl")] [StringLength(2048)]
    public string LogoUrl { get; set; }
    
    /// <summary>
    /// The banner of the guild. Does not have to be the same as the guild banner on Discord.
    /// </summary>
    [BsonElement("BannerUrl")] [StringLength(2048)]
    public string BannerUrl { get; set; }
    
    /// <summary>
    /// The description of the guild. Does not have to be the same as the guild description on Discord.
    /// </summary>
    [BsonElement("Description")] [StringLength(4096)]
    public string Description { get; set; }
    
    /// <summary>
    /// This will force people to use their real name, if we are able to find one.
    /// If this is toggled from false to true, then ALL names are updated.
    /// If it is toggled from true to false, then NO names are updated, they must be updated manually, as we do not save the past names of users.
    /// </summary>
    [BsonElement("ForceNameChange")]
    public bool ForceNameChange { get; set; }
    
    /// <summary>
    /// Users will only be able to join your guild through the website if they're verified.
    /// </summary>
    [BsonElement("ForceVerified")]
    public bool ForceVerified { get; set; }
    
    /// <summary>
    /// All verified users on a server will get this role. Verified people won't get a special role if the role can't be found or is non-existent.
    /// </summary>
    [BsonElement("VerifiedRole")]
    public ulong VerifiedRole { get; set; }
    
    /// <summary>
    /// Whether or not the welcome message is enabled. If it is enabled, then users receive a the WelcomeMessage upon
    /// joining the guild. If the WelcomeMessageEnabled is true but WelcomeMessage is empty, then no message is sent regardless.
    /// </summary>
    [BsonElement("WelcomeMessageEnabled")]
    public bool WelcomeMessageEnabled { get; set; }
    
    /// <summary>
    /// The Welcome message that is sent to new users.
    /// </summary>
    [BsonElement("WelcomeMessage")] [StringLength(4096)]
    public string WelcomeMessage { get; set; }
    
    /// <summary>
    /// The tags associated with the guild. They're used for searching and filtering
    /// </summary>
    [BsonElement("Tags")]
    public List<TagsMainModel> Tags { get; set; }
    
    /// <summary>
    /// Indicates whether the guild accept support tickets.
    /// </summary>
    public bool SupportTicketsEnabled { get; set; }
    
    [BsonElement("AnnouncementChannels")]
    public List<ulong> AnnouncementChannels { get; set; }
}