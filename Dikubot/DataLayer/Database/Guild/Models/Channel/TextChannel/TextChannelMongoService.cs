using System;
using System.Collections.Generic;
using Dikubot.DataLayer.Database.Guild.Models.Channel.SubModels.Overwrite;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel;

/// <summary>
///     Class for for retrieving information from the TextChannel collection.
/// </summary>
public class TextChannelMongoService : ChannelMongoService<TextChannelMainModel>
{
    public TextChannelMongoService(Database database, SocketGuild guild) : base(database, guild)
    {
    }

    /// <Summary>Converts a SocketTextChannel to a TextChannelModel.</Summary>
    /// <param name="textChannel">The SocketTextChannel to be converted.</param>
    /// <return>Returns a TextChannelModel.</return>
    public TextChannelMainModel SocketToModel(SocketTextChannel textChannel)
    {
        TextChannelMainModel _textChannel = new TextChannelMainModel();
        _textChannel.DiscordId = textChannel.Id.ToString();
        _textChannel.Position = textChannel.Position;
        _textChannel.CreatedAt = textChannel.CreatedAt.DateTime;
        _textChannel.Name = textChannel.Name;
        _textChannel.DiscordCategoryId = textChannel.CategoryId.ToString();
        _textChannel.Topic = textChannel.Topic;
        _textChannel.IsNsfw = textChannel.IsNsfw;
        try
        {
            _textChannel.SlowModeInterval = textChannel.SlowModeInterval;
        }
        catch (Exception e)
        {
            // ignored
            // this is necessary due to a bug in Discord.NET which throws an exception if we try to set slowmode on a news channel
        }

        _textChannel.PermissionOverwrites = new List<OverwriteMainModel>();
        foreach (Overwrite overwrite in textChannel.PermissionOverwrites)
            _textChannel.PermissionOverwrites.Add(new OverwriteMainModel(overwrite));

        return _textChannel;
    }

    /// <Summary>Converts a RestTextChannel to a TextChannelModel.</Summary>
    /// <param name="textChannel">The RestTextChannel to be converted.</param>
    /// <return>Returns a TextChannelModel.</return>
    public TextChannelMainModel RestToModel(RestTextChannel textChannel)
    {
        TextChannelMainModel _textChannel = new TextChannelMainModel();
        _textChannel.DiscordId = textChannel.Id.ToString();
        _textChannel.Position = textChannel.Position;
        _textChannel.CreatedAt = textChannel.CreatedAt.DateTime;
        _textChannel.Name = textChannel.Name;
        _textChannel.DiscordCategoryId = textChannel.CategoryId.ToString();
        _textChannel.Topic = textChannel.Topic;
        _textChannel.IsNsfw = textChannel.IsNsfw;
        try
        {
            _textChannel.SlowModeInterval = textChannel.SlowModeInterval;
        }
        catch (Exception e)
        {
            // ignored
            // this bug happens on sockets but I'm including it here anyway
        }

        _textChannel.PermissionOverwrites = new List<OverwriteMainModel>();
        foreach (Overwrite overwrite in textChannel.PermissionOverwrites)
            _textChannel.PermissionOverwrites.Add(new OverwriteMainModel(overwrite));

        return _textChannel;
    }

    /// <Summary>Converts a TextChannelModel to a TextChannelProperties.</Summary>
    /// <param name="textChannelMainModel">The TextChannelModel to be converted.</param>
    /// <return>Returns a TextChannelProperties.</return>
    public TextChannelProperties ModelToTextChannelProperties(TextChannelMainModel textChannelMainModel)
    {
        TextChannelProperties textChannelProperties = new TextChannelProperties();
        textChannelProperties.Name = textChannelMainModel.Name;
        textChannelProperties.Position = textChannelMainModel.Position;
        textChannelProperties.CategoryId = Convert.ToUInt64(textChannelMainModel.DiscordCategoryId);
        textChannelProperties.Topic = textChannelMainModel.Topic;
        textChannelProperties.IsNsfw = textChannelMainModel.IsNsfw;
        try
        {
            textChannelProperties.SlowModeInterval = textChannelMainModel.SlowModeInterval;
        }
        catch (Exception e)
        {
            // ignored
        }

        return textChannelProperties;
    }

    public override string GetCollectionName()
    {
        return "TextChannels";
    }
}