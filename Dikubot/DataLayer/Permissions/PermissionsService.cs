using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Guild.Models.Channel.CategoryChannel;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel;
using Dikubot.DataLayer.Database.Guild.Models.Channel.VoiceChannel;
using Dikubot.DataLayer.Database.Guild.Models.Guild;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Permissions
{
    /// <Summary>Class for talking between the database and Discord.</Summary>
    public partial class PermissionsService
    {
        /// <Summary>The constructor of PermissionServices.</Summary>
        /// <param name="guild">The guild for which the PermissionService is being executed in.</param>
        SocketGuild guild; // This can not be made private.

        private RoleMongoService _roleMongoService;
        private VoiceChannelMongoService _voiceChannelMongoService;
        private TextChannelMongoService _textChannelMongoService;
        private CategoryChannelMongoService _categoryChannelMongoService;
        private UserGuildMongoService _userMongoService;
        private GuildMongoService _guildMongoService;
        private readonly GuildSettingsMongoService _guildSettingsMongoService;

        public PermissionsService(SocketGuild guild)
        {
            this.guild = guild;
            _roleMongoService = new RoleMongoService(guild);
            _voiceChannelMongoService = new VoiceChannelMongoService(guild);
            _textChannelMongoService = new TextChannelMongoService(guild);
            _categoryChannelMongoService = new CategoryChannelMongoService(guild);
            _userMongoService = new UserGuildMongoService(guild);
            _guildMongoService = new GuildMongoService(guild);
            _guildSettingsMongoService = new GuildSettingsMongoService();
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

        public GuildSettingsMongoService GetGuildSettingsService()
        {
            return _guildSettingsMongoService;
        }
    }
}