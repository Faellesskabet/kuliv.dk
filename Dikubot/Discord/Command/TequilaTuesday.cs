using System;
using System.Threading.Tasks;
using Discord.Interactions;

namespace Dikubot.Discord.Command
{
    public class TequilaTuesday : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ttt", "is today TTT?")]
        public async Task TequilaTuesdayCommand()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
            {
                await ReplyAsync("JA DET ER TTT I DAG!!!!");
                return;
            }

            await ReplyAsync("Nej!!! :(");
        }
    }
}