using System;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using Dikubot.Database.Models.Channel;
using Dikubot.Database.Models.Channel.SubModels;
using Dikubot.Database.Models.VoiceChannel;
using Discord;
using Discord.Rest;

namespace Dikubot.Database.Models.CategoryChannel
{
    /// <summary>
    /// Class for for retrieving information from the CategoryChannel collection.
    /// </summary>
    public class CategoryChannelServices : ChannelServices<CategoryChannelModel>
    {
        public CategoryChannelServices(SocketGuild guild) : base("Main", "CategoryChannels", guild)
        {
        }

        /// <Summary>Converts a SocketCategoryChannel to a CategoryChannelModel.</Summary>
        /// <param name="categoryChannel">The SocketCategoryChannel to be converted.</param>
        /// <return>Returns a CategoryChannelModel.</return>
        public CategoryChannelModel SocketToModel(SocketCategoryChannel categoryChannel)
        {
            var _categoryChannel = new CategoryChannelModel();
            _categoryChannel.DiscordId = categoryChannel.Id.ToString();
            _categoryChannel.Position = categoryChannel.Position;
            _categoryChannel.CreatedAt = categoryChannel.CreatedAt.DateTime;
            _categoryChannel.Name = categoryChannel.Name;
            _categoryChannel.PermissionOverwrites = new List<OverwriteModel>();
            foreach (var overwrite in categoryChannel.PermissionOverwrites)
            {
                _categoryChannel.PermissionOverwrites.Add(new OverwriteModel(overwrite));
            }

            return _categoryChannel;
        }

        /// <Summary>Converts a RestCategoryChannel to a CategoryChannelModel.</Summary>
        /// <param name="categoryChannel">The RestCategoryChannel to be converted.</param>
        /// <return>Returns a CategoryChannelModel.</return>
        public CategoryChannelModel RestToModel(RestCategoryChannel categoryChannel)
        {
            var _categoryChannel = new CategoryChannelModel();
            _categoryChannel.DiscordId = categoryChannel.Id.ToString();
            _categoryChannel.Position = categoryChannel.Position;
            _categoryChannel.CreatedAt = categoryChannel.CreatedAt.DateTime;
            _categoryChannel.Name = categoryChannel.Name;
            _categoryChannel.PermissionOverwrites = new List<OverwriteModel>();
            foreach (var overwrite in categoryChannel.PermissionOverwrites)
            {
                _categoryChannel.PermissionOverwrites.Add(new OverwriteModel(overwrite));
            }

            return _categoryChannel;
        }

        /// <Summary>Converts a CategoryChannelModel to GuildChannelProperties.</Summary>
        /// <param name="categoryChannelModel">The CategoryChannelModel to be converted.</param>
        /// <return>Returns a GuildChannelProperties.</return>
        public GuildChannelProperties ModelToCategoryChannelProperties(CategoryChannelModel categoryChannelModel)
        {
            var categoryChannelProperties = new GuildChannelProperties();
            categoryChannelProperties.Name = categoryChannelModel.Name;
            categoryChannelProperties.Position = categoryChannelModel.Position;
            categoryChannelProperties.CategoryId = Convert.ToUInt64(categoryChannelModel.DiscordId);
            return categoryChannelProperties;
        }
    }
}