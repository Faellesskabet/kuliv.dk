using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages.News;
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
            ProcessMessage(message);
            
            #if DEBUG
            DebugMessage(message);    
            #endif
        }

        public async Task OnMessageRemoved(Cacheable<IMessage, ulong> cacheableMessage, Cacheable<IMessageChannel, ulong> cacheableChannel)
        {
            ProcessMessage(cacheableMessage.Value, true);
        }

        private void DebugMessage(SocketMessage message)
        {
            Logger.Debug($"{message.Author.ToString()} > {message.ToString()}");
        }

        public static void ProcessMessage(IMessage message, bool remove = false)
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
                textChannelServices.Get(model => model.DiscordId == message.Channel.Id.ToString());
            if (textChannelMainModel == null)
            {
                return;
            }
            
            if (textChannelMainModel.IsQuoteChannel.GetValueOrDefault())
            {
                MessageService messageService = new QuoteServices(guild);
                if (remove)
                {
                    messageService.Remove(messageService.IMessageToModel(message));
                }
                else
                {
                    messageService.Insert(messageService.IMessageToModel(message));
                }
            }
            
            if (textChannelMainModel.IsNewsChannel.GetValueOrDefault())
            {
                MessageService messageService = new NewsServices(guild);
                if (remove)
                {
                    messageService.Remove(messageService.IMessageToModel(message));
                }
                else
                {
                    messageService.Insert(messageService.IMessageToModel(message));
                }
            }
            
        }
    }
}