using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Addons.Interactive;
using Dikubot.Database.Models;
using Dikubot.Database.Models.Role;
using Dikubot.Database.Models.SubModels;
using Dikubot.Permissions;
using Discord.WebSocket;

namespace Dikubot.Discord.Command
{
    public class Games {
        [Group("terning")]
        [Alias("t")]
        public class Terning : InteractiveBase<SocketCommandContext> {
            
            /// <summary>
            /// "Terning" is a DIKU game, where players create new rules everytime a unique roll is thrown.
            /// </summary>
            public class TerningGame
            {
                public static List<TerningGame> runningGames = new();
                
                public List<IUser> players;
                public IChannel channel;
                public string[] rules = new string[20];
                public int turnIndex;

                public TerningGame(IUser[] players, IChannel channel) {
                    this.players = players.ToList();
                    this.channel = channel;
                    this.rules[0] = this.rules[19] = "Tag 1 bunder";
                    this.rules[1] = this.rules[18] = "Tag 1/2 bunder";
                    this.rules[10] = "Lav en ny regel";
                }

                public IUser GetPlayer() {
                    return players[turnIndex];
                }

                public IUser NextPlayer() {
                    turnIndex = turnIndex >= players.Count-1 ? 0 : ++turnIndex;
                    return players[turnIndex];
                }

                public static TerningGame GetGame(IChannel channel) {
                    return runningGames.Find(game => game.channel == channel);
                }

                public static void NewGame(TerningGame game) {
                    runningGames.Add(game);
                }

            }

            
            
            [Command("start")]
            [Summary("Starts a game of 'terning'")]
            public async Task CreateGameAsync(params IUser[] players) {
                if(TerningGame.GetGame(Context.Channel) != null) {
                    await ReplyAsync("A game is already running in this channel!");
                    return;
                }

                // If no players are given add the User
                if (players.Length == 0)
                    players = new IUser[] {Context.User};

                // Create a new game
                var game = new TerningGame(players, Context.Channel);
                TerningGame.NewGame(game);
                
                await ReplyAsync($"Started a new game! {game.GetPlayer().Mention} it's your turn! Write `!terning roll` to play");
            }

            [Command("stop")]
            [Summary("Ends a game of 'terning'")]
            public async Task CreateGameAsync() {
                var game = TerningGame.GetGame(Context.Channel);
                if (game == null) {
                    await ReplyAsync("No game is currently running in this channel");
                    return;
                }

                TerningGame.runningGames.Remove(game);
                await ReplyAsync("Game Stopped");
            }

            [Command("addplayer")]
            [Summary("Add player to game")]
            public async Task AddPlayerAsync(IUser player) {
                var game = TerningGame.GetGame(Context.Channel);

                if (game == null) {
                    await ReplyAsync("No game is running in this channel!");
                    return;
                }

                if (game.players.Contains(player)) {
                    await ReplyAsync($"{player} is already in the game!");
                    return;
                }

                game.players.Add(player);
                await ReplyAsync($"Added: {player}");
            }
            
            [Command("join")]
            [Summary("Join running game")]
            public async Task JoinGameAsync() {
                var game = TerningGame.GetGame(Context.Channel);

                if (game == null) {
                    await ReplyAsync("No game is running in this channel!");
                    return;
                }

                if (game.players.Contains(Context.User)) {
                    await ReplyAsync($"{Context.User.Mention} is already in the game!");
                    return;
                }

                game.players.Add(Context.User);
                await ReplyAsync($"User joined: {Context.User.Mention}");
            }

            [Command("roll", RunMode = RunMode.Async)]
            [Summary("Roll the dice")]
            public async Task RollDiceAsync() {
                // Roll d20
                var rand = new Random();
                int roll = rand.Next(1, 20);

                var game = TerningGame.GetGame(Context.Channel);
                if (game == null) {
                    await ReplyAsync($"No game is currently running in this channel!");
                    return;
                }

                if (game.GetPlayer() != Context.User) {
                    await ReplyAsync($"{Context.User.Mention} It's not your turn!");
                    return;
                }

                await ReplyAsync($"{Context.User.Mention} Rolled: " + roll);
                
                if (game.rules[roll] != null) { // rule exists
                    await ReplyAsync($"{Context.User.Mention} {game.rules[roll]}");
                    await ReplyAsync($"{game.NextPlayer().Mention} it's your turn!");
                } else { // rule does not exist
                    await ReplyAsync($"{Context.User.Mention} Create a new rule (simply reply with your new rule)");
                    var response = await NextMessageAsync(timeout:new TimeSpan(0,5,0));
                    if (response != null) {
                        game.rules[roll - 1] = response.Content;
                        await ReplyAsync($"Rule added `{response.Content}`. {game.NextPlayer().Mention} it's your turn! Write `!terning roll` to play");
                    } else {
                        await ReplyAsync($"You did not reply before the timeout :cry: {game.NextPlayer().Mention} it's your turn! Write !terning roll to play");
                    }
                }
            }

            [Command("replace")]
            [Summary("Replace a rule")]
            public async Task ReplaceRuleAsync(int ruleIndex, string rule) {
                var game = TerningGame.GetGame(Context.Channel);
                if (game == null) {
                    await ReplyAsync($"No game is currently running in this channel!");
                    return;
                }
                
                var oldRule = game.rules[ruleIndex - 1];
                game.rules[ruleIndex - 1] = rule;
                
                await ReplyAsync($"Replaced ${oldRule} with ${rule}");
            }
            
            [Command("rules")]
            [Summary("Show rules")]
            public async Task ShowRulesAsync() {
                var game = TerningGame.GetGame(Context.Channel);

                if (game == null) {
                    await ReplyAsync($"No game is currently running in this channel!");
                    return;
                }
                
                var ruleString = "";
                for (int i = 0; i < game.rules.Length; i++) {
                    ruleString += $"Rule {i + 1}: {game.rules[i]}\n";
                }
                
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle("Terning - Rules");
                builder.WithDescription(ruleString);
                await Context.Channel.SendMessageAsync("", false, builder.Build());
            }
        }
    }
}