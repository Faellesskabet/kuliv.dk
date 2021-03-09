using System;
using System.Linq;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using DynamicData.Kernel;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace Dikubot.Discord.Command {
    
    /// <summary>
    /// Music Commands. Requires Lavalink server to be running
    /// </summary>
    [Group("Music")]
    public class Music : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode lavaNode;

        public Music(LavaNode lavaNode) {
            this.lavaNode = lavaNode;
        }

        [Command("join", RunMode = RunMode.Async)]
        [Summary("Join voicechannel of User")]
        public async Task JoinAsync() 
        {
            // Dont allow hijacking, the bot can only play in one voice channel at a time
            if (lavaNode.HasPlayer(Context.Guild)) {
                await ReplyAsync("Already connected to a voice channel!");
                return;
            }
            
            // We can only join if the requesting User is currently in voice channel
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null) {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            // Join or fail
            try  {
                await lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
            } catch (Exception exception) {
                await ReplyAsync(exception.Message);
            }
        }

        [Command("play", RunMode = RunMode.Async)]
        [Summary("Play music by search. If music is already playing queue it")]
        public async Task PlayAsync([Remainder] string query) 
        {
            // Handle empty search
            if (string.IsNullOrWhiteSpace(query)) {
                await ReplyAsync("Please provide search terms.");
                return;
            }   

            // We can't leave if we are not in a voice channel
            // TODO: If the bot crashes it for some reason thinks it's still in a channel
            if (!lavaNode.HasPlayer(Context.Guild)) {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }
            
            // Search
            var searchResponse = await lavaNode.SearchYouTubeAsync(query);
            if (searchResponse.LoadStatus == LoadStatus.LoadFailed || searchResponse.LoadStatus == LoadStatus.NoMatches) {
                await ReplyAsync($"Unable to find anything for `{query}`.");
                return; 
            }

            // If Playing or paused simply queue the song
            var player = lavaNode.GetPlayer(Context.Guild);
            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused) {
                var track = searchResponse.Tracks[0];
                player.Queue.Enqueue(track);
                
                var thumbnail = track.FetchArtworkAsync();
                var enqueued = $"{track.Author} :: {track.Title} :: {track.Duration}";
                var embed = new EmbedBuilder()
                    .AddField("Enqueued", enqueued)
                    .WithThumbnailUrl(thumbnail.ToString());
                
                await ReplyAsync(embed: embed.Build());
            } else { // Play
                var track = searchResponse.Tracks[0];
                
                // Artwork is nice
                var thumbnail = track.FetchArtworkAsync();
                var currentTrack = $"{track.Author} :: {track.Title} :: {track.Duration}";
                var embed = new EmbedBuilder()
                    .AddField("Now Playing", currentTrack)
                    .WithThumbnailUrl(thumbnail.ToString());
                
                await ReplyAsync(embed: embed.Build());
                await player.PlayAsync(track);
            }
        }

        [Command("skip", RunMode = RunMode.Async)]
        [Summary("Skip current song")]
        public async Task SkipAsync()
        {
            var player = lavaNode.GetPlayer(Context.Guild);
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null || !lavaNode.HasPlayer(Context.Guild) || voiceState.VoiceChannel != player.VoiceChannel) {
                await ReplyAsync("You are either not connected to a voice channel, I'm not connected to a voice channel, or we are not in the same voice channel...!");
                return;
            }

            if (player.Queue.Count == 0) {
                await ReplyAsync("There's no songs left to skip");
                return;
            }

            await player.SkipAsync();
            
            // Fetch artwork for next song
            var playerTrack = player.Track;
            var thumbnail = playerTrack.FetchArtworkAsync();
            var track = $"{playerTrack.Author} :: {playerTrack.Title} :: {playerTrack.Duration}";
            var embed = new EmbedBuilder()
                .AddField("Now Playing", track)
                .WithThumbnailUrl(thumbnail.ToString());
                
            await ReplyAsync(embed: embed.Build());
        }
        
        [Command("pause", RunMode = RunMode.Async)]
        [Summary("Pause music player")]
        public async Task PauseAsync()
        {
            var player = lavaNode.GetPlayer(Context.Guild);
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null || !lavaNode.HasPlayer(Context.Guild) || voiceState.VoiceChannel != player.VoiceChannel) {
                await ReplyAsync("You are either not connected to a voice channel, I'm not connected to a voice channel, or we are not in the same voice channel...!");
                return;
            }

            if (player.PlayerState == PlayerState.Paused || player.PlayerState == PlayerState.Stopped) {
                await ReplyAsync("The music is already paused");
                return;
            }
            
            // Show artwork
            var playerTrack = player.Track;
            var thumbnail = playerTrack.FetchArtworkAsync();
            var paused = $"{playerTrack.Author} :: {playerTrack.Title} :: {playerTrack.Duration}";
            var embed = new EmbedBuilder()
                .AddField("Paused", paused)
                .WithThumbnailUrl(thumbnail.ToString());
                
            await ReplyAsync(embed: embed.Build());
            
            await player.PauseAsync();
        }
        
        
        [Command("resume", RunMode = RunMode.Async)]
        [Summary("Resume audio playback")]
        public async Task ResumeAsync()
        {
            var player = lavaNode.GetPlayer(Context.Guild);
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null || !lavaNode.HasPlayer(Context.Guild) || voiceState.VoiceChannel != player.VoiceChannel) {
                await ReplyAsync("You are either not connected to a voice channel, I'm not connected to a voice channel, or we are not in the same voice channel...!");
                return;
            }

            if (player.PlayerState == PlayerState.Playing) {
                await ReplyAsync("The music is already playing");
                return;
            }
            
            // Show artwork
            var playerTrack = player.Track;
            var thumbnail = playerTrack.FetchArtworkAsync();
            var resumed = $"{playerTrack.Author} :: {playerTrack.Title} :: {playerTrack.Duration}";
            var embed = new EmbedBuilder()
                .AddField("Resumed", resumed)
                .WithThumbnailUrl(thumbnail.ToString());
                
            await ReplyAsync(embed: embed.Build());
            
            await player.ResumeAsync();
        }
        
        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveAsync()
        {
            var player = lavaNode.GetPlayer(Context.Guild);
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null || !lavaNode.HasPlayer(Context.Guild) || voiceState.VoiceChannel != player.VoiceChannel) {
                await ReplyAsync("You are either not connected to a voice channel, I'm not connected to a voice channel, or we are not in the same voice channel...!");
                return;
            }
            
            await lavaNode.LeaveAsync(player.VoiceChannel);
        }
        
        [Command("clear", RunMode = RunMode.Async)]
        public async Task ClearAsync()
        {
            var player = lavaNode.GetPlayer(Context.Guild);
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null || !lavaNode.HasPlayer(Context.Guild) || voiceState.VoiceChannel != player.VoiceChannel) {
                await ReplyAsync("You are either not connected to a voice channel, I'm not connected to a voice channel, or we are not in the same voice channel...!");
                return;
            }

            await ReplyAsync("Cleared the queue");
            player.Queue.Clear();
        }
        
        [Command("queue")]
        public async Task QueueAsync(int page = 1)
        {
            var player = lavaNode.GetPlayer(Context.Guild);
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null || !lavaNode.HasPlayer(Context.Guild) || voiceState.VoiceChannel != player.VoiceChannel) {
                await ReplyAsync("You are either not connected to a voice channel, I'm not connected to a voice channel, or we are not in the same voice channel...!");
                return;
            }

            var builder = new EmbedBuilder();
            var stringBuilder = new StringBuilder();
            if (player.Queue.Count >= 1)
            {

                LavaTrack[] tracks = player.Queue.AsArray();
                int itemsPerPage = 20;
                for (int i = itemsPerPage*(page-1); i < tracks.Length; i++) 
                {
                    LavaTrack item = tracks[i];
                    stringBuilder.AppendLine($"{item.Author} :: {item.Title} :: {item.Duration}\n");
                }
                builder.WithFooter($"Page ${page}/{(tracks.Length / itemsPerPage)}");

                builder.AddField("------ Currently Playing ------\n",
                    $"{player.Track.Author} :: {player.Track.Title} :: {player.Track.Duration}");
                builder.AddField("------ Tracks ------\n", stringBuilder.ToString());
                var buildEmbed = builder.Build();
                await ReplyAsync(embed: buildEmbed);
            } else if (player.Queue.Count == 0) {
                builder = new EmbedBuilder().AddField("------ Currently Playing ------\n",
                    $"{player.Track.Author} :: {player.Track.Title} :: {player.Track.Duration}");
                var embed = builder.Build();
                await ReplyAsync(embed: embed);
            }
        }
        
        /// <summary>
        /// Handle auto play of song queue
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task OnTrackEnded(TrackEndedEventArgs args) 
        {
            if (!args.Reason.ShouldPlayNext()) {
                return;
            }

            var player = args.Player;
            if (!player.Queue.TryDequeue(out var queueable)) {
                await player.TextChannel.SendMessageAsync("Queue completed!");
                return;
            }

            if (!(queueable is { } track)) {
                return;
            }

            await args.Player.PlayAsync(track);
            await args.Player.TextChannel.SendMessageAsync($"{args.Reason}: {args.Track.Title}\nNow playing: {track.Title}");
        }
    }
    
}