using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Dikubot.Discord.Command
{
    public class TequilaTuesday : ModuleBase<SocketCommandContext>
    {
        [Command("tequilatuesday")]
        [Alias("ttt", "tequilatightstuesday", "tequilatightstirsdag", "tequilatuesday", "tequilatirsdag")]
        [Summary("Er det TTT i dag?")]
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