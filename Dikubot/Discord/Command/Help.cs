using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorFluentUI;
using Discord;
using Discord.Commands;

namespace Dikubot.Discord.Command
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Alias("hjælp", "man", "howto")]
        [Summary("Get help!")]
        public async Task HelpCommand([Summary("The arguments")] params string[] args)
        {
            CommandService commandService = DiscordBot.commandHandler.GetCommandService();
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle("Help commands");
            embedBuilder.WithColor(Color.Green);
            
            //trick people into thinking I bothered doing pagination
            embedBuilder.WithFooter("Page 1/1");
            embedBuilder.WithTimestamp(DateTimeOffset.Now);

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
                embedBuilder.AddField("⠀", 
                    $"__**{module.Name.First().ToString().ToUpper() + module.Name.Substring(1)} kommandoer**__");
                
                foreach (CommandInfo command in module.Commands)
                {
                    string embedFieldText = command.Summary ?? "Ingen beskrivelse tilgængelig";
                    string name = string.Equals(command.Module.Name, command.Name, StringComparison.CurrentCultureIgnoreCase) ? command.Name : $"{command.Module.Name} {command.Name}";
                    if (command.Parameters.Count > 0)
                    {
                        name += " *" + string.Join(" ", command.Parameters.Select(info => $"[{info.Name}]")) + "*";
                    }
                    embedBuilder.AddField($"{DiscordBot.commandHandler.GetCommandPrefix()}{name.ToLower()}", embedFieldText, true);
                }
            }

            await ReplyAsync("Her er en liste af kommandoer og beskrivelser: ", false, embedBuilder.Build());

        }
    }
}