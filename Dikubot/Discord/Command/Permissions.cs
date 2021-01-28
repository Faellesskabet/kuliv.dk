using Discord.Commands;
using Dikubot.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var permissionsService = new PermissionsService(Context);
            if (args == null) {
                await ReplyAsync("Invalid mængde argumenter.");
                return;
            }

            // Uploads Permissions from the server to the database.
            if (args[0] == "save" && args.Length == 1)
            {
                permissionsService.UploadRoles();
                permissionsService.UploadVoiceChannels();
                await ReplyAsync("Rolle permissions er blevet skrevet til database.");
                return;
            }
            
            // Changes Permissions on the server.
            if (args[0] == "overwrite" && args.Length == 1)
            {
                permissionsService.DownloadRoles();
                permissionsService.DownloadVoiceChannels();
                await ReplyAsync("Rolle permissions er blevet overskrevet.");
                return;
            }
        }
    }
}
