using System;
using System.Collections.Generic;
using System.Linq;
using Dikubot.Database.Models.TextChannel;
using Discord;
using Discord.WebSocket;

namespace Dikubot.Permissions
{
    public partial class PermissionsService
    {
        
        /// <Summary>Will sync all the textChannels on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void UploadTextChannels()
        {
            var textChannelModels = _textChannelServices.Get();
            var socketRoles = context.Guild.TextChannels.ToList();
            var toBeRemoved = new List<TextChannelModel>(textChannelModels);

            Func<TextChannelModel, SocketTextChannel, bool> inDB = (m0, m1) => 
                Convert.ToUInt64(m0.DiscordId) == m1.Id;
            
            // Remove all the roles from the database if they are not on the discord server.
            toBeRemoved.RemoveAll(m => 
                socketRoles.Exists(n => inDB(m, n)));

            // Remove the roles from the database that is not on the discord server.
            toBeRemoved.ForEach(m => _textChannelServices.Remove(m));
            
            // Makes an upsert of the roles on the server so they match the ones in the database.
            socketRoles.ForEach(model => 
                _textChannelServices.Upsert(_textChannelServices.SocketToModel(model)));
        }

        /// <Summary>Will add the overwrite permissions to the discords channel given.</Summary>
        /// <param name="textChannelId">The id of the text channel.</param>
        /// <param name="textChannelModel">The textChannelModel retrieved from Database.</param>
        /// <return>void.</return>
        public async void AddOverwritePermissions(ulong textChannelId, TextChannelModel textChannelModel)
        {
            var socketChannel = context.Guild.GetTextChannel(textChannelId);
            foreach (var type in textChannelModel.PermissionsOverwrites.Keys)
            {
                foreach (var id in textChannelModel.PermissionsOverwrites[type].Keys)
                {
                    if (type == "Role")
                    {
                        var overwritePermissions =
                            _textChannelServices.ModelToOverwritePermissions(
                                textChannelModel,
                                "Role",
                                id);
                        await socketChannel.AddPermissionOverwriteAsync(
                            context.Guild.GetRole(Convert.ToUInt64(id)),
                            overwritePermissions);
                    }
                    else
                    {
                        var overwritePermissions =
                            _textChannelServices.ModelToOverwritePermissions(
                                textChannelModel,
                                "User",
                                id);
                        await socketChannel.AddPermissionOverwriteAsync(
                            context.Guild.GetUser(Convert.ToUInt64(id)),
                            overwritePermissions);
                    }
                }
            }
        }
        
        /// <Summary>Will sync all the text channels on the database to the discord server.</Summary>
        /// <return>void.</return>
        public async void DownloadTextChannels()
        {
            var textChannelModels = _textChannelServices.Get();
            var socketTextChannels = context.Guild.TextChannels.ToList();
            var toBeRemoved = new List<SocketTextChannel>(socketTextChannels);

            Func<TextChannelModel, SocketTextChannel, bool> inDB = 
                (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id;
            
            // Remove all the roles from the discord server if they are not in the database.
            toBeRemoved.RemoveAll(m => textChannelModels.Exists(n => inDB(n, m)));

            foreach (var socketTextChannel in toBeRemoved)
                await socketTextChannel.DeleteAsync();
            
            foreach(var textChannelModel in textChannelModels)
            {

                // Finds the role discord server role that match the role in the database.
                var socketTextChannel = socketTextChannels.Find(socket => 
                    Convert.ToUInt64(textChannelModel.DiscordId) == socket.Id);
                
                if (socketTextChannel == null)
                {
                    // If the role could not be found create it.
                    var properties = _textChannelServices.ModelToTextChannelProperties(textChannelModel);
                    var temp = await context.Guild.CreateTextChannelAsync(
                        textChannelModel.Name,
                        channelProperties =>
                        {
                            channelProperties.Position = properties.Position;
                            channelProperties.CategoryId = properties.CategoryId;
                            channelProperties.Topic = properties.Topic;
                            channelProperties.IsNsfw = properties.IsNsfw;
                            channelProperties.SlowModeInterval = properties.SlowModeInterval;
                            channelProperties.Name = properties.Name;
                        });
                    
                    AddOverwritePermissions(temp.Id, textChannelModel);

                }
                else
                {
                    // If the role could be found modify it so it matches the database.
                    var properties = _textChannelServices.ModelToTextChannelProperties(textChannelModel);
                    await socketTextChannel.ModifyAsync(channelProperties =>
                    {
                        channelProperties.Position = properties.Position;
                        channelProperties.CategoryId = properties.CategoryId;
                        channelProperties.Topic = properties.Topic;
                        channelProperties.IsNsfw = properties.IsNsfw;
                        channelProperties.SlowModeInterval = properties.SlowModeInterval;
                        channelProperties.Name = properties.Name;
                    });

                    // Removes all the overwrite Permissions.
                    foreach (var overwrite in socketTextChannel.PermissionOverwrites)
                    {
                        if (overwrite.TargetType == PermissionTarget.Role)
                            await socketTextChannel.RemovePermissionOverwriteAsync(
                                context.Guild.GetRole(overwrite.TargetId));
                        else
                            await socketTextChannel.RemovePermissionOverwriteAsync(
                                context.Guild.GetUser(overwrite.TargetId));
                    }
                    
                    // Adds all the overwrite Permissions from the DB.
                    AddOverwritePermissions(socketTextChannel.Id, textChannelModel);
                }
            }
        }
    }
}