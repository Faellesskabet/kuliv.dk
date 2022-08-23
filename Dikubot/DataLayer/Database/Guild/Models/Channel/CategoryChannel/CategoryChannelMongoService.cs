using System;
using System.Collections.Generic;
using Dikubot.DataLayer.Database.Guild.Models.Channel.SubModels.Overwrite;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild.Models.Channel.CategoryChannel
{
    /// <summary>
    /// Class for for retrieving information from the CategoryChannel collection.
    /// </summary>
    public class CategoryChannelMongoService : ChannelMongoService<CategoryChannelMainModel>
    {
        public CategoryChannelMongoService(SocketGuild guild) : base("CategoryChannels", guild)
        {
        }

        /// <Summary>Converts a SocketCategoryChannel to a CategoryChannelModel.</Summary>
        /// <param name="categoryChannel">The SocketCategoryChannel to be converted.</param>
        /// <return>Returns a CategoryChannelModel.</return>
        public CategoryChannelMainModel SocketToModel(SocketCategoryChannel categoryChannel)
        {
            var _categoryChannel = new CategoryChannelMainModel();
            _categoryChannel.DiscordId = categoryChannel.Id.ToString();
            _categoryChannel.Position = categoryChannel.Position;
            _categoryChannel.CreatedAt = categoryChannel.CreatedAt.DateTime;
            _categoryChannel.Name = categoryChannel.Name;
            _categoryChannel.PermissionOverwrites = new List<OverwriteMainModel>();
            foreach (var overwrite in categoryChannel.PermissionOverwrites)
            {
                _categoryChannel.PermissionOverwrites.Add(new OverwriteMainModel(overwrite));
            }

            return _categoryChannel;
        }

        /// <Summary>Converts a RestCategoryChannel to a CategoryChannelModel.</Summary>
        /// <param name="categoryChannel">The RestCategoryChannel to be converted.</param>
        /// <return>Returns a CategoryChannelModel.</return>
        public CategoryChannelMainModel RestToModel(RestCategoryChannel categoryChannel)
        {
            var _categoryChannel = new CategoryChannelMainModel();
            _categoryChannel.DiscordId = categoryChannel.Id.ToString();
            _categoryChannel.Position = categoryChannel.Position;
            _categoryChannel.CreatedAt = categoryChannel.CreatedAt.DateTime;
            _categoryChannel.Name = categoryChannel.Name;
            _categoryChannel.PermissionOverwrites = new List<OverwriteMainModel>();
            foreach (var overwrite in categoryChannel.PermissionOverwrites)
            {
                _categoryChannel.PermissionOverwrites.Add(new OverwriteMainModel(overwrite));
            }

            return _categoryChannel;
        }

        /// <Summary>Converts a CategoryChannelModel to GuildChannelProperties.</Summary>
        /// <param name="categoryChannelMainModel">The CategoryChannelModel to be converted.</param>
        /// <return>Returns a GuildChannelProperties.</return>
        public GuildChannelProperties ModelToCategoryChannelProperties(CategoryChannelMainModel categoryChannelMainModel)
        {
            var categoryChannelProperties = new GuildChannelProperties();
            categoryChannelProperties.Name = categoryChannelMainModel.Name;
            categoryChannelProperties.Position = categoryChannelMainModel.Position;
            categoryChannelProperties.CategoryId = Convert.ToUInt64(categoryChannelMainModel.DiscordId);
            return categoryChannelProperties;
        }
    }
}