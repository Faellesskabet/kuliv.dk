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

public abstract class MessageMongoService : GuildMongoService<MessageModel>, IIndexed<MessageModel>
{
    private readonly IGuildMongoFactory _guildMongoFactory;
    private readonly MessageListener _messageListener;
    public MessageMongoService(Database database, IGuildMongoFactory guildMongoFactory, SocketGuild guild, MessageListener messageListener) : base(
        database, guild)
    {
        _guildMongoFactory = guildMongoFactory;
        _messageListener = messageListener;
    }
        
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
        TextChannelMongoService textChannelMongoService = _guildMongoFactory.Get<TextChannelMongoService>(guild);
        TextChannelMainModel textChannelMainModel = 
            textChannelMongoService.Get(model => model.DiscordId == channel.Id.ToString());
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
            _messageListener.ProcessMessage(message);
        }
    }

    public void DownloadAllMessages()
    {
        TextChannelMongoService textChannelMongoService =  _guildMongoFactory.Get<TextChannelMongoService>(Guild);
        List<TextChannelMainModel> channels = textChannelMongoService.GetAll(ChannelFilter());
        foreach (var channel in channels)
        {
            DownloadMessagesFromChannel(Guild.GetTextChannel( Convert.ToUInt64(channel.DiscordId)));
        }
    }

    protected abstract Expression<Func<TextChannelMainModel, bool>> ChannelFilter();
    public IEnumerable<CreateIndexModel<MessageModel>> GetIndexes()
    {
        var options = new CreateIndexOptions() { Unique = true };
        return new List<CreateIndexModel<MessageModel>>
        {
            new CreateIndexModel<MessageModel>(Builders<MessageModel>.IndexKeys.Ascending(model => model.TimeStamp).Ascending(model => model.MessageId), options)
        };
    }
}