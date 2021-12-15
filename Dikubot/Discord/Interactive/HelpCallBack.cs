using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dikubot.DataLayer.Static;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;

namespace Dikubot.Discord.Interactive
{
    public class HelpCallBack : IReactionCallback
    {
        
        public RunMode RunMode { get; }
        public ICriterion<SocketReaction> Criterion { get; }

        private InteractiveOptions _options;
        public TimeSpan? Timeout { get; set; }

        public SocketCommandContext Context { get; }
        //https://github.com/foxbot/Discord.Addons.Interactive/blob/master/Discord.Addons.Interactive/Paginator/PaginatedMessageCallback.cs
        public InteractiveService  Interactive { get; private set; }
        private string[] _search { get; set; }
        private int page = 1;
        private readonly int pages;
        private int fieldMaxCount = 10;
        public IUserMessage Message { get; private set; }
        
        public HelpCallBack(InteractiveService  interactive, 
            SocketCommandContext sourceContext,string[] search, ICriterion<SocketReaction> criterion = null)
        {
            Interactive = interactive;
            Context = sourceContext;
            Criterion = criterion;
            _search = search;
            
            EmbedBuilder embedBuilder = GetHelpEmbed(_search);
            pages = (int) Math.Ceiling(embedBuilder.Fields.Count / (float) fieldMaxCount);
        }
        

        public async Task DisplayAsync()
        {
            
            var embed = BuildEmbed();
            var message = await Context.Channel.SendMessageAsync("HELP:", embed: embed).ConfigureAwait(false);
            Message = message;
            Interactive.AddReactionCallback(message, this);
            
            Emoji[] Emojis = new Emoji[]{new Emoji("⬅️"), new Emoji("➡️"), new Emoji("⏹")};

            _ = Task.Run(async () =>
            {
                await message.AddReactionsAsync(Emojis);
                var manageMessages = (Context.Channel is IGuildChannel guildChannel)
                    ? (Context.User as IGuildUser).GetPermissions(guildChannel).ManageMessages
                    : false;
            });
				
        }

        public async Task<bool> HandleCallbackAsync(SocketReaction reaction)
        {
            var emote = reaction.Emote;
            if (emote.Equals(new Emoji("⬅️")))
            {
                if (page <= 1)
                {
                    _ = Message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                    return false;
                }
                --page;
            }
            else if (emote.Equals(new Emoji("➡️")))
            {
                if (page >= pages)
                {
                    _ = Message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                    return false;
                }
                ++page;
            }else if (emote.Equals(new Emoji("⏹")))
            {
                Interactive.RemoveReactionCallback(Message);
                await Message.DeleteAsync().ConfigureAwait(false);
                return true;
            }
            // TODO: (Next major version) timeouts need to be handled at the service-level!
            if (Timeout.HasValue && Timeout.Value != null)
            {
                _ = Task.Delay(Timeout.Value).ContinueWith(_ =>
                {
                    Interactive.RemoveReactionCallback(Message);
                    _ = Message.DeleteAsync();
                });
            }
            
            _ = Message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            await RenderAsync().ConfigureAwait(false);
            return false;
        }

        
        private EmbedBuilder GetHelpEmbed(string[] args)
        {
            CommandService commandService = DiscordBot.CommandHandler.GetCommandService();
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle("Help commands");
            embedBuilder.WithColor(Color.Green);
            embedBuilder.WithTimestamp(DateTimeOffset.Now);
            embedBuilder.WithThumbnailUrl(
                "https://cdn.discordapp.com/avatars/742743929459572766/db56c85ec14e14fb44d6c71e73661ce5.png?size=256");

            List<ModuleInfo> modules = commandService.Modules.ToList();
            if (args.Length >= 1)
            {
                string command = string.Join(" ", args);
                Console.WriteLine(command);
                embedBuilder.WithTitle($"Help - {command}");
                modules = modules.Where(info =>
                    info.Name.Contains(command) || info.Commands.Any(commandInfo => commandInfo.Name.Contains(command))).ToList();
                if (modules.Count <= 0)
                {
                    embedBuilder.WithDescription("Ingen resultater");
                }
            }

            foreach (ModuleInfo module in modules)
            {
                StringBuilder description = new StringBuilder();

                // The title is an invisible character as embedBuilder doesn't allow fields with no values
                embedBuilder.Fields.Add(GetEmbedFieldBuilder(Util.INVISIBLE_CHARACTER, 
                    $"__**{module.Name.First().ToString().ToUpper() + module.Name.Substring(1)} kommandoer**__"));
                foreach (CommandInfo command in module.Commands)
                {
                    string embedFieldText = command.Summary ?? "Ingen beskrivelse tilgængelig";
                    string name = string.Equals(command.Module.Name, command.Name, StringComparison.CurrentCultureIgnoreCase) ? command.Name : $"{command.Module.Name} {command.Name}";
                    if (command.Parameters.Count > 0)
                    {
                        name += " " + string.Join(" ", command.Parameters.Select(info => $"[{info.Name}]"));
                    }
                        
                    embedBuilder.Fields.Add(GetEmbedFieldBuilder($"```{DiscordBot.CommandHandler.GetCommandPrefix()}{name.ToLower()}```", embedFieldText, true));
                }
            }
            return embedBuilder;
        }

        private EmbedFieldBuilder GetEmbedFieldBuilder(string name, string value, bool inline = false)
        {
            return new EmbedFieldBuilder() {Name = name, Value = value, IsInline = inline};
        }

        protected virtual Embed BuildEmbed()
        {
            EmbedBuilder embedBuilder = GetHelpEmbed(_search);
            embedBuilder.WithFooter($"Page {page}/{pages}");
            int pageIndex = (page - 1) * fieldMaxCount;
            int pageCount = page * fieldMaxCount + (page - 1) * fieldMaxCount > embedBuilder.Fields.Count - 1
                ? embedBuilder.Fields.Count - 1 - (page - 1) * fieldMaxCount
                : page * fieldMaxCount;
            embedBuilder.Fields = embedBuilder.Fields.GetRange(pageIndex, pageCount);
            return embedBuilder.Build();
            
        }

        private async Task RenderAsync()
        {
            var embed = BuildEmbed();
            await Message.ModifyAsync(m => m.Embed = embed).ConfigureAwait(false);
        }
        
        
       
    }

    
}