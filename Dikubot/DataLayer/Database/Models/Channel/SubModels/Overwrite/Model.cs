using System;
using Discord;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models.Channel.SubModels
{
    public class OverwriteModel : Model
    {
        public OverwriteModel(Overwrite? overwrite = null)
        {
            if (overwrite != null)
                ToModel(overwrite.Value);
        }

        [BsonElement("Type")] public string Type { get; set; }
        [BsonElement("DiscordId")] public string DiscordId { get; set; }
        [BsonElement("Connect")] public string Connect { get; set; }
        [BsonElement("Speak")] public string Speak { get; set; }
        [BsonElement("AddReactions")] public string AddReactions { get; set; }
        [BsonElement("AttachFiles")] public string AttachFiles { get; set; }
        [BsonElement("DeafenMembers")] public string DeafenMembers { get; set; }
        [BsonElement("EmbedLinks")] public string EmbedLinks { get; set; }
        [BsonElement("ManageChannel")] public string ManageChannel { get; set; }
        [BsonElement("ManageMessages")] public string ManageMessages { get; set; }
        [BsonElement("ManageRoles")] public string ManageRoles { get; set; }
        [BsonElement("ManageWebhooks")] public string ManageWebhooks { get; set; }
        [BsonElement("MentionEveryone")] public string MentionEveryone { get; set; }
        [BsonElement("MoveMembers")] public string MoveMembers { get; set; }
        [BsonElement("MuteMembers")] public string MuteMembers { get; set; }
        [BsonElement("SendMessages")] public string SendMessages { get; set; }
        [BsonElement("ViewChannel")] public string ViewChannel { get; set; }
        [BsonElement("CreateInstantInvite")] public string CreateInstantInvite { get; set; }
        [BsonElement("ReadMessageHistory")] public string ReadMessageHistory { get; set; }
        [BsonElement("UseExternalEmojis")] public string UseExternalEmojis { get; set; }
        [BsonElement("UseVAD")] public string UseVAD { get; set; }
        [BsonElement("SendTTSMessages")] public string SendTTSMessages { get; set; }

        public Overwrite ToOverwrite()
        {
            PermValue StringToPermValue(string permValue)
            {
                if (permValue == "Inherit")
                    return PermValue.Inherit;
                if (permValue == "Deny")
                    return PermValue.Deny;
                if (permValue == "Allow")
                    return PermValue.Allow;
                throw new Exception($"Can't convert {permValue} to a PermValue");
            }

            var overwritePermissions = new OverwritePermissions(
                connect: StringToPermValue(Connect),
                speak: StringToPermValue(Speak),
                createInstantInvite: StringToPermValue(CreateInstantInvite),
                manageChannel: StringToPermValue(ManageChannel),
                addReactions: StringToPermValue(AddReactions),
                viewChannel: StringToPermValue(ViewChannel),
                sendTTSMessages: StringToPermValue(SendTTSMessages),
                manageMessages: StringToPermValue(ManageMessages),
                embedLinks: StringToPermValue(EmbedLinks),
                attachFiles: StringToPermValue(AttachFiles),
                sendMessages: StringToPermValue(SendMessages),
                readMessageHistory: StringToPermValue(ReadMessageHistory),
                mentionEveryone: StringToPermValue(MentionEveryone),
                useExternalEmojis: StringToPermValue(UseExternalEmojis),
                muteMembers: StringToPermValue(MuteMembers),
                deafenMembers: StringToPermValue(DeafenMembers),
                moveMembers: StringToPermValue(MoveMembers),
                useVoiceActivation: StringToPermValue(UseVAD),
                manageRoles: StringToPermValue(ManageRoles),
                manageWebhooks: StringToPermValue(ManageWebhooks)
            );

            PermissionTarget permissionTarget;

            if (Type == "Role")
                permissionTarget = PermissionTarget.Role;
            else if (Type == "User")
                permissionTarget = PermissionTarget.User;
            else
                throw new Exception($"Can't convert {Type} to a PermissionsTarget");

            var overwrite = new Overwrite(
                Convert.ToUInt64(DiscordId),
                permissionTarget,
                overwritePermissions
            );

            return overwrite;
        }

        public void ToModel(Overwrite overwrite)
        {
            Type = overwrite.TargetType.ToString();
            DiscordId = overwrite.TargetId.ToString();
            Connect = overwrite.Permissions.Connect.ToString();
            Speak = overwrite.Permissions.Speak.ToString();
            AddReactions = overwrite.Permissions.AddReactions.ToString();
            AttachFiles = overwrite.Permissions.AttachFiles.ToString();
            DeafenMembers = overwrite.Permissions.DeafenMembers.ToString();
            EmbedLinks = overwrite.Permissions.EmbedLinks.ToString();
            ManageChannel = overwrite.Permissions.ManageChannel.ToString();
            ManageMessages = overwrite.Permissions.ManageMessages.ToString();
            ManageRoles = overwrite.Permissions.ManageRoles.ToString();
            ManageWebhooks = overwrite.Permissions.ManageWebhooks.ToString();
            MentionEveryone = overwrite.Permissions.MentionEveryone.ToString();
            MoveMembers = overwrite.Permissions.MoveMembers.ToString();
            MuteMembers = overwrite.Permissions.MuteMembers.ToString();
            SendMessages = overwrite.Permissions.SendMessages.ToString();
            ViewChannel = overwrite.Permissions.ViewChannel.ToString();
            CreateInstantInvite = overwrite.Permissions.CreateInstantInvite.ToString();
            ReadMessageHistory = overwrite.Permissions.ReadMessageHistory.ToString();
            UseExternalEmojis = overwrite.Permissions.UseExternalEmojis.ToString();
            UseVAD = overwrite.Permissions.UseVAD.ToString();
            SendTTSMessages = overwrite.Permissions.SendTTSMessages.ToString();
        }
    }
}