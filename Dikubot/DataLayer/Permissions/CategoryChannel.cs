using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild.Models.Channel.CategoryChannel;
using Dikubot.DataLayer.Database.Guild.Models.Channel.SubModels.Overwrite;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Permissions;

public partial class PermissionsService
{
    /// <Summary>Will sync all the categoryChannels on the discord server to the database.</Summary>
    /// <return>void.</return>
    public void SetDatabaseCategoryChannels()
    {
        List<CategoryChannelMainModel> categoryChannelModels = _categoryChannelMongoService.Get();
        List<SocketCategoryChannel> socketCategories = guild.CategoryChannels.ToList();
        HashSet<ulong> discordRoleIds = new(socketCategories.Select(role => role.Id));
        List<CategoryChannelMainModel> toBeRemoved = categoryChannelModels
            .Where(model => !discordRoleIds.Contains(ulong.Parse(model.DiscordId))).ToList();

        Func<CategoryChannelMainModel, SocketCategoryChannel, bool> inDB = (m0, m1) =>
            Convert.ToUInt64(m0.DiscordId) == m1.Id;

        // Remove all the category channels from the database if they are not on the discord server.
        toBeRemoved.RemoveAll(m =>
            socketCategories.Exists(n => inDB(m, n)));

        // Remove the category channels from the database that is not on the discord server.
        toBeRemoved.ForEach(m => _categoryChannelMongoService.Remove(m));

        // Makes an upsert of the category channels on the server so they match the ones in the database.
        socketCategories.ForEach(model =>
            _categoryChannelMongoService.Upsert(_categoryChannelMongoService.SocketToModel(model)));
    }

    /// <Summary>Will sync all the category channels on the database to the discord server.</Summary>
    /// <return>void.</return>
    public async void SetDiscordCategoryChannels()
    {
        List<CategoryChannelMainModel> categoryChannelModels = _categoryChannelMongoService.Get();
        List<SocketCategoryChannel> socketCategoryChannels = guild.CategoryChannels.ToList();
        List<SocketCategoryChannel> toBeRemoved = new(socketCategoryChannels);

        Func<CategoryChannelMainModel, SocketCategoryChannel, bool> inDB =
            (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id;

        // Remove all the category channels from the discord server if they are not in the database.
        toBeRemoved.RemoveAll(m => categoryChannelModels.Exists(n => inDB(n, m)));


        foreach (SocketCategoryChannel socketCategoryChannel in toBeRemoved)
            await socketCategoryChannel.DeleteAsync();

        foreach (CategoryChannelMainModel categoryChannelModel in categoryChannelModels)
        {
            // Finds the category channel discord server category channel that match the category channel in the database.
            SocketCategoryChannel socketCategoryChannel = socketCategoryChannels.Find(socket =>
                Convert.ToUInt64(categoryChannelModel.DiscordId) == socket.Id);

            if (socketCategoryChannel == null)
            {
                // If the category channel could not be found create it.
                GuildChannelProperties properties = _categoryChannelMongoService.ModelToCategoryChannelProperties(categoryChannelModel);
                RestCategoryChannel restCategoryChannel = await guild.CreateCategoryChannelAsync(
                    categoryChannelModel.Name,
                    channelProperties =>
                    {
                        channelProperties.CategoryId = properties.CategoryId;
                        channelProperties.Position = properties.Position;
                    });

                // Adds all the overwrite Permissions from the DB.
                foreach (OverwriteMainModel overwriteModel in categoryChannelModel.PermissionOverwrites)
                {
                    Overwrite overwrite = overwriteModel.ToOverwrite();
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
                GuildChannelProperties properties = _categoryChannelMongoService.ModelToCategoryChannelProperties(categoryChannelModel);
                await socketCategoryChannel.ModifyAsync(channelProperties =>
                {
                    channelProperties.Position = properties.Position;
                    channelProperties.CategoryId = properties.CategoryId;
                    channelProperties.Name = properties.Name;
                });

                // Adds all the overwrite Permissions from the DB.
                foreach (OverwriteMainModel overwriteModel in categoryChannelModel.PermissionOverwrites)
                {
                    Overwrite overwrite = overwriteModel.ToOverwrite();
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
    public void AddOrUpdateDatabaseCategoryChannel(SocketCategoryChannel categoryChannel)
    {
        _categoryChannelMongoService.Upsert(_categoryChannelMongoService.SocketToModel(categoryChannel));
    }


    /// <Summary>Add a category channel on the discord server to the database.</Summary>
    /// <return>void.</return>
    public void AddOrUpdateDatabaseCategoryChannel(RestCategoryChannel categoryChannel)
    {
        _categoryChannelMongoService.Upsert(_categoryChannelMongoService.RestToModel(categoryChannel));
    }


    /// <Summary>Add a category channel on the discord server to the database.</Summary>
    /// <return>void.</return>
    public void AddOrUpdateDatabaseCategoryChannel(CategoryChannelMainModel categoryChannelMain)
    {
        _categoryChannelMongoService.Remove(categoryChannelMain);
    }

    /// <Summary>Removes a category channel from the database.</Summary>
    /// <return>void.</return>
    public void RemoveDatabaseCategoryChannel(SocketCategoryChannel categoryChannel)
    {
        _categoryChannelMongoService.Remove(_categoryChannelMongoService.SocketToModel(categoryChannel));
    }

    /// <Summary>Removes a category channel from the database.</Summary>
    /// <return>void.</return>
    public void RemoveDatabaseCategoryChannel(RestCategoryChannel categoryChannel)
    {
        _categoryChannelMongoService.Remove(_categoryChannelMongoService.RestToModel(categoryChannel));
    }

    /// <Summary>Removes a category channel from the database.</Summary>
    /// <return>void.</return>
    public void RemoveDatabaseCategoryChannel(CategoryChannelMainModel categoryChannelMain)
    {
        _categoryChannelMongoService.Remove(categoryChannelMain);
    }

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

    /// <Summary>Removes a category channel from the database and the discord server.</Summary>
    /// <return>void.</return>
    public async Task RemoveCategoryChannel(CategoryChannelMainModel categoryChannelMain)
    {
        RemoveDatabaseCategoryChannel(categoryChannelMain);
        await guild.GetCategoryChannel(Convert.ToUInt64(categoryChannelMain)).DeleteAsync();
    }

    /// <Summary>Adds a category channel to the database and the discord server.</Summary>
    /// <return>void.</return>
    public async Task<CategoryChannelMainModel> AddCategoryChannel(CategoryChannelMainModel categoryChannelMain)
    {
        GuildChannelProperties properties = _categoryChannelMongoService.ModelToCategoryChannelProperties(categoryChannelMain);
        RestCategoryChannel restCategoryChannel = await guild.CreateCategoryChannelAsync(
            categoryChannelMain.Name,
            channelProperties =>
            {
                channelProperties.CategoryId = properties.CategoryId;
                channelProperties.Position = properties.Position;
            });
        categoryChannelMain.DiscordId = restCategoryChannel.Id.ToString();
        AddOrUpdateDatabaseCategoryChannel(categoryChannelMain);
        return categoryChannelMain;
    }
}