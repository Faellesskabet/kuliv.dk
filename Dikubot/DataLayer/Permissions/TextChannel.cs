using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Permissions
{
    public partial class PermissionsService
    {
        /// <Summary>Will sync all the textChannels on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void SetDatabaseTextChannels()
        {
            var textChannelModels = _textChannelMongoService.Get();
            var socketRoles = guild.TextChannels.ToList();
            var discordRoleIds = new HashSet<ulong>(socketRoles.Select(role => role.Id));
            var toBeRemoved = textChannelModels.Where(model => !discordRoleIds.Contains(UInt64.Parse(model.DiscordId))).ToList();

            Func<TextChannelMainModel, SocketTextChannel, bool> inDB = (m0, m1) =>
                Convert.ToUInt64(m0.DiscordId) == m1.Id;

            // Remove all the text channels from the database if they are not on the discord server.
            toBeRemoved.RemoveAll(m =>
                socketRoles.Exists(n => inDB(m, n)));

            // Remove the text channels from the database that is not on the discord server.
            toBeRemoved.ForEach(m => _textChannelMongoService.Remove(m));

            // Makes an upsert of the text channels on the server so they match the ones in the database.
            socketRoles.ForEach(model =>
                _textChannelMongoService.Upsert(_textChannelMongoService.SocketToModel(model)));
        }

        /// <Summary>Will sync all the text channels on the database to the discord server.</Summary>
        /// <return>void.</return>
        public async void SetDiscordTextChannels()
        {
            var textChannelModels = _textChannelMongoService.Get();
            var socketTextChannels = guild.TextChannels.ToList();
            var toBeRemoved = new List<SocketTextChannel>(socketTextChannels);

            Func<TextChannelMainModel, SocketTextChannel, bool> inDB =
                (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id;

            // Remove all the text channels from the discord server if they are not in the database.
            toBeRemoved.RemoveAll(m => textChannelModels.Exists(n => inDB(n, m)));

            foreach (var socketTextChannel in toBeRemoved)
                await socketTextChannel.DeleteAsync();

            foreach (var textChannelModel in textChannelModels)
            {
                // Finds the text channel discord server text channel that match the text channel in the database.
                var socketTextChannel = socketTextChannels.Find(socket =>
                    Convert.ToUInt64(textChannelModel.DiscordId) == socket.Id);

                if (socketTextChannel == null)
                {
                    // If the text channel could not be found create it.
                    var properties = _textChannelMongoService.ModelToTextChannelProperties(textChannelModel);
                    var restTextChannel = await guild.CreateTextChannelAsync(
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

                    // Adds all the overwrite Permissions from the DB.
                    foreach (var overwriteModel in textChannelModel.PermissionOverwrites)
                    {
                        var overwrite = overwriteModel.ToOverwrite();
                        if (overwrite.TargetType == PermissionTarget.Role)
                            await restTextChannel.AddPermissionOverwriteAsync(
                                guild.GetRole(overwrite.TargetId),
                                overwrite.Permissions);
                        else
                            await restTextChannel.AddPermissionOverwriteAsync(
                                guild.GetUser(overwrite.TargetId),
                                overwrite.Permissions);
                    }
                }
                else
                {
                    // If the text channel could be found modify it so it matches the database.
                    var properties = _textChannelMongoService.ModelToTextChannelProperties(textChannelModel);
                    await socketTextChannel.ModifyAsync(channelProperties =>
                    {
                        channelProperties.Position = properties.Position;
                        channelProperties.CategoryId = properties.CategoryId;
                        channelProperties.Topic = properties.Topic;
                        channelProperties.IsNsfw = properties.IsNsfw;
                        channelProperties.SlowModeInterval = properties.SlowModeInterval;
                        channelProperties.Name = properties.Name;
                    });

                    // Adds all the overwrite Permissions from the DB.
                    foreach (var overwriteModel in textChannelModel.PermissionOverwrites)
                    {
                        var overwrite = overwriteModel.ToOverwrite();
                        if (overwrite.TargetType == PermissionTarget.Role)
                            await socketTextChannel.AddPermissionOverwriteAsync(
                                guild.GetRole(overwrite.TargetId),
                                overwrite.Permissions);
                        else
                            await socketTextChannel.AddPermissionOverwriteAsync(
                                guild.GetUser(overwrite.TargetId),
                                overwrite.Permissions);
                    }
                }
            }
        }

        /// <Summary>Add a text channel on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void AddOrUpdateDatabaseTextChannel(SocketTextChannel textChannel) =>
            _textChannelMongoService.Upsert(_textChannelMongoService.SocketToModel(textChannel));

        /// <Summary>Add a text channel on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void AddOrUpdateDatabaseTextChannel(RestTextChannel textChannel) =>
            _textChannelMongoService.Upsert(_textChannelMongoService.RestToModel(textChannel));

        /// <Summary>Add a text channel on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void AddOrUpdateDatabaseTextChannel(TextChannelMainModel textChannelMain) =>
            _textChannelMongoService.Upsert(textChannelMain);

        /// <Summary>Removes a text channel from the database.</Summary>
        /// <return>void.</return>
        public void RemoveDatabaseTextChannel(SocketTextChannel textChannel) =>
            _textChannelMongoService.Remove(_textChannelMongoService.SocketToModel(textChannel));

        /// <Summary>Removes a text channel from the database.</Summary>
        /// <return>void.</return>
        public void RemoveDatabaseTextChannel(RestTextChannel textChannel) =>
            _textChannelMongoService.Remove(_textChannelMongoService.RestToModel(textChannel));

        /// <Summary>Removes a text channel from the database.</Summary>
        /// <return>void.</return>
        public void RemoveDatabaseTextChannel(TextChannelMainModel textChannelMain) =>
            _textChannelMongoService.Remove(textChannelMain);

        /// <Summary>Removes a text channel from the database and the discord server.</Summary>
        /// <return>void.</return>
        public async Task RemoveTextChannel(SocketTextChannel textChannel)
        {
            RemoveDatabaseTextChannel(textChannel);
            await textChannel.DeleteAsync();
        }

        /// <Summary>Removes a text channel from the database and the discord server.</Summary>
        /// <return>void.</return>
        public async Task RemoveTextChannel(RestTextChannel textChannel)
        {
            RemoveDatabaseTextChannel(textChannel);
            await textChannel.DeleteAsync();
        }

        /// <Summary>Removes a text channel from the database and the discord server.</Summary>
        /// <return>void.</return>
        public async Task RemoveTextChannel(TextChannelMainModel textChannelMain)
        {
            RemoveDatabaseTextChannel(textChannelMain);
            await guild.GetTextChannel(Convert.ToUInt64(textChannelMain)).DeleteAsync();
        }

        /// <Summary>Adds a text channel to the database and the discord server.</Summary>
        /// <return>void.</return>
        public async Task<TextChannelMainModel> AddTextChannel(TextChannelMainModel textChannelMain)
        {
            var properties = _textChannelMongoService.ModelToTextChannelProperties(textChannelMain);
            var restTextChannel = await guild.CreateTextChannelAsync(
                textChannelMain.Name,
                channelProperties =>
                {
                    channelProperties.Position = properties.Position;
                    channelProperties.CategoryId = properties.CategoryId;
                    channelProperties.Topic = properties.Topic;
                    channelProperties.IsNsfw = properties.IsNsfw;
                    channelProperties.SlowModeInterval = properties.SlowModeInterval;
                });
            textChannelMain.DiscordId = restTextChannel.Id.ToString();
            AddOrUpdateDatabaseTextChannel(textChannelMain);
            return textChannelMain;
        }
    }
}