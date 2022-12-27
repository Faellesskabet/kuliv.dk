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

    public MessageMongoService(Database database, IGuildMongoFactory guildMongoFactory, SocketGuild guild,
        MessageListener messageListener) : base(
        database, guild)
    {
        _guildMongoFactory = guildMongoFactory;
        _messageListener = messageListener;
    }

    public IEnumerable<CreateIndexModel<MessageModel>> GetIndexes()
    {
        CreateIndexOptions options = new CreateIndexOptions { Unique = true };
        return new List<CreateIndexModel<MessageModel>>
        {
            new(
                Builders<MessageModel>.IndexKeys.Ascending(model => model.TimeStamp)
                    .Ascending(model => model.MessageId), options)
        };
    }

    public MessageModel SocketToModel(SocketMessage textMessage)
    {
        MessageModel messageModel = new MessageModel();
        messageModel.MessageId = textMessage.Id.ToString();
        messageModel.ChannelId = textMessage.Channel.Id.ToString();
        messageModel.TimeStamp = textMessage.Timestamp;
        return messageModel;
    }

    public MessageModel IMessageToModel(IMessage textMessage)
    {
        MessageModel messageModel = new MessageModel();
        messageModel.MessageId = textMessage.Id.ToString();
        messageModel.ChannelId = textMessage.Channel.Id.ToString();
        messageModel.TimeStamp = textMessage.Timestamp;
        return messageModel;
    }

    public async void DownloadMessagesFromChannel(SocketTextChannel textChannel)
    {
        if (textChannel is not SocketGuildChannel channel) return;
        SocketGuild guild = channel.Guild;
        TextChannelMongoService textChannelMongoService = _guildMongoFactory.Get<TextChannelMongoService>(guild);
        TextChannelMainModel textChannelMainModel =
            textChannelMongoService.Get(model => model.DiscordId == channel.Id.ToString());
        if (textChannelMainModel == null) return;

        List<MessageModel> messages = new();
        IAsyncEnumerable<IMessage> asyncEnumerator = textChannel.GetMessagesAsync(int.MaxValue).Flatten();
        await foreach (IMessage message in asyncEnumerator.WithCancellation(default).ConfigureAwait(false))
        {
            if (Exists(model => model.MessageId == message.Id.ToString())) continue;
            _messageListener.ProcessMessage(message);
        }
    }

    public void DownloadAllMessages()
    {
        TextChannelMongoService textChannelMongoService = _guildMongoFactory.Get<TextChannelMongoService>(Guild);
        List<TextChannelMainModel> channels = textChannelMongoService.GetAll(ChannelFilter());
        foreach (TextChannelMainModel channel in channels)
            DownloadMessagesFromChannel(Guild.GetTextChannel(Convert.ToUInt64(channel.DiscordId)));
    }

    protected abstract Expression<Func<TextChannelMainModel, bool>> ChannelFilter();
}