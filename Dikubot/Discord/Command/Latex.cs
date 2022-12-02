using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;

namespace Dikubot.Discord.Command
{
    public class Latex : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("latex", "Latex to image")]
        public async Task HelpCommand([Summary(description:"Latex to be rendered as an image")] params string[] latex)
        {
            try {
                var embed = new EmbedBuilder();
                embed.WithTitle($"Latex: {latex}");
                // Thats a really random link... But it works!
                embed.WithImageUrl(Uri.EscapeUriString($"https://cloud.nawouak.net/run/tex2png--10?{latex}"));

                await ReplyAsync("", false, embed.Build());
            } catch {
                await ReplyAsync("Something went wrong...");
            }
        }
    }
}