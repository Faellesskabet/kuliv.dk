using System;
using System.Linq.Expressions;
using Dikubot.Discord.EventListeners;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages.News;

public class NewsMongoServices : MessageMongoService
{
    public NewsMongoServices(Database database, IGuildMongoFactory guildMongoFactory,
        SocketGuild guild, MessageListener messageListener) : base(database, guildMongoFactory, guild, messageListener)
    {
    }

    protected override Expression<Func<TextChannelMainModel, bool>> ChannelFilter()
    {
        return model => model.IsNewsChannel.Value;
    }

    public override string GetCollectionName()
    {
        return "News";
    }
}