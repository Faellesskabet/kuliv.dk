using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Interactions;

namespace Dikubot.Discord.Command
{
    public class Ping : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping", ":thinking:")]
        public async Task PingCommand()
        {
            await ReplyAsync($"{new Random().Next(1, 200)}ms... I mean pong!");
        }
    }
}