using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.Database.Models.VoiceChannel;
using Discord.WebSocket;
using Discord;
using Discord.Rest;

namespace Dikubot.Permissions
{
    public partial class PermissionsService
    {
        /// <Summary>Will sync all the categoryChannels on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void SetDatabaseCategoryChannels()
        {
            var categoryChannelModels = _categoryChannelServices.Get();
            var socketCategories = guild.CategoryChannels.ToList();
            var toBeRemoved = new List<CategoryChannelModel>(categoryChannelModels);

            Func<CategoryChannelModel, SocketCategoryChannel, bool> inDB = (m0, m1) => 
                Convert.ToUInt64(m0.DiscordId) == m1.Id;
            
            // Remove all the category channels from the database if they are not on the discord server.
            toBeRemoved.RemoveAll(m => 
                socketCategories.Exists(n => inDB(m, n)));

            // Remove the category channels from the database that is not on the discord server.
            toBeRemoved.ForEach(m => _categoryChannelServices.Remove(m));
            
            // Makes an upsert of the category channels on the server so they match the ones in the database.
            socketCategories.ForEach(model => 
                _categoryChannelServices.Upsert(_categoryChannelServices.SocketToModel(model)));
        }
        
        /// <Summary>Will sync all the category channels on the database to the discord server.</Summary>
        /// <return>void.</return>
        public async void SetDiscordCategoryChannels()
        {
            var categoryChannelModels = _categoryChannelServices.Get();
            var socketCategoryChannels = guild.CategoryChannels.ToList();
            var toBeRemoved = new List<SocketCategoryChannel>(socketCategoryChannels);

            Func<CategoryChannelModel, SocketCategoryChannel, bool> inDB = 
                (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id;
            
            // Remove all the category channels from the discord server if they are not in the database.
            toBeRemoved.RemoveAll(m => categoryChannelModels.Exists(n => inDB(n, m)));
            
            
            foreach (var socketCategoryChannel in toBeRemoved)
                await socketCategoryChannel.DeleteAsync();
            
            foreach(var categoryChannelModel in categoryChannelModels)
            {
                // Finds the category channel discord server category channel that match the category channel in the database.
                var socketCategoryChannel = socketCategoryChannels.Find(socket => 
                    Convert.ToUInt64(categoryChannelModel.DiscordId) == socket.Id);
                
                if (socketCategoryChannel == null)
                {
                    // If the category channel could not be found create it.
                    var properties = _categoryChannelServices.ModelToCategoryChannelProperties(categoryChannelModel);
                    var restCategoryChannel = await guild.CreateCategoryChannelAsync(
                        categoryChannelModel.Name,
                        channelProperties =>
                        {
                            channelProperties.CategoryId = properties.CategoryId;
                            channelProperties.Position = properties.Position;
                        });
                    
                    // Adds all the overwrite Permissions from the DB.
                    foreach (var overwriteModel in categoryChannelModel.PermissionOverwrites)
                    {
                        var overwrite = overwriteModel.ToOverwrite();
                        if (overwrite.TargetType == PermissionTarget.Role)
                            await restCategoryChannel.AddPermissionOverwriteAsync(
                                guild.GetRole(overwrite.TargetId),
                                overwrite.Permissions);
                        else
                            await restCategoryChannel.AddPermissionOverwriteAsync(
                                guild.GetUser(overwrite.TargetId),
                                overwrite.Permissions);
                    }
                }
                else
                {
                    // If the category channel could be found modify it so it matches the database.
                    var properties = _categoryChannelServices.ModelToCategoryChannelProperties(categoryChannelModel);
                    await socketCategoryChannel.ModifyAsync(channelProperties =>
                    {
                        channelProperties.Position = properties.Position;
                        channelProperties.CategoryId = properties.CategoryId;
                        channelProperties.Name = properties.Name;
                    });
                    
                    // Adds all the overwrite Permissions from the DB.
                    foreach (var overwriteModel in categoryChannelModel.PermissionOverwrites)
                    {
                        var overwrite = overwriteModel.ToOverwrite();
                        if (overwrite.TargetType == PermissionTarget.Role)
                            await socketCategoryChannel.AddPermissionOverwriteAsync(
                                guild.GetRole(overwrite.TargetId),
                                overwrite.Permissions);
                        else
                            await socketCategoryChannel.AddPermissionOverwriteAsync(
                                guild.GetUser(overwrite.TargetId),
                                overwrite.Permissions);
                    }
                }
            }
        }
        
        /// <Summary>Add a category channel on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void AddOrUpdateDatabaseCategoryChannel(SocketCategoryChannel categoryChannel) =>
            _categoryChannelServices.Upsert(_categoryChannelServices.SocketToModel(categoryChannel));
        
        
        /// <Summary>Add a category channel on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void AddOrUpdateDatabaseCategoryChannel(RestCategoryChannel categoryChannel) =>
            _categoryChannelServices.Upsert(_categoryChannelServices.RestToModel(categoryChannel));
        
        
        /// <Summary>Add a category channel on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void AddOrUpdateDatabaseCategoryChannel(CategoryChannelModel categoryChannel) =>
            _categoryChannelServices.Remove(categoryChannel);
        
        /// <Summary>Removes a category channel from the database.</Summary>
        /// <return>void.</return>
        public void RemoveDatabaseCategoryChannel(SocketCategoryChannel categoryChannel) =>
            _categoryChannelServices.Remove(_categoryChannelServices.SocketToModel(categoryChannel));

        /// <Summary>Removes a category channel from the database.</Summary>
        /// <return>void.</return>
        public void RemoveDatabaseCategoryChannel(RestCategoryChannel categoryChannel) =>
            _categoryChannelServices.Remove(_categoryChannelServices.RestToModel(categoryChannel));
        
        /// <Summary>Removes a category channel from the database.</Summary>
        /// <return>void.</return>
        public void RemoveDatabaseCategoryChannel(CategoryChannelModel categoryChannel) =>
            _categoryChannelServices.Remove(categoryChannel);

        /// <Summary>Removes a category channel from the database and the discord server.</Summary>
        /// <return>void.</return>
        public async Task RemoveCategoryChannel(SocketCategoryChannel categoryChannel)
        {
            RemoveDatabaseCategoryChannel(categoryChannel);
            await categoryChannel.DeleteAsync();
        }
        /// <Summary>Removes a category channel from the database and the discord server.</Summary>
        /// <return>void.</return>
        public async Task RemoveCategoryChannel(RestCategoryChannel categoryChannel)
        {
            RemoveDatabaseCategoryChannel(categoryChannel);
            await categoryChannel.DeleteAsync();
        }
        
        /// <Summary>Adds a category channel to the database and the discord server.</Summary>
        /// <return>void.</return>
        public async Task<CategoryChannelModel> AddCategoryChannel(CategoryChannelModel categoryChannel)
        {
            var properties = _categoryChannelServices.ModelToCategoryChannelProperties(categoryChannel);
            var restCategoryChannel = await guild.CreateCategoryChannelAsync(
                categoryChannel.Name,
                channelProperties =>
                {
                    channelProperties.CategoryId = properties.CategoryId;
                    channelProperties.Position = properties.Position;
                });
            categoryChannel.DiscordId = restCategoryChannel.Id.ToString();
            AddOrUpdateDatabaseCategoryChannel(categoryChannel);
            return categoryChannel;
        }
    }
}