using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dikubot.Database.Models;
using Dikubot.Webapp.Shared;
using Dikubot.Webapp.Shared.Login;
using Discord.Commands;

namespace Dikubot.Discord.Command
{
    public class Connect : ModuleBase<SocketCommandContext>
    {
        /*
         * Implement auto-removal of pending_passwords to avoid filling memory
         */
        public static Dictionary<string, ConnectDiscord> pending_passwords = new Dictionary<String, ConnectDiscord>();
        
        [Command("connect")]
        [Summary("Connects you to the website")]
        public async Task ConnectAsync([Remainder][Summary("The connection password")] string password)
        {
            if (pending_passwords.ContainsKey(password))
            {
                pending_passwords[password].DiscordConnected(Context.User);
                pending_passwords.Remove(password);
                await ReplyAsync(":white_check_mark: Success! Din browser burde nu blive redirected...");
                return;
            }
            await ReplyAsync(":x: Din kode virker ikke! Refresh hjemmesiden for at få en ny");
        }
    }
}