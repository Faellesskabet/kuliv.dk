using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Dikubot.Discord.Command
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary(":thinking:")]
        public async Task PingCommand()
        {
            await ReplyAsync($"{new Random().Next(1, 200)}ms... I mean pong!");
        }
    }
}