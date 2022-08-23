using System;
using System.Collections.Generic;
using Dikubot.DataLayer.Database.Guild.Models.Channel.SubModels.Overwrite;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild.Models.Channel.VoiceChannel
{
    /// <summary>
    /// Class for for retrieving information from the VoiceChannel collection.
    /// </summary>
    public class VoiceChannelMongoService : ChannelMongoService<VoiceChannelMainModel>
    {
        public VoiceChannelMongoService(SocketGuild guild) : base("VoiceChannels", guild)
        {
        }

        /// <Summary>Converts a SocketVoiceChannel to a VoiceChannelModel.</Summary>
        /// <param name="voiceChannel">The SocketVoiceChannel to be converted.</param>
        /// <return>Returns a VoiceChannelModel.</return>
        public VoiceChannelMainModel SocketToModel(SocketVoiceChannel voiceChannel)
        {
            var _voiceChannel = new VoiceChannelMainModel();
            _voiceChannel.DiscordId = voiceChannel.Id.ToString();
            _voiceChannel.Bitrate = voiceChannel.Bitrate;
            _voiceChannel.Position = voiceChannel.Position;
            _voiceChannel.CreatedAt = voiceChannel.CreatedAt.DateTime;
            _voiceChannel.Name = voiceChannel.Name;
            _voiceChannel.UserLimit = voiceChannel.UserLimit;
            _voiceChannel.DiscordCategoryId = voiceChannel.CategoryId.ToString();
            _voiceChannel.PermissionOverwrites = new List<OverwriteMainModel>();
            foreach (var overwrite in voiceChannel.PermissionOverwrites)
            {
                _voiceChannel.PermissionOverwrites.Add(new OverwriteMainModel(overwrite));
            }

            return _voiceChannel;
        }

        /// <Summary>Converts a RestVoiceChannel to a VoiceChannelModel.</Summary>
        /// <param name="voiceChannel">The RestVoiceChannel to be converted.</param>
        /// <return>Returns a VoiceChannelModel.</return>
        public VoiceChannelMainModel RestToModel(RestVoiceChannel voiceChannel)
        {
            var _voiceChannel = new VoiceChannelMainModel();
            _voiceChannel.DiscordId = voiceChannel.Id.ToString();
            _voiceChannel.Bitrate = voiceChannel.Bitrate;
            _voiceChannel.Position = voiceChannel.Position;
            _voiceChannel.CreatedAt = voiceChannel.CreatedAt.DateTime;
            _voiceChannel.Name = voiceChannel.Name;
            _voiceChannel.UserLimit = voiceChannel.UserLimit;
            _voiceChannel.DiscordCategoryId = voiceChannel.CategoryId.ToString();
            _voiceChannel.PermissionOverwrites = new List<OverwriteMainModel>();
            foreach (var overwrite in voiceChannel.PermissionOverwrites)
            {
                _voiceChannel.PermissionOverwrites.Add(new OverwriteMainModel(overwrite));
            }

            return _voiceChannel;
        }

        /// <Summary>Converts a VoiceChannelModel to a VoiceChannelProperties.</Summary>
        /// <param name="voiceChannelMainModel">The VoiceChannelModel to be converted.</param>
        /// <return>Returns a VoiceChannelProperties.</return>
        public VoiceChannelProperties ModelToVoiceChannelProperties(VoiceChannelMainModel voiceChannelMainModel)
        {
            var voiceChannelProperties = new VoiceChannelProperties();
            voiceChannelProperties.Bitrate = voiceChannelMainModel.Bitrate;
            voiceChannelProperties.UserLimit = voiceChannelMainModel.UserLimit;
            voiceChannelProperties.Name = voiceChannelMainModel.Name;
            voiceChannelProperties.Position = voiceChannelMainModel.Position;
            voiceChannelProperties.CategoryId = Convert.ToUInt64(voiceChannelMainModel.DiscordCategoryId);
            return voiceChannelProperties;
        }
    }
}