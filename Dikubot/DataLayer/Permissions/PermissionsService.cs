using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Commands;
using Dikubot.Database.Models.Role;
using Dikubot.Database.Models.VoiceChannel;
using Discord;
using Discord.WebSocket;

namespace Dikubot.Permissions
{
    /// <Summary>Class for talking between the database and Discord.</Summary>
    public partial class PermissionsService
    {
        /// <Summary>The constructor of PermissionServices.</Summary>
        /// <param name="context">The context for which the PermissionService is being executed in.</param
        SocketCommandContext context; // This can not be made private.
        private readonly RoleServices _roleServices;
        private readonly VoiceChannelServices _voiceChannelServices;
        public PermissionsService(SocketCommandContext context)
        {
            this.context = context;
            _roleServices = new RoleServices();
            _voiceChannelServices = new VoiceChannelServices();
        }
    }
}