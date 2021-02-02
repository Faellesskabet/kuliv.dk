using Discord.Commands;
using Dikubot.Database.Models.Role;
using Dikubot.Database.Models.VoiceChannel;
using Dikubot.Database.Models.TextChannel;
using Discord.WebSocket;

namespace Dikubot.Permissions
{
    /// <Summary>Class for talking between the database and Discord.</Summary>
    public partial class PermissionsService
    {
        /// <Summary>The constructor of PermissionServices.</Summary>
        /// <param name="guild">The context for which the PermissionService is being executed in.</param
        private readonly SocketGuild guild; // This can not be made private.
        private readonly RoleServices _roleServices;
        private readonly VoiceChannelServices _voiceChannelServices;
        private readonly TextChannelServices _textChannelServices;
        public PermissionsService(SocketGuild guild)
        {
            this.guild = guild;
            _roleServices = new RoleServices();
            _voiceChannelServices = new VoiceChannelServices();
            _textChannelServices = new TextChannelServices();
        }

        public void SetDatabase()
        {
            SetDatabaseRoles();
            SetDatabaseVoiceChannels();
            SetDatabaseTextChannels();
        }

        public void SetDiscord()
        {
            SetDiscordRoles();
            SetDiscordVoiceChannels();
            SetDiscordTextChannels();
        }
    }
}