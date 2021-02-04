using System;
using Discord.WebSocket;
using System.Collections.Generic;
using Dikubot.Database.Models.Channel;
using Dikubot.Database.Models.Channel.SubModels;
using Discord;
using Discord.Rest;

namespace Dikubot.Database.Models.VoiceChannel
{
    /// <summary>
    /// Class for for retrieving information from the VoiceChannel collection.
    /// </summary>
    public class VoiceChannelServices : ChannelServices<VoiceChannelModel>
    {
        public VoiceChannelServices() : base("Main", "VoiceChannels")
        {
        }

        /// <Summary>Converts a SocketVoiceChannel to a VoiceChannelModel.</Summary>
        /// <param name="voiceChannel">The SocketVoiceChannel to be converted.</param>
        /// <return>Returns a VoiceChannelModel.</return>
        public VoiceChannelModel SocketToModel(SocketVoiceChannel voiceChannel)
        {
            var _voiceChannel = new VoiceChannelModel();
            _voiceChannel.DiscordId = voiceChannel.Id.ToString();
            _voiceChannel.Bitrate = voiceChannel.Bitrate;
            _voiceChannel.Position = voiceChannel.Position;
            _voiceChannel.CreatedAt = voiceChannel.CreatedAt.DateTime;
            _voiceChannel.Name = voiceChannel.Name;
            _voiceChannel.UserLimit = voiceChannel.UserLimit;
            _voiceChannel.DiscordCategoryId = voiceChannel.CategoryId.ToString();
            _voiceChannel.PermissionOverwrites = new List<OverwriteModel>();
            foreach (var overwrite in voiceChannel.PermissionOverwrites)
            {
                _voiceChannel.PermissionOverwrites.Add(new OverwriteModel(overwrite));
            }

            return _voiceChannel;
        }

        /// <Summary>Converts a RestVoiceChannel to a VoiceChannelModel.</Summary>
        /// <param name="voiceChannel">The RestVoiceChannel to be converted.</param>
        /// <return>Returns a VoiceChannelModel.</return>
        public VoiceChannelModel RestToModel(RestVoiceChannel voiceChannel)
        {
            var _voiceChannel = new VoiceChannelModel();
            _voiceChannel.DiscordId = voiceChannel.Id.ToString();
            _voiceChannel.Bitrate = voiceChannel.Bitrate;
            _voiceChannel.Position = voiceChannel.Position;
            _voiceChannel.CreatedAt = voiceChannel.CreatedAt.DateTime;
            _voiceChannel.Name = voiceChannel.Name;
            _voiceChannel.UserLimit = voiceChannel.UserLimit;
            _voiceChannel.DiscordCategoryId = voiceChannel.CategoryId.ToString();
            _voiceChannel.PermissionOverwrites = new List<OverwriteModel>();
            foreach (var overwrite in voiceChannel.PermissionOverwrites)
            {
                _voiceChannel.PermissionOverwrites.Add(new OverwriteModel(overwrite));
            }

            return _voiceChannel;
        }

        /// <Summary>Converts a VoiceChannelModel to a VoiceChannelProperties.</Summary>
        /// <param name="voiceChannelModel">The VoiceChannelModel to be converted.</param>
        /// <return>Returns a VoiceChannelProperties.</return>
        public VoiceChannelProperties ModelToVoiceChannelProperties(VoiceChannelModel voiceChannelModel)
        {
            var voiceChannelProperties = new VoiceChannelProperties();
            voiceChannelProperties.Bitrate = voiceChannelModel.Bitrate;
            voiceChannelProperties.UserLimit = voiceChannelModel.UserLimit;
            voiceChannelProperties.Name = voiceChannelModel.Name;
            voiceChannelProperties.Position = voiceChannelModel.Position;
            voiceChannelProperties.CategoryId = Convert.ToUInt64(voiceChannelModel.DiscordCategoryId);
            return voiceChannelProperties;
        }
    }
}