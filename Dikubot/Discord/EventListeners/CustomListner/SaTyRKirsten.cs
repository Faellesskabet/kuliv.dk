using System.Threading.Tasks;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners.CustomListner
{
    public class SaTyRKirsten
    {
        private Task Kristen(SocketMessage message)
        {
            if (message.ToString().ToLower().Contains("vi elsker alle dyr"))
            {
                message.Channel.SendMessageAsync("og Kirsten");
            }
            return Task.CompletedTask;
        }
        
        public SaTyRKirsten()
        {
            DiscordBot.Client.MessageReceived += Kristen;
            
        }
        
        
        
    }
}