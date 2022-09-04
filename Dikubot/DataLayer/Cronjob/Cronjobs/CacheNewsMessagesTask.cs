using System;
using System.Collections.Generic;
using Dikubot.DataLayer.Caching;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages.News;
using Dikubot.DataLayer.Static;
using Dikubot.Discord;
using Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Cronjob.Cronjobs;

public class CacheNewsMessagesTask: CronTask
{

    private static Cache<MessageModel, IMessage> _cache;
    // */30 * * * *
    /// <summary>
    /// Caches all the latest news messages every 30 minute
    /// </summary>
    public CacheNewsMessagesTask(Cache<MessageModel, IMessage> cache) : base(
        Cronos.CronExpression.Parse("*/30 * * * *"), Cache)
    {
        _cache = cache;
    }

    private static void Cache()
    {
        Logger.Debug("Caching latest news");
        foreach (SocketGuild guild in DiscordBot.ClientStatic.Guilds)
        {
            NewsServices newsServices = new NewsServices(guild);
            List<MessageModel> messageModels = newsServices.Get(20);
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
                    newsServices.Remove(messageModel);
                    continue;
                }

                _cache[messageModel] = message;
            }
        }
        Logger.Debug("Finished caching latest news");
    }
}