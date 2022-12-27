using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild.Models.Channel.SubModels.Overwrite;
using Dikubot.DataLayer.Database.Guild.Models.Channel.VoiceChannel;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Permissions;

public partial class PermissionsService
{
    /// <Summary>Will sync all the voiceChannels on the discord server to the database.</Summary>
    /// <return>void.</return>
    public void SetDatabaseVoiceChannels()
    {
        List<VoiceChannelMainModel> voiceChannelModels = _voiceChannelMongoService.Get();
        List<SocketVoiceChannel> socketRoles = guild.VoiceChannels.ToList();
        HashSet<ulong> discordRoleIds = new(socketRoles.Select(role => role.Id));
        List<VoiceChannelMainModel> toBeRemoved = voiceChannelModels
            .Where(model => !discordRoleIds.Contains(ulong.Parse(model.DiscordId))).ToList();

        Func<VoiceChannelMainModel, SocketVoiceChannel, bool> inDB = (m0, m1) =>
            Convert.ToUInt64(m0.DiscordId) == m1.Id;

        // Remove all the roles from the database if they are not on the discord server.
        toBeRemoved.RemoveAll(m =>
            socketRoles.Exists(n => inDB(m, n)));

        // Remove the roles from the database that is not on the discord server.
        toBeRemoved.ForEach(m => _voiceChannelMongoService.Remove(m));

        // Makes an upsert of the roles on the server so they match the ones in the database.
        socketRoles.ForEach(model =>
            _voiceChannelMongoService.Upsert(_voiceChannelMongoService.SocketToModel(model)));
    }

    /// <Summary>Will sync all the voice channels on the database to the discord server.</Summary>
    /// <return>void.</return>
    public async void SetDiscordVoiceChannels()
    {
        List<VoiceChannelMainModel> voiceChannelModels = _voiceChannelMongoService.Get();
        List<SocketVoiceChannel> socketVoiceChannels = guild.VoiceChannels.ToList();
        List<SocketVoiceChannel> toBeRemoved = new(socketVoiceChannels);

        Func<VoiceChannelMainModel, SocketVoiceChannel, bool> inDB =
            (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id;

        // Remove all the roles from the discord server if they are not in the database.
        toBeRemoved.RemoveAll(m => voiceChannelModels.Exists(n => inDB(n, m)));

        foreach (SocketVoiceChannel socketVoiceChannel in toBeRemoved)
            await socketVoiceChannel.DeleteAsync();

        foreach (VoiceChannelMainModel voiceChannelModel in voiceChannelModels)
        {
            // Finds the role discord server role that match the role in the database.
            SocketVoiceChannel socketVoiceChannel = socketVoiceChannels.Find(socket =>
                Convert.ToUInt64(voiceChannelModel.DiscordId) == socket.Id);

            if (socketVoiceChannel == null)
            {
                // If the role could not be found create it.
                VoiceChannelProperties properties = _voiceChannelMongoService.ModelToVoiceChannelProperties(voiceChannelModel);
                RestVoiceChannel restVoiceChannel = await guild.CreateVoiceChannelAsync(
                    voiceChannelModel.Name,
                    channelProperties =>
                    {
                        channelProperties.Bitrate = properties.Bitrate;
                        channelProperties.UserLimit = properties.UserLimit;
                        channelProperties.Position = properties.Position;
                        channelProperties.CategoryId = properties.CategoryId;
                    });

                // Adds all the overwrite Permissions from the DB.
                foreach (OverwriteMainModel overwriteModel in voiceChannelModel.PermissionOverwrites)
                {
                    Overwrite overwrite = overwriteModel.ToOverwrite();
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
                VoiceChannelProperties properties = _voiceChannelMongoService.ModelToVoiceChannelProperties(voiceChannelModel);
                await socketVoiceChannel.ModifyAsync(channelProperties =>
                {
                    channelProperties.Name = properties.Name;
                    channelProperties.Bitrate = properties.Bitrate;
                    channelProperties.UserLimit = properties.UserLimit;
                    channelProperties.Position = properties.Position;
                    channelProperties.CategoryId = properties.CategoryId;
                });

                // Removes all the overwrite Permissions.
                foreach (Overwrite overwrite in socketVoiceChannel.PermissionOverwrites)
                    if (overwrite.TargetType == PermissionTarget.Role)
                        await socketVoiceChannel.RemovePermissionOverwriteAsync(
                            guild.GetRole(overwrite.TargetId));
                    else
                        await socketVoiceChannel.RemovePermissionOverwriteAsync(
                            guild.GetUser(overwrite.TargetId));

                // Adds all the overwrite Permissions from the DB.
                foreach (OverwriteMainModel overwriteModel in voiceChannelModel.PermissionOverwrites)
                {
                    Overwrite overwrite = overwriteModel.ToOverwrite();
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
    public void AddOrUpdateDatabaseVoiceChannel(SocketVoiceChannel voiceChannel)
    {
        _voiceChannelMongoService.Upsert(_voiceChannelMongoService.SocketToModel(voiceChannel));
    }

    /// <Summary>Add a voice channel on the discord server to the database.</Summary>
    /// <return>void.</return>
    public void AddOrUpdateDatabaseVoiceChannel(RestVoiceChannel voiceChannel)
    {
        _voiceChannelMongoService.Upsert(_voiceChannelMongoService.RestToModel(voiceChannel));
    }

    /// <Summary>Add a voice channel on the discord server to the database.</Summary>
    /// <return>void.</return>
    public void AddOrUpdateDatabaseVoiceChannel(VoiceChannelMainModel voiceChannelMain)
    {
        _voiceChannelMongoService.Upsert(voiceChannelMain);
    }

    /// <Summary>Removes a voice channel from the database.</Summary>
    /// <return>void.</return>
    public void RemoveDatabaseVoiceChannel(SocketVoiceChannel voiceChannel)
    {
        _voiceChannelMongoService.Remove(_voiceChannelMongoService.SocketToModel(voiceChannel));
    }

    /// <Summary>Removes a voice channel from the database.</Summary>
    /// <return>void.</return>
    public void RemoveDatabaseVoiceChannel(RestVoiceChannel voiceChannel)
    {
        _voiceChannelMongoService.Remove(_voiceChannelMongoService.RestToModel(voiceChannel));
    }

    /// <Summary>Removes a voice channel from the database.</Summary>
    /// <return>void.</return>
    public void RemoveDatabaseVoiceChannel(VoiceChannelMainModel voiceChannelMain)
    {
        _voiceChannelMongoService.Remove(voiceChannelMain);
    }

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

    /// <Summary>Removes a voice channel from the database and the discord server.</Summary>
    /// <return>void.</return>
    public async Task RemoveVoiceChannel(VoiceChannelMainModel voiceChannelMain)
    {
        RemoveDatabaseVoiceChannel(voiceChannelMain);
        await guild.GetVoiceChannel(Convert.ToUInt64(voiceChannelMain)).DeleteAsync();
    }

    /// <Summary>Adds a voice channel to the database and the discord server.</Summary>
    /// <return>void.</return>
    public async Task<VoiceChannelMainModel> AddVoiceChannel(VoiceChannelMainModel voiceChannelMain)
    {
        VoiceChannelProperties properties = _voiceChannelMongoService.ModelToVoiceChannelProperties(voiceChannelMain);
        RestVoiceChannel restVoiceChannel = await guild.CreateVoiceChannelAsync(
            voiceChannelMain.Name,
            channelProperties =>
            {
                channelProperties.Bitrate = properties.Bitrate;
                channelProperties.UserLimit = properties.UserLimit;
                channelProperties.Position = properties.Position;
                channelProperties.CategoryId = properties.CategoryId;
            });
        voiceChannelMain.DiscordId = restVoiceChannel.Id.ToString();
        AddOrUpdateDatabaseVoiceChannel(voiceChannelMain);
        return voiceChannelMain;
    }
}