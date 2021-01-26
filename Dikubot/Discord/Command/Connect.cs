using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dikubot.Database.Models;
using Dikubot.DataLayer.Logic.User;
using Dikubot.Webapp.Shared;
using Dikubot.Webapp.Shared.Login;
using Discord.Commands;

namespace Dikubot.Discord.Command
{
    public class Connect : ModuleBase<SocketCommandContext>
    {
        
        [Command("connect")]
        [Summary("Connects you to the website")]
        public async Task ConnectAsync([Remainder][Summary("The connection password")] string password)
        {
            if (DiscordWebConnector.Validate(password, Context.User))
            {
                await ReplyAsync(":white_check_mark: Success! Din browser burde nu blive redirected...");
                return;
            }
            await ReplyAsync(":x: Din kode virker ikke! Refresh hjemmesiden for at få en ny");
        }
    }
}