using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild.Models.Channel.VoiceChannel;
using Dikubot.DataLayer.Permissions;
using Discord.Commands;

namespace Dikubot.Discord.Command
{
    [Group("channel")]
    public class ChannelCommands : ModuleBase<SocketCommandContext>
    {
        [Command("expandable")]
        [Summary("Make a channel expandable")]
        public async Task ExpandAsync([Summary("Channel id")] params string[] channelid)
        {
            var mod = Context.Guild.Roles.Any(role => role.Permissions.Administrator);
            if (!mod)
            {
                return;
            }

            var permissionsService = new PermissionsService(Context.Guild);

            if (channelid.Length != 1)
            {
                await ReplyAsync("Missing channel id");
                return;
            }
            
            var voiceChannelServices = new VoiceChannelMongoService(Context.Guild);
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