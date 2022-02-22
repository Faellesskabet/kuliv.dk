using System;
using System.Linq.Expressions;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages.News;

public class NewsServices : MessageService
{
    public NewsServices(SocketGuild guild) : base("News", guild) { }

    protected override Expression<Func<TextChannelMainModel, bool>> ChannelFilter()
    {
        return model => model.IsNewsChannel.Value;
    }

}