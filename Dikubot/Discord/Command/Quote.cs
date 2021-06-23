using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.Database.Models.TextChannel;
using Dikubot.DataLayer.Static;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Graph;
using Util = Dikubot.Discord.EventListeners.Util;

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
            TextChannelServices textChannelServices = new TextChannelServices(this.Context.Guild);
            List<TextChannelModel> textChannelModels = textChannelServices.GetAll(model => model.IsQuoteChannel);

            List<SocketTextChannel> textChannels = textChannelModels.Select(model => Context.Guild.GetTextChannel(Convert.ToUInt64(model.DiscordId))).ToList();
            if (!textChannels.Any())
            {
                await ReplyAsync("There are no quote channels:(");
                return;
            }
            List<IMessage> messages = new List<IMessage>();
            foreach (SocketTextChannel textChannel in textChannels)
            {
                var asyncEnumerator = textChannel.GetMessagesAsync(100).Flatten();
                await foreach (var message in asyncEnumerator.WithCancellation(default).ConfigureAwait(false))
                {
                    messages.Add(message);
                }
            }

            int index = new Random().Next(messages.Count - 1);
            await ReplyAsync(messages[index].Content);
        }
        
        [Command("connect")]
        [Summary("Connect a quote channel")]
        public async Task ConnectChannel([Summary("Channel id")] long channelId = 0)
        {
            if (!Util.IsMod(Context.User, Context.Guild))
            {
                await ReplyAsync("Only users with role Mod may use this command");
                return;
            }
            await TryUpdateChannel(channelId, true);
        }
        
        [Command("disconnect")]
        [Summary("Disconnect a quote channel")]
        public async Task DisconnectChannel([Summary("Channel id")] long channelId = 0)
        {
            if (!Util.IsMod(Context.User, Context.Guild))
            {
                await ReplyAsync("Only users with role Mod may use this command");
                return;
            }
            await TryUpdateChannel(channelId, false);
        }

        private async Task TryUpdateChannel(long channelId, bool connect)
        {
            if (channelId == 0)
            {
                channelId = (long) this.Context.Channel.Id;
            }
            TextChannelServices textChannelServices = new TextChannelServices(this.Context.Guild);
            TextChannelModel textChannelModel = textChannelServices.Get(model => model.DiscordId == channelId.ToString());
            if (textChannelModel == null)
            {
                await ReplyAsync("The given channelId does not match any channels.");
                return;
            }
            textChannelModel.IsQuoteChannel = connect;
            textChannelModel.IsNsfw = false;
            textChannelServices.Update(textChannelModel);
            await ReplyAsync($"The channel named {textChannelModel.Name} has been {(connect?"connected":"disconnected")} from quotes!");
        } 
        
    }
}