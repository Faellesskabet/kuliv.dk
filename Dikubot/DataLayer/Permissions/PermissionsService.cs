using Dikubot.Database.Models;
using Dikubot.Database.Models.CategoryChannel;
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
        /// <param name="guild">The guild for which the PermissionService is being executed in.</param>
        SocketGuild guild; // This can not be made private.

        RoleServices _roleServices;
        VoiceChannelServices _voiceChannelServices;
        TextChannelServices _textChannelServices;
        CategoryChannelServices _categoryChannelServices;
        UserServices _userServices;

        public PermissionsService(SocketGuild guild)
        {
            this.guild = guild;
            _roleServices = new RoleServices();
            _voiceChannelServices = new VoiceChannelServices();
            _textChannelServices = new TextChannelServices();
            _categoryChannelServices = new CategoryChannelServices();
            _userServices = new UserServices();
        }

        /// <Summary>Takes channels, roles and users information from discord and saves it in the database.</Summary>
        /// <returns>Void.</returns>
        public void SetDatabase()
        {
            SetDatabaseRoles();
            SetDatabaseUsers();
            SetDatabaseCategoryChannels();
            SetDatabaseVoiceChannels();
            SetDatabaseTextChannels();
        }

        /// <Summary>Takes channels, roles and users information from database and saves it in the discord.</Summary>
        /// <returns>Void.</returns>
        public void SetDiscord()
        {
            SetDiscordRoles();
            SetDiscordUsers();
            SetDiscordCategoryChannels();
            SetDiscordVoiceChannels();
            SetDiscordTextChannels();
        }
    }
}