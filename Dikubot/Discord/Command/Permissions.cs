using Discord.Commands;
using Dikubot.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using Dikubot.Database.Models.VoiceChannel;
using System.Threading.Tasks;

namespace Dikubot.Discord.Command
{
    /// <Summary>Permission class for the command.</Summary>
    public class Permissions : ModuleBase<SocketCommandContext>
    {
        /// <Summary>The function that is called when using !permissions.</Summary>
        [Command("permissions")]
        [Summary("Command for managing permissions.")]
        public async Task PermissionsAsync([Summary("The arguments")] params string[] args)
        {
            var mod = Context.Guild.Roles.ToList().Find(role => role.Name == "Mod");
            if (mod != null && !Context.Guild.GetUser(Context.User.Id).Roles.Contains(mod))
                return;
            if (mod == null)
                return;

            var permissionsService = new PermissionsService(Context.Guild);

            if (args == null)
            {
                await ReplyAsync("Invalid mængde argumenter.");
                return;
            }

            // Uploads Permissions from the server to the database.
            if (args[0] == "save" && args.Length == 1)
            {
                permissionsService.SetDatabase();
                await ReplyAsync("Rolle permissions er blevet skrevet til database.");
                return;
            }

            // Changes Permissions on the server.
            if (args[0] == "overwrite" && args.Length == 1)
            {
                permissionsService.SetDiscord();
                await ReplyAsync("Rolle permissions er blevet overskrevet.");
                return;
            }

            if (args[0] == "makeExpandable" && args.Length == 2)
            {
                var voiceChannelServices = new VoiceChannelServices();
                var newModel = voiceChannelServices.Get(model => model.DiscordId == args[1]);
                if (newModel == null)
                {
                    await ReplyAsync("Invalid Voice Channel Id.");
                    return;
                }

                newModel.ExpandOnJoin = true;
                newModel.ExpandId = newModel.DiscordId;
                permissionsService.AddOrUpdateDatabaseVoiceChannel(newModel);
                await ReplyAsync("Har opdateret voice chat til af være expandable.");
                return;
            }
        }
    }
}