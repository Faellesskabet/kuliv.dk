using System;
using System.Threading.Tasks;
using Discord.Interactions;

namespace Dikubot.Discord.Command;

public class Magic8Ball : InteractionModuleBase<SocketInteractionContext>
{
    private static readonly string[] answers =
    {
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
        "Meget tvivlsomt."
    };

    [SlashCommand("magic8ball", "Answer all your questions here")]
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