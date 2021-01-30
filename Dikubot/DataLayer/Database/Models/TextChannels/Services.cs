using System;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Sockets;
using Dikubot.Database.Models.TextChannel;
using Dikubot.Discord;
using Discord;
using MongoDB.Driver;

namespace Dikubot.Database.Models.TextChannel
{
    /// <summary>
    /// Class for for retrieving information from the TextChannel collection.
    /// </summary>
    public class TextChannelServices : Services<TextChannelModel>
    {
        public TextChannelServices() : base("Main", "TextChannels") { }
        
        /// <Summary>Inserts a TextChannelModel in the collection. If a TextChannelModel with the same ID or discordID
        /// already exists, then we imply invoke Update() on the model instead.</Summary>
        /// <param name="modelIn">The Model one wishes to be inserted.</param>
        /// <return>A Model.</return>
        public new TextChannelModel Upsert(TextChannelModel modelIn)
        {
            bool idCollision = Exists(model => model.Id == modelIn.Id);
            bool discordIdCollision = Exists(model => model.DiscordId == modelIn.DiscordId);

            if (idCollision)
            {
                Update(modelIn);
                return modelIn;
            }
            if (discordIdCollision)
            {
                Update(m => m.DiscordId == modelIn.DiscordId, modelIn);
                return modelIn;
            }
            Insert(modelIn);
            return modelIn;
        }

        /// <Summary>Removes a element from the collection by it's unique elements.</Summary>
        /// <param name="modelIn">The Model one wishes to remove.</param>
        /// <return>Void.</return>
        public new void Remove(TextChannelModel modelIn)
        {
            Remove(model => model.Id == modelIn.Id);
            Remove(model => model.DiscordId == modelIn.DiscordId);
        }
        
        /// <Summary>Converts a SocketTextChannel to a TextChannelModel.</Summary>
        /// <param name="textChannel">The SocketTextChannel to be converted.</param>
        /// <return>Returns a TextChannelModel.</return>
        public TextChannelModel SocketToModel(SocketTextChannel textChannel)
        {
            var _textChannel = new TextChannelModel();
            _textChannel.DiscordId = textChannel.Id.ToString();
            _textChannel.Position = textChannel.Position;
            _textChannel.CreatedAt = textChannel.CreatedAt.DateTime;
            _textChannel.Name = textChannel.Name;
            _textChannel.DiscordCategoryId = textChannel.CategoryId.ToString();
            _textChannel.Topic = textChannel.Topic;
            _textChannel.IsNsfw = textChannel.IsNsfw;
            _textChannel.SlowModeInterval = textChannel.SlowModeInterval;
            _textChannel.PermissionsOverwrites = 
                    new Dictionary<string, Dictionary<string, Dictionary<string, string?>>>();
            foreach (var overwrite in textChannel.PermissionOverwrites)
            {
                var id = overwrite.TargetId.ToString();
                string type;
                if (overwrite.TargetType == PermissionTarget.Role)
                    type = "Role";
                else
                    type = "User";

                if (!_textChannel.PermissionsOverwrites.ContainsKey(type))
                {
                    _textChannel.PermissionsOverwrites[type] = new Dictionary<string, Dictionary<string, string?>>();
                }

                if (!_textChannel.PermissionsOverwrites[type].ContainsKey(id))
                {
                    _textChannel.PermissionsOverwrites[type][id] = new Dictionary<string, string?>();
                }
                _textChannel.PermissionsOverwrites[type][id]["Connect"] = 
                        overwrite.Permissions.Connect.ToString();
                _textChannel.PermissionsOverwrites[type][id]["Speak"] = 
                        overwrite.Permissions.Speak.ToString();
                _textChannel.PermissionsOverwrites[type][id]["AddReactions"] = 
                        overwrite.Permissions.AddReactions.ToString();
                _textChannel.PermissionsOverwrites[type][id]["AttachFiles"] = 
                        overwrite.Permissions.AttachFiles.ToString();
                _textChannel.PermissionsOverwrites[type][id]["DeafenMembers"] = 
                        overwrite.Permissions.DeafenMembers.ToString();
                _textChannel.PermissionsOverwrites[type][id]["EmbedLinks"] = 
                        overwrite.Permissions.EmbedLinks.ToString();
                _textChannel.PermissionsOverwrites[type][id]["ManageChannel"] = 
                        overwrite.Permissions.ManageChannel.ToString();
                _textChannel.PermissionsOverwrites[type][id]["ManageMessages"] = 
                        overwrite.Permissions.ManageMessages.ToString();
                _textChannel.PermissionsOverwrites[type][id]["ManageRoles"] = 
                        overwrite.Permissions.ManageRoles.ToString();
                _textChannel.PermissionsOverwrites[type][id]["ManageWebhooks"] = 
                        overwrite.Permissions.ManageWebhooks.ToString();
                _textChannel.PermissionsOverwrites[type][id]["MentionEveryone"] = 
                        overwrite.Permissions.MentionEveryone.ToString();
                _textChannel.PermissionsOverwrites[type][id]["MoveMembers"] = 
                        overwrite.Permissions.MoveMembers.ToString();
                _textChannel.PermissionsOverwrites[type][id]["MuteMembers"] = 
                        overwrite.Permissions.MuteMembers.ToString();
                _textChannel.PermissionsOverwrites[type][id]["SendMessages"] = 
                        overwrite.Permissions.SendMessages.ToString();
                _textChannel.PermissionsOverwrites[type][id]["ViewChannel"] = 
                        overwrite.Permissions.ViewChannel.ToString();
                _textChannel.PermissionsOverwrites[type][id]["CreateInstantInvite"] = 
                        overwrite.Permissions.CreateInstantInvite.ToString();
                _textChannel.PermissionsOverwrites[type][id]["ReadMessageHistory"] = 
                        overwrite.Permissions.ReadMessageHistory.ToString();
                _textChannel.PermissionsOverwrites[type][id]["UseExternalEmojis"] = 
                        overwrite.Permissions.UseExternalEmojis.ToString();
                _textChannel.PermissionsOverwrites[type][id]["UseVAD"] = 
                        overwrite.Permissions.UseVAD.ToString();
                _textChannel.PermissionsOverwrites[type][id]["SendTTSMessages"] = 
                        overwrite.Permissions.SendTTSMessages.ToString();
            }
            return _textChannel;
        }

        /// <Summary>Converts a TextChannelModel to a TextChannelProperties.</Summary>
        /// <param name="textChannelModel">The TextChannelModel to be converted.</param>
        /// <return>Returns a TextChannelProperties.</return>
        public TextChannelProperties ModelToTextChannelProperties(TextChannelModel textChannelModel)
        {
                var textChannelProperties = new TextChannelProperties();
                textChannelProperties.Name = textChannelModel.Name;
                textChannelProperties.Position = textChannelModel.Position;
                textChannelProperties.CategoryId = Convert.ToUInt64(textChannelModel.DiscordCategoryId);
                textChannelProperties.Topic = textChannelModel.Topic;
                textChannelProperties.IsNsfw = textChannelModel.IsNsfw;
                textChannelProperties.SlowModeInterval = textChannelModel.SlowModeInterval;
                return textChannelProperties;
        }
        
        /// <Summary>Converts the TextChannelModel property OverwritePermissions to a OverwritePermissions.</Summary>
        /// <param name="textChannelModel">The TextChannelModel to be converted.</param>
        /// <param name="type">The type so either role or user.</param>
        /// <param name="id">The id of the role or user.</param>
        /// <return>Returns a OverwritePermissions.</return>
        public OverwritePermissions ModelToOverwritePermissions(TextChannelModel textChannelModel, 
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
                                textChannelModel.PermissionsOverwrites[type][id]["Connect"]),
                        speak: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["Speak"]),
                        createInstantInvite: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["CreateInstantInvite"]),
                        manageChannel: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["ManageChannel"]),
                        addReactions: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["AddReactions"]),
                        viewChannel: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["ViewChannel"]),
                        sendTTSMessages: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["SendTTSMessages"]),
                        manageMessages: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["ManageMessages"]),
                        embedLinks: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["EmbedLinks"]),
                        attachFiles: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["AttachFiles"]),
                        sendMessages: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["SendMessages"]),
                        readMessageHistory: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["ReadMessageHistory"]),
                        mentionEveryone: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["MentionEveryone"]),
                        useExternalEmojis: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["UseExternalEmojis"]),
                        muteMembers: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["MuteMembers"]),
                        deafenMembers: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["DeafenMembers"]),
                        moveMembers: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["MoveMembers"]),
                        useVoiceActivation: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["UseVAD"]),
                        manageRoles: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["ManageRoles"]),
                        manageWebhooks: StringToPermValue(
                                textChannelModel.PermissionsOverwrites[type][id]["ManageWebhooks"]));
                
                return overwritePermissions;
        }
    }
}

