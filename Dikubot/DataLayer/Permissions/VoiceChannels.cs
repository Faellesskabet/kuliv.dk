using System;
using System.Collections.Generic;
using System.Linq;
using Dikubot.Database.Models.VoiceChannel;
using Discord;
using Discord.WebSocket;

namespace Dikubot.Permissions
{
    public partial class PermissionsService
    {
        
        /// <Summary>Will sync all the voiceChannels on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void UploadVoiceChannels()
        {
            var voiceChannelModels = _voiceChannelServices.Get();
            var socketRoles = context.Guild.VoiceChannels.ToList();
            var toBeRemoved = new List<VoiceChannelModel>(voiceChannelModels);

            Func<VoiceChannelModel, SocketVoiceChannel, bool> inDB = (m0, m1) => 
                Convert.ToUInt64(m0.DiscordId) == m1.Id;
            
            // Remove all the roles from the database if they are not on the discord server.
            toBeRemoved.RemoveAll(m => 
                socketRoles.Exists(n => inDB(m, n)));

            // Remove the roles from the database that is not on the discord server.
            toBeRemoved.ForEach(m => _voiceChannelServices.Remove(m));
            
            // Makes an upsert of the roles on the server so they match the ones in the database.
            socketRoles.ForEach(model => 
                _voiceChannelServices.Upsert(_voiceChannelServices.SocketToModel(model)));
        }

        /// <Summary>Will add the overwrite permissions to the discords channel given.</Summary>
        /// <param name="voiceChannelId">The id of the voice channel.</param>
        /// <param name="voiceChannelModel">The voiceChannelModel retrieved from Database.</param>
        /// <return>void.</return>
        public async void AddOverwritePermissions(ulong voiceChannelId, VoiceChannelModel voiceChannelModel)
        {
            var socketChannel = context.Guild.GetVoiceChannel(voiceChannelId);
            foreach (var type in voiceChannelModel.PermissionsOverwrites.Keys)
            {
                foreach (var id in voiceChannelModel.PermissionsOverwrites[type].Keys)
                {
                    if (type == "Role")
                    {
                        var overwritePermissions =
                            _voiceChannelServices.ModelToOverwritePermissions(
                                voiceChannelModel,
                                "Role",
                                id);
                        await socketChannel.AddPermissionOverwriteAsync(
                            context.Guild.GetRole(Convert.ToUInt64(id)),
                            overwritePermissions);
                    }
                    else
                    {
                        var overwritePermissions =
                            _voiceChannelServices.ModelToOverwritePermissions(
                                voiceChannelModel,
                                "User",
                                id);
                        await socketChannel.AddPermissionOverwriteAsync(
                            context.Guild.GetUser(Convert.ToUInt64(id)),
                            overwritePermissions);
                    }
                }
            }
        }
        
        /// <Summary>Will sync all the voice channels on the database to the discord server.</Summary>
        /// <return>void.</return>
        public async void DownloadVoiceChannels()
        {
            var voiceChannelModels = _voiceChannelServices.Get();
            var socketVoiceChannels = context.Guild.VoiceChannels.ToList();
            var toBeRemoved = new List<SocketVoiceChannel>(socketVoiceChannels);

            Func<VoiceChannelModel, SocketVoiceChannel, bool> inDB = 
                (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id;
            
            // Remove all the roles from the discord server if they are not in the database.
            toBeRemoved.RemoveAll(m => voiceChannelModels.Exists(n => inDB(n, m)));

            foreach (var socketVoiceChannel in toBeRemoved)
                await socketVoiceChannel.DeleteAsync();
            
            foreach(var voiceChannelModel in voiceChannelModels)
            {

                // Finds the role discord server role that match the role in the database.
                var socketVoiceChannel = socketVoiceChannels.Find(socket => 
                    Convert.ToUInt64(voiceChannelModel.DiscordId) == socket.Id);
                
                if (socketVoiceChannel == null)
                {
                    // If the role could not be found create it.
                    var properties = _voiceChannelServices.ModelToVoiceChannelProperties(voiceChannelModel);
                    var temp = await context.Guild.CreateVoiceChannelAsync(
                        voiceChannelModel.Name,
                        channelProperties =>
                        {
                            channelProperties.Name = properties.Name;
                            channelProperties.Bitrate = properties.Bitrate;
                            channelProperties.UserLimit = properties.UserLimit;
                            channelProperties.Position = properties.Position;
                            channelProperties.CategoryId = properties.CategoryId;
                        });
                    
                    AddOverwritePermissions(temp.Id, voiceChannelModel);

                }
                else
                {
                    // If the role could be found modify it so it matches the database.
                    var properties = _voiceChannelServices.ModelToVoiceChannelProperties(voiceChannelModel);
                    await socketVoiceChannel.ModifyAsync(channelProperties =>
                    {
                        channelProperties.Name = properties.Name;
                        channelProperties.Bitrate = properties.Bitrate;
                        channelProperties.UserLimit = properties.UserLimit;
                        channelProperties.Position = properties.Position;
                        channelProperties.CategoryId = properties.CategoryId;
                    });

                    // Removes all the overwrite Permissions.
                    foreach (var overwrite in socketVoiceChannel.PermissionOverwrites)
                    {
                        if (overwrite.TargetType == PermissionTarget.Role)
                            await socketVoiceChannel.RemovePermissionOverwriteAsync(
                                context.Guild.GetRole(overwrite.TargetId));
                        else
                            await socketVoiceChannel.RemovePermissionOverwriteAsync(
                                context.Guild.GetUser(overwrite.TargetId));
                    }
                    
                    // Adds all the overwrite Permissions from the DB.
                    AddOverwritePermissions(socketVoiceChannel.Id, voiceChannelModel);
                }
            }
        }
    }
}