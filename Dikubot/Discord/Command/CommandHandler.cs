using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Dikubot.Discord.Command
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;
        private readonly IServiceProvider services;
        private readonly char CommandPrefix = '!';
        
        public CommandHandler(IServiceProvider services)
        {
            this.commands = services.GetRequiredService<CommandService>();
            this.client = services.GetRequiredService<DiscordSocketClient>();
            this.services = services;

            client.MessageReceived += HandleCommand;
        }

        public async Task init()
        {
            // Register modules that are public and inherit ModuleBase<T>.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        public CommandService GetCommandService()
        {
            return commands;
        }

        private async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix(this.CommandPrefix, ref argPos) ||
                  message.HasMentionPrefix(client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(client, message);

            // Execute the command
            await commands.ExecuteAsync(context, argPos, services);
        }

        public Char GetCommandPrefix()
        {
            return CommandPrefix;
        }
        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these
            // errors
            if (!command.IsSpecified)
                return;

            // the command was successful, we don't care about this result, unless we want to log that a command
            // succeeded.
            if (result.IsSuccess)
                return;

            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync($":x: Fejl: {result}");
        }
    }
}