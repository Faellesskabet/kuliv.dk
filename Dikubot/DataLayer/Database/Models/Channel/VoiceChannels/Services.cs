using System;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using Dikubot.Discord;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.Database.Models.VoiceChannel
{
    /// <summary>
    /// Class for for retrieving information from the VoiceChannel collection.
    /// </summary>
    public class VoiceChannelServices : Services<VoiceChannelModel>
    {
        public VoiceChannelServices() : base("Main", "VoiceChannels") { }

        /// <Summary>Inserts a TextChannelModel in the collection. If a VoiceChannelModel with the same ID or discordID
        /// already exists, then we imply invoke Update() on the model instead.</Summary>
        /// <param name="modelIn">The Model one wishes to be inserted.</param>
        /// <return>A Model.</return>
        public new VoiceChannelModel Upsert(VoiceChannelModel modelIn)
        {
            bool idCollision = Exists(model => model.Id == modelIn.Id);
            bool discordIdCollision = Exists(model => model.DiscordId == modelIn.DiscordId);

            if (idCollision)
            {
                Update(modelIn, new ReplaceOptions() {IsUpsert = true});
                return modelIn;
            }
            if (discordIdCollision)
            {
                Update(m => m.DiscordId == modelIn.DiscordId, modelIn, new ReplaceOptions() {IsUpsert = true});
                return modelIn;
            }
            Insert(modelIn);
            return modelIn;
        }

        /// <Summary>Removes a VoiceChannelModel from the collection by it's unique elements.</Summary>
        /// <param name="modelIn">The Model one wishes to remove.</param>
        /// <return>Void.</return>
        public new void Remove(VoiceChannelModel modelIn)
        {
            Remove(model => model.Id == modelIn.Id);
            Remove(model => model.DiscordId == modelIn.DiscordId);
        }
        
        /// <Summary>Converts a SocketVoiceChannel to a VoiceChannelModel.</Summary>
        /// <param name="voiceChannel">The SocketVoiceChannel to be converted.</param>
        /// <return>Returns a VoiceChannelModel.</return>
        public VoiceChannelModel SocketToModel(SocketVoiceChannel voiceChannel)
        {
            var _voiceChannel = new VoiceChannelModel();
            _voiceChannel.DiscordId = voiceChannel.Id.ToString();
            _voiceChannel.Bitrate = voiceChannel.Bitrate;
            _voiceChannel.Position = voiceChannel.Position;
            _voiceChannel.CreatedAt = voiceChannel.CreatedAt.DateTime;
            _voiceChannel.Name = voiceChannel.Name;
            _voiceChannel.UserLimit = voiceChannel.UserLimit;
            _voiceChannel.DiscordCategoryId = voiceChannel.CategoryId.ToString();
            _voiceChannel.PermissionsOverwrites = 
                    new Dictionary<string, Dictionary<string, Dictionary<string, string?>>>();
            foreach (var overwrite in voiceChannel.PermissionOverwrites)
            {
                var id = overwrite.TargetId.ToString();
                string type;
                if (overwrite.TargetType == PermissionTarget.Role)
                    type = "Role";
                else
                    type = "User";

                if (!_voiceChannel.PermissionsOverwrites.ContainsKey(type))
                {
                    _voiceChannel.PermissionsOverwrites[type] = new Dictionary<string, Dictionary<string, string?>>();
                }

                if (!_voiceChannel.PermissionsOverwrites[type].ContainsKey(id))
                {
                    _voiceChannel.PermissionsOverwrites[type][id] = new Dictionary<string, string?>();
                }
                _voiceChannel.PermissionsOverwrites[type][id]["Connect"] = 
                        overwrite.Permissions.Connect.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["Speak"] = 
                        overwrite.Permissions.Speak.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["AddReactions"] = 
                        overwrite.Permissions.AddReactions.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["AttachFiles"] = 
                        overwrite.Permissions.AttachFiles.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["DeafenMembers"] = 
                        overwrite.Permissions.DeafenMembers.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["EmbedLinks"] = 
                        overwrite.Permissions.EmbedLinks.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["ManageChannel"] = 
                        overwrite.Permissions.ManageChannel.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["ManageMessages"] = 
                        overwrite.Permissions.ManageMessages.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["ManageRoles"] = 
                        overwrite.Permissions.ManageRoles.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["ManageWebhooks"] = 
                        overwrite.Permissions.ManageWebhooks.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["MentionEveryone"] = 
                        overwrite.Permissions.MentionEveryone.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["MoveMembers"] = 
                        overwrite.Permissions.MoveMembers.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["MuteMembers"] = 
                        overwrite.Permissions.MuteMembers.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["SendMessages"] = 
                        overwrite.Permissions.SendMessages.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["ViewChannel"] = 
                        overwrite.Permissions.ViewChannel.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["CreateInstantInvite"] = 
                        overwrite.Permissions.CreateInstantInvite.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["ReadMessageHistory"] = 
                        overwrite.Permissions.ReadMessageHistory.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["UseExternalEmojis"] = 
                        overwrite.Permissions.UseExternalEmojis.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["UseVAD"] = 
                        overwrite.Permissions.UseVAD.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["SendTTSMessages"] = 
                        overwrite.Permissions.SendTTSMessages.ToString();
            }
            return _voiceChannel;
        }
        
        /// <Summary>Converts a RestVoiceChannel to a VoiceChannelModel.</Summary>
        /// <param name="voiceChannel">The RestVoiceChannel to be converted.</param>
        /// <return>Returns a VoiceChannelModel.</return>
        public VoiceChannelModel RestToModel(RestVoiceChannel voiceChannel)
        {
            var _voiceChannel = new VoiceChannelModel();
            _voiceChannel.DiscordId = voiceChannel.Id.ToString();
            _voiceChannel.Bitrate = voiceChannel.Bitrate;
            _voiceChannel.Position = voiceChannel.Position;
            _voiceChannel.CreatedAt = voiceChannel.CreatedAt.DateTime;
            _voiceChannel.Name = voiceChannel.Name;
            _voiceChannel.UserLimit = voiceChannel.UserLimit;
            _voiceChannel.DiscordCategoryId = voiceChannel.CategoryId.ToString();
            _voiceChannel.PermissionsOverwrites = 
                    new Dictionary<string, Dictionary<string, Dictionary<string, string?>>>();
            foreach (var overwrite in voiceChannel.PermissionOverwrites)
            {
                var id = overwrite.TargetId.ToString();
                string type;
                if (overwrite.TargetType == PermissionTarget.Role)
                    type = "Role";
                else
                    type = "User";

                if (!_voiceChannel.PermissionsOverwrites.ContainsKey(type))
                {
                    _voiceChannel.PermissionsOverwrites[type] = new Dictionary<string, Dictionary<string, string?>>();
                }

                if (!_voiceChannel.PermissionsOverwrites[type].ContainsKey(id))
                {
                    _voiceChannel.PermissionsOverwrites[type][id] = new Dictionary<string, string?>();
                }
                _voiceChannel.PermissionsOverwrites[type][id]["Connect"] = 
                        overwrite.Permissions.Connect.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["Speak"] = 
                        overwrite.Permissions.Speak.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["AddReactions"] = 
                        overwrite.Permissions.AddReactions.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["AttachFiles"] = 
                        overwrite.Permissions.AttachFiles.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["DeafenMembers"] = 
                        overwrite.Permissions.DeafenMembers.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["EmbedLinks"] = 
                        overwrite.Permissions.EmbedLinks.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["ManageChannel"] = 
                        overwrite.Permissions.ManageChannel.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["ManageMessages"] = 
                        overwrite.Permissions.ManageMessages.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["ManageRoles"] = 
                        overwrite.Permissions.ManageRoles.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["ManageWebhooks"] = 
                        overwrite.Permissions.ManageWebhooks.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["MentionEveryone"] = 
                        overwrite.Permissions.MentionEveryone.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["MoveMembers"] = 
                        overwrite.Permissions.MoveMembers.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["MuteMembers"] = 
                        overwrite.Permissions.MuteMembers.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["SendMessages"] = 
                        overwrite.Permissions.SendMessages.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["ViewChannel"] = 
                        overwrite.Permissions.ViewChannel.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["CreateInstantInvite"] = 
                        overwrite.Permissions.CreateInstantInvite.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["ReadMessageHistory"] = 
                        overwrite.Permissions.ReadMessageHistory.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["UseExternalEmojis"] = 
                        overwrite.Permissions.UseExternalEmojis.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["UseVAD"] = 
                        overwrite.Permissions.UseVAD.ToString();
                _voiceChannel.PermissionsOverwrites[type][id]["SendTTSMessages"] = 
                        overwrite.Permissions.SendTTSMessages.ToString();
            }
            return _voiceChannel;
        }

        /// <Summary>Converts a VoiceChannelModel to a VoiceChannelProperties.</Summary>
        /// <param name="voiceChannelModel">The VoiceChannelModel to be converted.</param>
        /// <return>Returns a VoiceChannelProperties.</return>
        public VoiceChannelProperties ModelToVoiceChannelProperties(VoiceChannelModel voiceChannelModel)
        {
                var voiceChannelProperties = new VoiceChannelProperties();
                voiceChannelProperties.Bitrate = voiceChannelModel.Bitrate;
                voiceChannelProperties.UserLimit = voiceChannelModel.UserLimit;
                voiceChannelProperties.Name = voiceChannelModel.Name;
                voiceChannelProperties.Position = voiceChannelModel.Position;
                voiceChannelProperties.CategoryId = Convert.ToUInt64(voiceChannelModel.DiscordCategoryId);
                return voiceChannelProperties;
        }
        
        /// <Summary>Converts the VoiceChannelModel property OverwritePermissions to a OverwritePermissions.</Summary>
        /// <param name="voiceChannelModel">The VoiceChannelModel to be converted.</param>
        /// <param name="type">The type so either role or user.</param>
        /// <param name="id">The id of the role or user.</param>
        /// <return>Returns a OverwritePermissions.</return>
        public OverwritePermissions ModelToOverwritePermissions(VoiceChannelModel voiceChannelModel, 
                string type, 
                string id)
        {
                PermValue StringToPermValue(string permValue)
                {
                        if (permValue == "Inherit")
                        {
                                return PermValue.Inherit;
                        }

                        if (permValue == "Deny")
                        {
                                return PermValue.Deny;
                        }

                        if (permValue == "Allow")
                        {
                                return PermValue.Allow;
                        }

                        throw new Exception($"Can't convert {permValue} to a PermValue");
                }

                // Converts the dictionary to overwritePermissions
                var overwritePermissions = new OverwritePermissions(
                        connect: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["Connect"]),
                        speak: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["Speak"]),
                        createInstantInvite: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["CreateInstantInvite"]),
                        manageChannel: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["ManageChannel"]),
                        addReactions: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["AddReactions"]),
                        viewChannel: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["ViewChannel"]),
                        sendTTSMessages: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["SendTTSMessages"]),
                        manageMessages: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["ManageMessages"]),
                        embedLinks: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["EmbedLinks"]),
                        attachFiles: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["AttachFiles"]),
                        sendMessages: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["SendMessages"]),
                        readMessageHistory: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["ReadMessageHistory"]),
                        mentionEveryone: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["MentionEveryone"]),
                        useExternalEmojis: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["UseExternalEmojis"]),
                        muteMembers: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["MuteMembers"]),
                        deafenMembers: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["DeafenMembers"]),
                        moveMembers: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["MoveMembers"]),
                        useVoiceActivation: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["UseVAD"]),
                        manageRoles: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["ManageRoles"]),
                        manageWebhooks: StringToPermValue(
                                voiceChannelModel.PermissionsOverwrites[type][id]["ManageWebhooks"]));
                
                return overwritePermissions;
        }
    }
}


