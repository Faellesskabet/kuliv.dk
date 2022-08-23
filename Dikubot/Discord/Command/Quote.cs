using System;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages.Quote;
using Dikubot.Discord.EventListeners;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Dikubot.Discord.Command
{
    [Group("quote")]
    [RequireContext(ContextType.Guild)]
    [Alias("citat")]
    public class Quote : ModuleBase<SocketCommandContext>
    {
        [Command("")]
        [Alias("")]
        [Summary("Get a random quote from the quote channel")]
        public async Task GetQuote()
        {
            SocketGuild guild = Context.Guild;
            MessageModel quoteModel = new QuoteMongoServices(Context.Guild).GetSamples(1).FirstOrDefault();

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
            await ReplyAsync("test " + new QuoteMongoServices(Context.Guild).Get().FirstOrDefault()?.MessageId);
        }

        [Command("connect")]
        [Summary("Connect a quote channel")]
        public async Task ConnectChannel([Summary("Channel id")] ulong channelId = 0)
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
        [Summary("Disconnect a quote channel")]
        public async Task DisconnectChannel([Summary("Channel id")] ulong channelId = 0)
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
            if (channelId == 0)
            {
                channelId = (ulong) this.Context.Channel.Id;
            }
            TextChannelMongoService textChannelMongoService = new TextChannelMongoService(this.Context.Guild);
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
            {
                new QuoteMongoServices(this.Context.Guild).DownloadMessagesFromChannel(Context.Guild.GetTextChannel(channelId));
            }
            //We remove all the quotes from the channel
            else
            {
                new QuoteMongoServices(this.Context.Guild).Remove(model => model.ChannelId == channelId.ToString());
            }
            await ReplyAsync($"The channel named {textChannelModel.Name} has been {(connect?"connected":"disconnected")} from quotes!");
        } 
        
    }
}