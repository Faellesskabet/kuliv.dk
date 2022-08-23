using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dikubot.Discord.EventListeners;
using Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages.Quote
{
    public class QuoteMongoServices : MessageMongoService
    {
        public QuoteMongoServices(SocketGuild guild) : base("Quotes", guild)
        {
        }

        protected override Expression<Func<TextChannelMainModel, bool>> ChannelFilter()
        {
            return model => model.IsQuoteChannel.Value;
        }
    }
}