using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorFluentUI;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;

namespace Dikubot.Discord.Command
{
    public class Latex : ModuleBase<SocketCommandContext>
    {
        [Command("latex")]
        [Summary("Latex to image")]
        public async Task HelpCommand([Remainder] string latex)
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