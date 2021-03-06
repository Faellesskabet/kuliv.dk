using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.Database.Models;
using Dikubot.Database.Models.Role;
using Dikubot.Database.Models.SubModels;
using Dikubot.Permissions;
using Discord.WebSocket;

namespace Dikubot.Discord.Command
{
    public class Echo : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        [Summary("Echoes a message.")]
        public async Task SayAsync([Remainder] [Summary("The text to echo")]
            string echo)
        {
            await ReplyAsync(echo);
        }
    }
}