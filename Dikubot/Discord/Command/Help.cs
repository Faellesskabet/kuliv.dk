using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
namespace Dikubot.Discord.Command
{
public class Help : ModuleBase<SocketCommandContext>
    {


        [Command("help")]
        [Alias("hjælp", "man", "howto")]
        [Summary("Get help with a specific command")]
        public async Task HelpCommand([Summary("Search arguments")] params string[] search)
        {
            await HelpCommand(1, search);
        }
        
        [Command("help")]
        [Alias("hjælp", "man", "howto")]
        [Summary("Access other help pages")]
        public async Task HelpCommand(int page = 1, [Summary("Search arguments")] params string[] search)
        {

            EmbedBuilder embedBuilder = GetHelpEmbed(search);
            embedBuilder.WithFooter($"Page {page}/{Math.Ceiling(embedBuilder.Fields.Count / 25.0)}");
            int pageIndex = (page - 1) * 25;
            int pageCount = page * 25 + (page - 1) * 25 > embedBuilder.Fields.Count-1 ? embedBuilder.Fields.Count-1 - (page - 1) * 25 : page * 25;
            embedBuilder.Fields = embedBuilder.Fields.GetRange(pageIndex, pageCount);
            Embed embed = embedBuilder.Build();
            await ReplyAsync("Her er en liste af kommandoer og beskrivelser: ", false, embed);

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
                embedBuilder.Fields.Add(GetEmbedFieldBuilder("⠀", 
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
    }
}