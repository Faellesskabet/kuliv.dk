using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.Database.Models.VoiceChannel;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace Dikubot.Permissions
{
    public partial class PermissionsService
    {
        
        /// <Summary>Will sync all the voiceChannels on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void SetDatabaseVoiceChannels()
        {
            var voiceChannelModels = _voiceChannelServices.Get();
            var socketRoles = guild.VoiceChannels.ToList();
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

        /// <Summary>Will sync all the voice channels on the database to the discord server.</Summary>
        /// <return>void.</return>
        public async void SetDiscordVoiceChannels()
        {
            var voiceChannelModels = _voiceChannelServices.Get();
            var socketVoiceChannels = guild.VoiceChannels.ToList();
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
                    var restVoiceChannel = await guild.CreateVoiceChannelAsync(
                        voiceChannelModel.Name,
                        channelProperties =>
                        {
                            channelProperties.Bitrate = properties.Bitrate;
                            channelProperties.UserLimit = properties.UserLimit;
                            channelProperties.Position = properties.Position;
                            channelProperties.CategoryId = properties.CategoryId;
                        });
                    
                    // Adds all the overwrite Permissions from the DB.
                    foreach (var overwriteModel in voiceChannelModel.PermissionOverwrites)
                    {
                        var overwrite = overwriteModel.ToOverwrite();
                        if (overwrite.TargetType == PermissionTarget.Role)
                            await restVoiceChannel.AddPermissionOverwriteAsync(
                                guild.GetRole(overwrite.TargetId),
                                overwrite.Permissions);
                        else
                            await restVoiceChannel.AddPermissionOverwriteAsync(
                                guild.GetUser(overwrite.TargetId),
                                overwrite.Permissions);
                    }
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
                                guild.GetRole(overwrite.TargetId));
                        else
                            await socketVoiceChannel.RemovePermissionOverwriteAsync(
                                guild.GetUser(overwrite.TargetId));
                    }
                    
                    // Adds all the overwrite Permissions from the DB.
                    foreach (var overwriteModel in voiceChannelModel.PermissionOverwrites)
                    {
                        var overwrite = overwriteModel.ToOverwrite();
                        if (overwrite.TargetType == PermissionTarget.Role)
                            await socketVoiceChannel.AddPermissionOverwriteAsync(
                                guild.GetRole(overwrite.TargetId),
                                overwrite.Permissions);
                        else
                            await socketVoiceChannel.AddPermissionOverwriteAsync(
                                guild.GetUser(overwrite.TargetId),
                                overwrite.Permissions);
                    }
                }
            }
        }
        /// <Summary>Add a voice channel on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void AddOrUpdateDatabaseVoiceChannel(SocketVoiceChannel voiceChannel) =>
            _voiceChannelServices.Upsert(_voiceChannelServices.SocketToModel(voiceChannel));
        
        /// <Summary>Add a voice channel on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void AddOrUpdateDatabaseVoiceChannel(RestVoiceChannel voiceChannel) =>
            _voiceChannelServices.Upsert(_voiceChannelServices.RestToModel(voiceChannel));
        
        /// <Summary>Add a voice channel on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void AddOrUpdateDatabaseVoiceChannel(VoiceChannelModel voiceChannel) =>
            _voiceChannelServices.Upsert(voiceChannel);
        
        /// <Summary>Removes a voice channel from the database.</Summary>
        /// <return>void.</return>
        public void RemoveDatabaseVoiceChannel(SocketVoiceChannel voiceChannel) =>
            _voiceChannelServices.Remove(_voiceChannelServices.SocketToModel(voiceChannel));
        
        /// <Summary>Removes a voice channel from the database.</Summary>
        /// <return>void.</return>
        public void RemoveDatabaseVoiceChannel(RestVoiceChannel voiceChannel) =>
            _voiceChannelServices.Remove(_voiceChannelServices.RestToModel(voiceChannel));
        
        /// <Summary>Removes a voice channel from the database.</Summary>
        /// <return>void.</return>
        public void RemoveDatabaseVoiceChannel(VoiceChannelModel voiceChannel) =>
            _voiceChannelServices.Remove(voiceChannel);
        
        /// <Summary>Removes a voice channel from the database and the discord server.</Summary>
        /// <return>void.</return>
        public async Task RemoveVoiceChannel(SocketVoiceChannel voiceChannel)
        {
            RemoveDatabaseVoiceChannel(voiceChannel);
            await voiceChannel.DeleteAsync();
        }
        /// <Summary>Removes a voice channel from the database and the discord server.</Summary>
        /// <return>void.</return>
        public async Task RemoveVoiceChannel(RestVoiceChannel voiceChannel)
        {
            RemoveDatabaseVoiceChannel(voiceChannel);
            await voiceChannel.DeleteAsync();
        }
        
        /// <Summary>Adds a voice channel to the database and the discord server.</Summary>
        /// <return>void.</return>
        public async Task<VoiceChannelModel> AddVoiceChannel(VoiceChannelModel voiceChannel)
        {
            var properties = _voiceChannelServices.ModelToVoiceChannelProperties(voiceChannel);
            var restVoiceChannel = await guild.CreateVoiceChannelAsync(
                voiceChannel.Name,
                channelProperties =>
                {
                    channelProperties.Bitrate = properties.Bitrate;
                    channelProperties.UserLimit = properties.UserLimit;
                    channelProperties.Position = properties.Position;
                    channelProperties.CategoryId = properties.CategoryId;
                });
            voiceChannel.DiscordId = restVoiceChannel.Id.ToString();
            AddOrUpdateDatabaseVoiceChannel(voiceChannel);
            return voiceChannel;
        }
    }
}