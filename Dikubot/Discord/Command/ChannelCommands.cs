using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Database.Guild.Models.Channel.VoiceChannel;
using Dikubot.DataLayer.Permissions;
using Discord.Interactions;

namespace Dikubot.Discord.Command
{
    [global::Discord.Commands.Group("channel")]
    public class ChannelCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private IPermissionServiceFactory _permissionServiceFactory;
        private IGuildMongoFactory _guildMongoFactory;

        public ChannelCommands(IPermissionServiceFactory permissionServiceFactory, IGuildMongoFactory guildMongoFactory)
        {
            _permissionServiceFactory = permissionServiceFactory;
            _guildMongoFactory = guildMongoFactory;
        }

        [SlashCommand("expandable", "Make a channel expandable")]
        public async Task ExpandAsync([Summary(description:"Channel id")] params string[] channelid)
        {
            var mod = Context.Guild.Roles.Any(role => role.Permissions.Administrator);
            if (!mod)
            {
                return;
            }

            var permissionsService = _permissionServiceFactory.Get(Context.Guild);

            if (channelid.Length != 1)
            {
                await ReplyAsync("Missing channel id");
                return;
            }
            
            var voiceChannelServices = _guildMongoFactory.Get<VoiceChannelMongoService>(Context.Guild);
            var newModel = voiceChannelServices.Get(model => model.DiscordId == channelid[0]);
            if (newModel == null)
            {
                await ReplyAsync("Invalid Voice Channel Id.");
                return;
            }

            newModel.ExpandOnJoin = true;
            newModel.ExpandId = newModel.DiscordId;
            permissionsService.AddOrUpdateDatabaseVoiceChannel(newModel);
            await ReplyAsync("Har opdateret voice chat til af være expandable.");
            
        }
    }
}