using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Role.SubModels.GuildPermissions
{
    public class GuildPermissionsModel
    {
        public GuildPermissionsModel(global::Discord.GuildPermissions? guildPermissions = null)
        {
            ToModel(guildPermissions);
        }

        [BsonElement("Administrator")] public bool Administrator { get; set; }
        [BsonElement("Connect")] public bool Connect { get; set; }
        [BsonElement("Speak")] public bool Speak { get; set; }
        [BsonElement("Stream")] public bool Stream { get; set; }
        [BsonElement("AddReactions")] public bool AddReactions { get; set; }
        [BsonElement("AttachFiles")] public bool AttachFiles { get; set; }
        [BsonElement("BanMembers")] public bool BanMembers { get; set; }
        [BsonElement("ChangeNickname")] public bool ChangeNickname { get; set; }
        [BsonElement("DeafenMembers")] public bool DeafenMembers { get; set; }
        [BsonElement("EmbedLinks")] public bool EmbedLinks { get; set; }
        [BsonElement("KickMembers")] public bool KickMembers { get; set; }
        [BsonElement("ManageChannels")] public bool ManageChannels { get; set; }
        [BsonElement("ManageEmojis")] public bool ManageEmojis { get; set; }
        [BsonElement("ManageGuild")] public bool ManageGuild { get; set; }
        [BsonElement("ManageMessages")] public bool ManageMessages { get; set; }
        [BsonElement("ManageNicknames")] public bool ManageNicknames { get; set; }
        [BsonElement("ManageRoles")] public bool ManageRoles { get; set; }
        [BsonElement("ManageWebhooks")] public bool ManageWebhooks { get; set; }
        [BsonElement("MentionEveryone")] public bool MentionEveryone { get; set; }
        [BsonElement("MoveMembers")] public bool MoveMembers { get; set; }
        [BsonElement("MuteMembers")] public bool MuteMembers { get; set; }
        [BsonElement("PrioritySpeaker")] public bool PrioritySpeaker { get; set; }
        [BsonElement("SendMessages")] public bool SendMessages { get; set; }
        [BsonElement("ViewChannel")] public bool ViewChannel { get; set; }
        [BsonElement("CreateInstantInvite")] public bool CreateInstantInvite { get; set; }
        [BsonElement("ReadMessageHistory")] public bool ReadMessageHistory { get; set; }
        [BsonElement("UseExternalEmojis")] public bool UseExternalEmojis { get; set; }
        [BsonElement("ViewAuditLog")] public bool ViewAuditLog { get; set; }
        [BsonElement("UseVAD")] public bool UseVAD { get; set; }
        [BsonElement("SendTTSMessages")] public bool SendTTSMessages { get; set; }

        public global::Discord.GuildPermissions ToGuildPermission()
        {
            var guildPermissions = new global::Discord.GuildPermissions(
                createInstantInvite: CreateInstantInvite,
                kickMembers: KickMembers,
                banMembers: BanMembers,
                administrator: Administrator,
                manageChannels: ManageChannels,
                manageGuild: ManageGuild,
                addReactions: AddReactions,
                viewAuditLog: ViewAuditLog,
                viewChannel: ViewChannel,
                sendMessages: SendMessages,
                sendTTSMessages: SendTTSMessages,
                manageMessages: ManageMessages,
                embedLinks: EmbedLinks,
                attachFiles: AttachFiles,
                readMessageHistory: ReadMessageHistory,
                mentionEveryone: MentionEveryone,
                useExternalEmojis: UseExternalEmojis,
                connect: Connect,
                speak: Speak,
                muteMembers: MuteMembers,
                deafenMembers: DeafenMembers,
                moveMembers: MoveMembers,
                useVoiceActivation: UseVAD,
                prioritySpeaker: PrioritySpeaker,
                stream: Stream,
                changeNickname: ChangeNickname,
                manageNicknames: ManageNicknames,
                manageRoles: ManageRoles,
                manageWebhooks: ManageWebhooks,
                manageEmojisAndStickers: ManageEmojis);
            return guildPermissions;
        }

        public void ToModel(global::Discord.GuildPermissions? guildPermissions)
        {
            if (guildPermissions == null)
                return;

            Administrator = guildPermissions.Value.Administrator;
            Connect = guildPermissions.Value.Connect;
            Speak = guildPermissions.Value.Speak;
            Stream = guildPermissions.Value.Stream;
            AddReactions = guildPermissions.Value.AddReactions;
            AttachFiles = guildPermissions.Value.AttachFiles;
            BanMembers = guildPermissions.Value.BanMembers;
            ChangeNickname = guildPermissions.Value.ChangeNickname;
            DeafenMembers = guildPermissions.Value.DeafenMembers;
            EmbedLinks = guildPermissions.Value.EmbedLinks;
            KickMembers = guildPermissions.Value.KickMembers;
            ManageChannels = guildPermissions.Value.ManageChannels;
            ManageEmojis = guildPermissions.Value.ManageEmojisAndStickers;
            ManageGuild = guildPermissions.Value.ManageGuild;
            ManageMessages = guildPermissions.Value.ManageMessages;
            ManageNicknames = guildPermissions.Value.ManageNicknames;
            ManageRoles = guildPermissions.Value.ManageRoles;
            ManageWebhooks = guildPermissions.Value.ManageWebhooks;
            MentionEveryone = guildPermissions.Value.MentionEveryone;
            MoveMembers = guildPermissions.Value.MoveMembers;
            MuteMembers = guildPermissions.Value.MuteMembers;
            PrioritySpeaker = guildPermissions.Value.PrioritySpeaker;
            SendMessages = guildPermissions.Value.SendMessages;
            ViewChannel = guildPermissions.Value.ViewChannel;
            CreateInstantInvite = guildPermissions.Value.CreateInstantInvite;
            ReadMessageHistory = guildPermissions.Value.ReadMessageHistory;
            UseExternalEmojis = guildPermissions.Value.UseExternalEmojis;
            ViewAuditLog = guildPermissions.Value.ViewAuditLog;
            UseVAD = guildPermissions.Value.UseVAD;
            SendTTSMessages = guildPermissions.Value.SendTTSMessages;
        }
        
        
    }
}