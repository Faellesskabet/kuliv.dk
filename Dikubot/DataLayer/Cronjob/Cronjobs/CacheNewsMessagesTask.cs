using System;
using System.Collections.Generic;
using Cronos;
using Dikubot.DataLayer.Caching;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages.News;
using Dikubot.DataLayer.Static;
using Dikubot.Discord;
using Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Cronjob.Cronjobs;

public class CacheNewsMessagesTask: CronTask
{

    private readonly Cache<MessageModel, IMessage> _cache;
    private readonly IGuildMongoFactory _guildMongoFactory;
    
    public CacheNewsMessagesTask(Cache<MessageModel, IMessage> cache, IGuildMongoFactory guildMongoFactory)
    {
        _cache = cache;
        _guildMongoFactory = guildMongoFactory;
    }

    // */30 * * * *
    /// <summary>
    /// Caches all the latest news messages every 30 minute
    /// </summary>
    protected override CronExpression CronExpression()
    {
        return Cronos.CronExpression.Parse("*/30 * * * *");
    }

    public override void RunTask()
    {
        Logger.Debug("Caching latest news");
        foreach (SocketGuild guild in DiscordBot.ClientStatic.Guilds)
        {
            NewsMongoServices newsMongoServices = _guildMongoFactory.Get<NewsMongoServices>(guild);
            List<MessageModel> messageModels = newsMongoServices.Get(20);
            foreach (MessageModel messageModel in messageModels)
            {
                if (_cache.ContainsKey(messageModel))
                {
                    continue;
                }

                IMessage message = guild.GetTextChannel(Convert.ToUInt64(messageModel.ChannelId))
                    .GetMessageAsync(Convert.ToUInt64(messageModel.MessageId)).Result;
                
                if (message == null)
                {
                    newsMongoServices.Remove(messageModel);
                    continue;
                }

                _cache[messageModel] = message;
            }
        }
        Logger.Debug("Finished caching latest news");
    }
}