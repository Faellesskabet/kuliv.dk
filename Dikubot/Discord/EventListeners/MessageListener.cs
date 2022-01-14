using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages.Quote;
using Dikubot.DataLayer.Static;
using Discord;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners
{
    public class MessageListener
    {
        public async Task OnMessageReceived(SocketMessage message)
        {
            ProcessQuote(message);
            
            #if DEBUG
            DebugMessage(message);    
            #endif
        }
        private void DebugMessage(SocketMessage message)
        {
            Logger.Debug($"{message.Author.ToString()} > {message.ToString()}");
        }

        public static void ProcessQuote(IMessage message)
        {
            if (message.Source != MessageSource.User)
            {
                return;
            }

            if (message.Channel is not SocketGuildChannel channel)
            {
                return;
            }
            SocketGuild guild = channel.Guild;
            TextChannelServices textChannelServices = new TextChannelServices(guild);
            TextChannelMainModel textChannelMainModel = 
                textChannelServices.Get(model => model.IsQuoteChannel 
                                                 && model.DiscordId == message.Channel.Id.ToString());
            if (textChannelMainModel == null)
            {
                return;
            }

            QuoteServices quoteServices = new QuoteServices(guild);
            quoteServices.Insert(quoteServices.IMessageToModel(message));
        }
    }
}