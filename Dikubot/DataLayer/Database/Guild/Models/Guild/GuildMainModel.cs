using System;
using System.Collections.Generic;
using System.Globalization;
using Dikubot.DataLayer.Database.Guild.Models.Guild.Submodels.GuildOptions;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Guild
{
    
    
    public class GuildMainModel : MainModel 
    {
        //Options going here
        [BsonElement("Options")] public GuildOptions Options { get; set; }
        
        //MAIN THING
        private IGuild _guildImplementation;
        
        
        public GuildMainModel(SocketGuild guildImplementation)
        {
            _guildImplementation = guildImplementation;
            DiscordId = _guildImplementation.Id;
            CreatedAt = _guildImplementation.CreatedAt;
            Name =  (_guildImplementation.Name);
            AFKTimeout = _guildImplementation.AFKTimeout;
            //IsEmbeddable = _guildImplementation.IsEmbeddable;
            IsWidgetEnabled = _guildImplementation.IsWidgetEnabled;
            DefaultMessageNotifications = _guildImplementation.DefaultMessageNotifications;
            MfaLevel = _guildImplementation.MfaLevel;
            Description = _guildImplementation.Description;
            VerificationLevel = _guildImplementation.VerificationLevel;
            ExplicitContentFilter = _guildImplementation.ExplicitContentFilter;
            IconId = _guildImplementation.IconId;
            IconUrl = _guildImplementation.IconUrl;
            SplashId = _guildImplementation.SplashId;
            SplashUrl = _guildImplementation.SplashUrl;
            DiscoverySplashId = _guildImplementation.DiscoverySplashId;
            DiscoverySplashUrl = _guildImplementation.DiscoverySplashUrl;
            Available = _guildImplementation.Available;
            AFKChannelId = _guildImplementation.AFKChannelId;
            DefaultChannelId = _guildImplementation.GetDefaultChannelAsync().Result.Id;
            //EmbedChannelId = _guildImplementation.EmbedChannelId;
            WidgetChannelId = _guildImplementation.WidgetChannelId;
            SystemChannelId = _guildImplementation.SystemChannelId;
            RulesChannelId = _guildImplementation.RulesChannelId;
            PublicUpdatesChannelId = _guildImplementation.PublicUpdatesChannelId;
            OwnerId = _guildImplementation.OwnerId;
            ApplicationId = _guildImplementation.ApplicationId;
            VoiceRegionId = _guildImplementation.VoiceRegionId;
            BannerId = _guildImplementation.BannerId;
            BannerUrl = _guildImplementation.BannerUrl;
            VanityURLCode = _guildImplementation.VanityURLCode;
            PremiumSubscriptionCount = _guildImplementation.PremiumSubscriptionCount;
            MaxPresences = _guildImplementation.MaxPresences;
            MaxMembers = _guildImplementation.MaxMembers;
            MaxVideoChannelUsers = _guildImplementation.MaxVideoChannelUsers;
            ApproximateMemberCount = _guildImplementation.ApproximateMemberCount;
            ApproximatePresenceCount = _guildImplementation.ApproximatePresenceCount;
            PreferredLocale = _guildImplementation.PreferredLocale;
        }




        [BsonElement("DiscordId")]
        public ulong DiscordId
        {
            get;
            set;
        }
        [BsonElement("CreatedAt")]
        public DateTimeOffset CreatedAt
        {
            get;
            set;
        }

        [BsonElement("Name")]
        public string Name
        {
            get;
            set;
        }
        
        [BsonElement("AFKTimeout")]
        public int AFKTimeout
        {
            get;
            set;
        }

        [BsonElement("IsEmbeddable")]
        public bool IsEmbeddable
        {
            get;
            set;
        }

        [BsonElement("IsWidgetEnabled")]
        public bool IsWidgetEnabled
        {
            get;
            set;
        }

        [BsonElement("DefaultMessageNotifications")]
        public DefaultMessageNotifications DefaultMessageNotifications
        {
            get;
            set;
        }

        [BsonElement("MfaLevel")]
        public MfaLevel MfaLevel { get; set; }

        [BsonElement("VerificationLevel")]
        public VerificationLevel VerificationLevel { get; set; }

        [BsonElement("ExplicitContentFilter")]
        public ExplicitContentFilterLevel ExplicitContentFilter
        {
            get;
            set;
        }

        [BsonElement("IconId")]
        public string IconId
        {
            get;
            set;
        }

        [BsonElement("IconUrl")]
        public string IconUrl
        {
            get;
            set;
        }

        [BsonElement("SplashId")]
        public string SplashId
        {
            get;
            set;
        }

        [BsonElement("SplashUrl")]
        public string SplashUrl
        {
            get;
            set;
        }

        [BsonElement("DiscoverySplashId")]
        public string DiscoverySplashId
        {
            get;
            set;
        }

        [BsonElement("DiscoverySplashUrl")]
        public string DiscoverySplashUrl
        {
            get;
            set;
        }

        [BsonElement("Available")]
        public bool Available
        {
            get;
            set;
        }

        [BsonElement("AFKChannelId")]
        public ulong? AFKChannelId
        {
            get;
            set;
        }

        [BsonElement("DefaultChannelId")]
        public ulong DefaultChannelId
        {
            get;
            set;
        }

        [BsonElement("EmbedChannelId")]
        public ulong? EmbedChannelId
        {
            get;
            set;
        }

        [BsonElement("WidgetChannelId")]
        public ulong? WidgetChannelId
        {
            get;
            set;
        }

        [BsonElement("SystemChannelId")]
        public ulong? SystemChannelId
        {
            get;
            set;
        }

        [BsonElement("RulesChannelId")]
        public ulong? RulesChannelId
        {
            get;
            set;
        }

        [BsonElement("PublicUpdatesChannelId")]
        public ulong? PublicUpdatesChannelId
        {
            get;
            set;
        }

        [BsonElement("OwnerId")]
        public ulong OwnerId
        {
            get;
            set;
        }

        [BsonElement("ApplicationId")]
        public ulong? ApplicationId
        {
            get;
            set;
        }

        [BsonElement("VoiceRegionId")]
        public string VoiceRegionId
        {
            get;
            set;
        }

        
        public IAudioClient AudioClient
        {
            get { return _guildImplementation.AudioClient; }
            //set { }
        }

        public IRole EveryoneRole
        {
            get { return _guildImplementation.EveryoneRole; }
            //set { }
        }

        public IReadOnlyCollection<GuildEmote> Emotes
        {
            get { return (IReadOnlyCollection<GuildEmote>) _guildImplementation.Emotes; }
            //set { }
        }

        public IReadOnlyCollection<string> Features
        {
            get { return  (IReadOnlyCollection<string>) _guildImplementation.Features; }
            //set { }
        }

        public IReadOnlyCollection<IRole> Roles
        {
            get { return (IReadOnlyCollection<IRole>) _guildImplementation.Roles; }
            //set { }
        }

        
        public PremiumTier PremiumTier
        {
            get { return _guildImplementation.PremiumTier; }
            //set { }
        }

        [BsonElement("BannerId")]
        public string BannerId
        {
            get;
            set;
        }

        [BsonElement("BannerUrl")]
        public string BannerUrl
        {
            get;
            set;
        }

        [BsonElement("VanityURLCode")]
        public string VanityURLCode
        {
            get;
            set;
        }
        
        public SystemChannelMessageDeny SystemChannelFlags
        {
            get { return _guildImplementation.SystemChannelFlags; }
            //set { }
        }

        [BsonElement("Description")]
        public string Description
        {
            get;
            set;
        }

        [BsonElement("PremiumSubscriptionCount")]
        public int PremiumSubscriptionCount
        {
            get;
            set;
        }
        
        [BsonElement("MaxPresences")]
        public int? MaxPresences
        {
            get;
            set;
        }

        [BsonElement("MaxMembers")]
        public int? MaxMembers
        {
            get;
            set;
        }

        [BsonElement("MaxVideoChannelUsers")]
        public int? MaxVideoChannelUsers
        {
            get;
            set;
        }

        [BsonElement("ApproximateMemberCount")]
        public int? ApproximateMemberCount
        {
            get;
            set;
        }

        [BsonElement("ApproximatePresenceCount")]
        public int? ApproximatePresenceCount
        {
            get;
            set;
        }

        [BsonElement("PreferredLocale")]
        public string PreferredLocale
        {
            get;
            set;
        }

        
        public CultureInfo PreferredCulture
        {
            get { return _guildImplementation.PreferredCulture; }
            //set { }
        }
    }

    

    
}
