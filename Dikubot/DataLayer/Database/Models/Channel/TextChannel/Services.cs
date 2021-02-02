using System;
using Discord.WebSocket;
using System.Collections.Generic;
using Dikubot.Database.Models.Channel;
using Dikubot.Database.Models.Channel.SubModels;
using Discord;
using Discord.Rest;

namespace Dikubot.Database.Models.TextChannel
{
    /// <summary>
    /// Class for for retrieving information from the TextChannel collection.
    /// </summary>
    public class TextChannelServices : ChannelServices<TextChannelModel>
    {
        public TextChannelServices() : base("Main", "TextChannels") { }
        
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
            _textChannel.PermissionOverwrites = new List<OverwriteModel>();
            foreach (var overwrite in textChannel.PermissionOverwrites)
            {
                _textChannel.PermissionOverwrites.Add(new OverwriteModel(overwrite));
            }
            return _textChannel;
        }
        
        /// <Summary>Converts a RestTextChannel to a TextChannelModel.</Summary>
        /// <param name="textChannel">The RestTextChannel to be converted.</param>
        /// <return>Returns a TextChannelModel.</return>
        public TextChannelModel RestToModel(RestTextChannel textChannel)
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
            _textChannel.PermissionOverwrites = new List<OverwriteModel>();
            foreach (var overwrite in textChannel.PermissionOverwrites)
            {
                _textChannel.PermissionOverwrites.Add(new OverwriteModel(overwrite));
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
    }
}

