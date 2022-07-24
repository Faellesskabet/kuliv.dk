using System.Linq;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Guild.Models.Channel.CategoryChannel;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel;
using Dikubot.DataLayer.Database.Guild.Models.Channel.VoiceChannel;
using Dikubot.DataLayer.Database.Guild.Models.Guild;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Permissions
{
    /// <Summary>Class for talking between the database and Discord.</Summary>
    public partial class PermissionsService
    {
        /// <Summary>The constructor of PermissionServices.</Summary>
        /// <param name="guild">The guild for which the PermissionService is being executed in.</param>
        SocketGuild guild; // This can not be made private.

        private RoleServices _roleServices;
        private VoiceChannelServices _voiceChannelServices;
        private TextChannelServices _textChannelServices;
        private CategoryChannelServices _categoryChannelServices;
        private UserGuildServices _userServices;
        private GuildServices _guildServices;
        private readonly GuildSettingsService _guildSettingsService;

        public PermissionsService(SocketGuild guild)
        {
            this.guild = guild;
            _roleServices = new RoleServices(guild);
            _voiceChannelServices = new VoiceChannelServices(guild);
            _textChannelServices = new TextChannelServices(guild);
            _categoryChannelServices = new CategoryChannelServices(guild);
            _userServices = new UserGuildServices(guild);
            _guildServices = new GuildServices(guild);
            _guildSettingsService = new GuildSettingsService();
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
            SetDatabaseGuild();
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

        public GuildSettingsService GetGuildSettingsService()
        {
            return _guildSettingsService;
        }
    }
}