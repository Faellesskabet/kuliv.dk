using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dikubot.discord.Command
{
    public class Echo : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        [Summary("Echoes a message.")]
        public async Task SayAsync([Remainder][Summary("The text to echo")] string echo) {
            await ReplyAsync(echo);
        }
    }
}
