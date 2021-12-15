using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Dikubot.Discord.Command
{
    public class Magic8Ball : ModuleBase<SocketCommandContext>
    {
        private static readonly string[] answers = new string[] {
            "Det er sikkert.",
            "Det er bestemt.",
            "Uden tvivl.",
            "Ja helt sikkert.",
            "Det kan du tro på.",
            "Som jeg ser det, ja.",
            "Højst sandsynlig.",
            "Det lover godt.",
            "Ja.",
            "Skiltene peger mod et ja.",
            "Svar uklar, prøv igen.",
            "Spørg igen senere.",
            "Bedre ikke at fortælle dig nu.",
            "Kan ikke forudsiges nu.",
            "Koncentrer dig og spørg igen.",
            "Regn ikke med det.",
            "Mit svar er nej.",
            "Mine kilder siger nej.",
            "Ser ikke for godt ud.",
            "Meget tvivlsomt."};

        [Command("magiceightball")]
        [Alias("m8b", "magiskottekugle", "magiskotteboldt", "magisk8boldt", "magisk8kugle")]
        [Summary("Få svar på dine spørgsmål!")]
        public async Task Magic8BallCommand(params string[] question)
        {
            if (question.Length < 1)
            {
                await ReplyAsync("Du stillede ikke et spørgsmål! >:(");
                return;
            }
            Random rand = new Random();
            await ReplyAsync(answers[rand.Next(answers.Length)]);
        }
    }
}