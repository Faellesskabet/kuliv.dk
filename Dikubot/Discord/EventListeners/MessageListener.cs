using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild;
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
        private readonly IGuildMongoFactory _guildMongoFactory;

        public MessageListener(IGuildMongoFactory guildMongoFactory)
        {
            _guildMongoFactory = guildMongoFactory;
        }

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

        public void ProcessMessage(IMessage message, bool remove = false)
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
            TextChannelMongoService textChannelMongoService = _guildMongoFactory.Get<TextChannelMongoService>(guild);
            TextChannelMainModel textChannelMainModel = 
                textChannelMongoService.Get(model => model.DiscordId == message.Channel.Id.ToString());
            if (textChannelMainModel == null)
            {
                return;
            }
            
            if (textChannelMainModel.IsQuoteChannel.GetValueOrDefault())
            {
                MessageMongoService messageMongoService = _guildMongoFactory.Get<QuoteMongoServices>(guild);
                if (remove)
                {
                    messageMongoService.Remove(messageMongoService.IMessageToModel(message));
                }
                else
                {
                    messageMongoService.Insert(messageMongoService.IMessageToModel(message));
                }
            }
            
            if (textChannelMainModel.IsNewsChannel.GetValueOrDefault())
            {
                MessageMongoService messageMongoService = _guildMongoFactory.Get<NewsMongoServices>(guild);
                if (remove)
                {
                    messageMongoService.Remove(messageMongoService.IMessageToModel(message));
                }
                else
                {
                    messageMongoService.Insert(messageMongoService.IMessageToModel(message));
                }
            }
            
        }
    }
}