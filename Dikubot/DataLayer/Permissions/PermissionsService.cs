using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Database.Guild;
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
        readonly SocketGuild guild; // This can not be made private.

        private readonly RoleMongoService _roleMongoService;
        private readonly VoiceChannelMongoService _voiceChannelMongoService;
        private readonly TextChannelMongoService _textChannelMongoService;
        private readonly CategoryChannelMongoService _categoryChannelMongoService;
        private readonly UserGuildMongoService _userMongoService;
        private readonly GuildMongoService _guildMongoService;
        private readonly GuildSettingsMongoService _guildSettingsMongoService;
        private IGuildMongoFactory _guildMongoFactory;
        private readonly UserGlobalMongoService _userGlobalMongoService;

        public PermissionsService(IGuildMongoFactory guildMongoFactory, GuildSettingsMongoService guildSettingsMongoService,
        UserGlobalMongoService userGlobalMongoService, SocketGuild guild)
        {
            this.guild = guild;
            _guildMongoFactory = guildMongoFactory;
            _roleMongoService = guildMongoFactory.Get<RoleMongoService>(guild);
            _voiceChannelMongoService = guildMongoFactory.Get<VoiceChannelMongoService>(guild);
            _textChannelMongoService = guildMongoFactory.Get<TextChannelMongoService>(guild);
            _categoryChannelMongoService = guildMongoFactory.Get<CategoryChannelMongoService>(guild);
            _userMongoService = guildMongoFactory.Get<UserGuildMongoService>(guild);
            _guildMongoService = guildMongoFactory.Get<GuildMongoService>(guild);
            _guildSettingsMongoService = guildSettingsMongoService;
            _userGlobalMongoService = userGlobalMongoService;
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