using System;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages.Quote;
using Dikubot.Discord.EventListeners;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using ContextType = Discord.Commands.ContextType;

namespace Dikubot.Discord.Command;

[global::Discord.Commands.Group("quote")]
[global::Discord.Commands.RequireContext(ContextType.Guild)]
[Alias("citat")]
public class Quote : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IGuildMongoFactory _guildMongoFactory;

    public Quote(IGuildMongoFactory guildMongoFactory)
    {
        _guildMongoFactory = guildMongoFactory;
    }

    [SlashCommand("quote", "Get a random quote from the quote channel!")]
    public async Task GetQuote()
    {
        SocketGuild guild = Context.Guild;
        MessageModel quoteModel = _guildMongoFactory.Get<QuoteMongoServices>(Context.Guild).GetSamples(1).FirstOrDefault();

        if (quoteModel == null)
        {
            await ReplyAsync("I couldn't find any quotes! :(");
            return;
        }

        IMessage quote = guild.GetTextChannel(Convert.ToUInt64(quoteModel.ChannelId))
            .GetMessageAsync(Convert.ToUInt64(quoteModel.MessageId)).Result;
        await ReplyAsync(quote.Content);
    }

    [Command("test")]
    public async Task GetQuotes()
    {
        await ReplyAsync("test " + _guildMongoFactory.Get<QuoteMongoServices>(Context.Guild).Get().FirstOrDefault()
            ?.MessageId);
    }

    [Command("connect")]
    [global::Discord.Commands.Summary("Connect a quote channel")]
    public async Task ConnectChannel([global::Discord.Commands.Summary("Channel id")] ulong channelId = 0)
    {
        if (!Util.IsMod(Context.User, Context.Guild))
        {
            await ReplyAsync("Only users with role Mod may use this command");
            return;
        }

        await ReplyAsync("Loading all quotes... This may take a while!");
        await TryUpdateChannel(channelId, true);
    }

    [Command("disconnect")]
    [global::Discord.Commands.Summary("Disconnect a quote channel")]
    public async Task DisconnectChannel([global::Discord.Commands.Summary("Channel id")] ulong channelId = 0)
    {
        if (!Util.IsMod(Context.User, Context.Guild))
        {
            await ReplyAsync("Only users with role Mod may use this command");
            return;
        }

        await TryUpdateChannel(channelId, false);
    }

    private async Task TryUpdateChannel(ulong channelId, bool connect)
    {
        if (channelId == 0) channelId = Context.Channel.Id;
        TextChannelMongoService textChannelMongoService = _guildMongoFactory.Get<TextChannelMongoService>(Context.Guild);
        TextChannelMainModel textChannelModel = textChannelMongoService.Get(model => model.DiscordId == channelId.ToString());
        if (textChannelModel == null)
        {
            await ReplyAsync("The given channelId does not match any channels.");
            return;
        }

        textChannelModel.IsQuoteChannel = connect;
        textChannelMongoService.Update(textChannelModel);

        //We add all the quotes to the database
        if (connect)
            _guildMongoFactory.Get<QuoteMongoServices>(Context.Guild)
                .DownloadMessagesFromChannel(Context.Guild.GetTextChannel(channelId));
        //We remove all the quotes from the channel
        else
            _guildMongoFactory.Get<QuoteMongoServices>(Context.Guild)
                .Remove(model => model.ChannelId == channelId.ToString());
        await ReplyAsync(
            $"The channel named {textChannelModel.Name} has been {(connect ? "connected" : "disconnected")} from quotes!");
    }
}