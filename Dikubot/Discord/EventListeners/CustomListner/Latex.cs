using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners.CustomListner
{
    public class LatexLisner
    {
        private Task LatexCompiler(SocketMessage message)
        {

            if (message.Content.Contains("$"))
            {
                try
                {
                    string latex = message.Content;
                    var embed = new EmbedBuilder();
                    embed.WithTitle($"Latex: ");
                    // Thats a really random link... But it works!
                    //https://latex.codecogs.com / type . format ? LaTeX-Markup
                    latex = $"\\text{{{latex}}}";
                    //embed.WithImageUrl(Uri.EscapeUriString($"https://latex.codecogs.com/png.image?\\dpi{200}{latex}"));
                    embed.WithImageUrl(Uri.EscapeUriString($"https://cloud.nawouak.net/run/tex2png--10?{latex}"));
                    message.Channel.SendMessageAsync("",false,embed.Build());
                } catch {
                    message.Channel.SendMessageAsync("Something went wrong...");
                
                }
            }
            
            
            return Task.CompletedTask;
        }
        
        public LatexLisner()
        {
            DiscordBot.Client.MessageReceived += LatexCompiler;
            
        }
        
        
        
    }
}