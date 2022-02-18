using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Interfaces;
using Dikubot.Discord.EventListeners;
using Discord;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages;

public abstract class MessageService : GuildServices<MessageModel>, IIndexed<MessageModel>
{
    public MessageService(string collection, SocketGuild guild) : base(collection, guild) { }
        
    public MessageModel SocketToModel(SocketMessage textMessage)
    {
        var messageModel = new MessageModel();
        messageModel.MessageId = textMessage.Id.ToString();
        messageModel.ChannelId = textMessage.Channel.Id.ToString();
        messageModel.TimeStamp = textMessage.Timestamp;
        return messageModel;
    }

    public MessageModel IMessageToModel(IMessage textMessage)
    {
        var messageModel = new MessageModel();
        messageModel.MessageId = textMessage.Id.ToString();
        messageModel.ChannelId = textMessage.Channel.Id.ToString();
        messageModel.TimeStamp = textMessage.Timestamp;
        return messageModel;
    }

    public async void DownloadMessagesFromChannel(SocketTextChannel textChannel)
    {
        if (textChannel is not SocketGuildChannel channel)
        {
            return;
        }
        SocketGuild guild = channel.Guild;
        TextChannelServices textChannelServices = new TextChannelServices(guild);
        TextChannelMainModel textChannelMainModel = 
            textChannelServices.Get(model => model.DiscordId == channel.Id.ToString());
        if (textChannelMainModel == null)
        {
            return;
        }
            
        List<MessageModel> messages = new List<MessageModel>();
        var asyncEnumerator = textChannel.GetMessagesAsync(Int32.MaxValue).Flatten();
        await foreach (var message in asyncEnumerator.WithCancellation(default).ConfigureAwait(false))
        {
            if (Exists(model => model.MessageId == message.Id.ToString()))
            {
                continue;
            }
            MessageListener.ProcessMessage(message);
        }
    }

    public void DownloadAllMessages()
    {
        TextChannelServices textChannelServices = new TextChannelServices(Guild);
        List<TextChannelMainModel> channels = textChannelServices.GetAll(ChannelFilter());
        foreach (var channel in channels)
        {
            DownloadMessagesFromChannel(Guild.GetTextChannel( Convert.ToUInt64(channel.DiscordId)));
        }
    }

    protected abstract Expression<Func<TextChannelMainModel, bool>> ChannelFilter();
    public IEnumerable<IndexKeysDefinition<MessageModel>> GetIndexes()
    {
        return new List<IndexKeysDefinition<MessageModel>>
        {
            Builders<MessageModel>.IndexKeys.Ascending(model => model.TimeStamp).Ascending(model => model.MessageId)
        };
    }
}