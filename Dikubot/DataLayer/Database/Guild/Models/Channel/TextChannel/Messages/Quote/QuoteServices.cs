using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dikubot.Discord.EventListeners;
using Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages.Quote
{
    public class QuoteServices : GuildServices<QuoteModel>
    {
        public QuoteServices(SocketGuild guild) : base("Quotes", guild) { }
        
        public QuoteModel SocketToModel(SocketMessage textMessage)
        {
            var quoteModel = new QuoteModel();
            quoteModel.MessageId = textMessage.Id.ToString();
            quoteModel.ChannelId = textMessage.Channel.Id.ToString();
            quoteModel.TimeStamp = textMessage.Timestamp;
            return quoteModel;
        }

        public QuoteModel IMessageToModel(IMessage textMessage)
        {
            var quoteModel = new QuoteModel();
            quoteModel.MessageId = textMessage.Id.ToString();
            quoteModel.ChannelId = textMessage.Channel.Id.ToString();
            quoteModel.TimeStamp = textMessage.Timestamp;
            return quoteModel;
        }

        public async void DownloadQuotesFromChannel(SocketTextChannel textChannel)
        {
            if (textChannel is not SocketGuildChannel channel)
            {
                return;
            }
            SocketGuild guild = channel.Guild;
            TextChannelServices textChannelServices = new TextChannelServices(guild);
            TextChannelMainModel textChannelMainModel = 
                textChannelServices.Get(model => model.IsQuoteChannel 
                                                 && model.DiscordId == channel.Id.ToString());
            if (textChannelMainModel == null)
            {
                return;
            }
            
            List<QuoteModel> messages = new List<QuoteModel>();
            var asyncEnumerator = textChannel.GetMessagesAsync(Int32.MaxValue).Flatten();
            await foreach (var message in asyncEnumerator.WithCancellation(default).ConfigureAwait(false))
            {
                if (Exists(model => model.MessageId == message.Id.ToString()))
                {
                    continue;
                }
                MessageListener.ProcessQuote(message);
            }
        }

        public static void DownloadAllQuotes(SocketGuild guild)
        {
            QuoteServices quoteServices = new QuoteServices(guild);
            TextChannelServices textChannelServices = new TextChannelServices(guild);
            List<TextChannelMainModel> quoteChannels = textChannelServices.GetAll(model => model.IsQuoteChannel);
            foreach (var quoteChannel in quoteChannels)
            {
                quoteServices.DownloadQuotesFromChannel(guild.GetTextChannel( Convert.ToUInt64(quoteChannel.DiscordId)));
            }
        }
    }
}