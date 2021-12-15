using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dikubot.DataLayer.Static;
using Dikubot.Discord.Interactive;
using Dikubot.Discord.Interactive.Criterions;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;

namespace Dikubot.Discord.Command
{
public class Help : InteractiveBase<SocketCommandContext>
    {

        [Command("help")]
        [Alias("hjælp", "man", "howto")]
        [Summary("Get help with a specific command")]
        public async Task HelpCommand([Summary("Search arguments")] params string[] search)
        {
            await HelpCommand(1, search);
        }
        
        /// <summary>
        /// ALT OPDATERING AF DENNE SKAL FOREGÅ I Discord/Interactive/HelpCallBack.cs
        /// </summary>
        /// <param name="page"></param>
        /// <param name="search"></param>
        [Command("help")]
        [Alias("hjælp", "man", "howto")]
        [Summary("Access other help pages")]
        public async Task HelpCommand(int page = 1, [Summary("Search arguments")] params string[] search)
        {

            var criterion = new Criteria<SocketReaction>();
            criterion.AddCriterion(new EnsureReactionFromSourceUserCriterion());
            HelpCallBack callBack = new HelpCallBack(Interactive,Context, search, criterion);
            await callBack.DisplayAsync().ConfigureAwait(false);
            
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
                    $":arrow_forward: __**{module.Name.First().ToString().ToUpper() + module.Name.Substring(1)} kommandoer**__"));
                foreach (CommandInfo command in module.Commands)
                {
                    string embedFieldText = command.Summary ?? "Ingen beskrivelse tilgængelig";
                    string name = string.Equals(command.Module.Name, command.Name, StringComparison.CurrentCultureIgnoreCase) ? command.Name : $"{command.Module.Name} {command.Name}";
                    if (command.Parameters.Count > 0)
                    {
                        name += " " + string.Join(" ", command.Parameters.Select(info => $"[{info.Name}]"));
                    }

                    if (string.IsNullOrWhiteSpace(embedFieldText))
                    {
                        continue;
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
    }
}