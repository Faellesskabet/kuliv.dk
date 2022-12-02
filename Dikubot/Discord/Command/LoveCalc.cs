using System;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;

namespace Dikubot.Discord.Command
{
    public class LoveCalc : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("lovecalc", "Give me a list of words and I'll tell you how much they all love each other!")]
        public async Task LoveCalcCommand([Summary(description:"Your lovely words")] params string[] words)
        {
            if (words.Length < 2)
            {
                await ReplyAsync("I need at least 2 space seperated words for this command!");
                return;
            }
            string[] wordsCopy = (string[]) words.Clone();
            Array.Sort(wordsCopy);
            int sum = 0;
            foreach (var word in wordsCopy)
            {
                foreach (char character in word)
                {
                    sum += (int) Char.GetNumericValue(char.ToLowerInvariant(character));
                }
            }
            double loveRatio = new Random(sum).NextDouble();
            StringBuilder message = new StringBuilder();
            for (int i = 0; i < words.Length - 1; i++)
            {
                Console.WriteLine(words[i]);
                message.Append(words[i]);
                if (i < words.Length - 2)
                {
                    message.Append(", ");
                }
            }
            message.Append((" & "));
            message.Append(words[^1]);
            await ReplyAsync($"The love between \"{message}\" is {Math.Floor(loveRatio * 100)}%");
        }
    }
}