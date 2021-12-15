using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild.Models.JoinRole;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.DataLayer.Database.Guild.Models.User.SubModels;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;

namespace Dikubot.Discord.Interactive
{
    public class joinRolesCallBack : IReactionCallback
    {

        
        
        public RunMode RunMode { get; }
        public ICriterion<SocketReaction> Criterion { get; }
        public TimeSpan? Timeout { get; set; }
        public SocketCommandContext Context { get; set; }
        public string StopEmoji = "⏹";
        public ISocketMessageChannel SocketChannel { get; }
        public SocketGuild SocketGuild { get; }

        //https://github.com/foxbot/Discord.Addons.Interactive/blob/master/Discord.Addons.Interactive/Paginator/PaginatedMessageCallback.cs
        public InteractiveService Interactive { get; private set; }
        private List<JoinRoleMainModel> _fieldBuilders { get; set; }
        private string _title { get; set; }

        private HashSet<Guid> _permersions;
        public IUserMessage Message { get; private set; }
        
        
        //TODO: INTERACTIVE, SKAL VÆRE STATIC ELLER CONS, I ADMIN TING TING, SÅ MAN KAN FJERNE GAMLE INTERACTION.
        public joinRolesCallBack(InteractiveService  interactive, 
            SocketCommandContext sourceContext,
            ISocketMessageChannel ctxChannel,
            SocketGuild ctxGuild, List<JoinRoleMainModel> fieldBuilders, HashSet<Guid> permersions,
            ICriterion<SocketReaction> criterion = null, string title = "Roller")
        {
            Interactive = interactive;
            SocketChannel = ctxChannel;
            SocketGuild = ctxGuild;
            if (sourceContext != null)
            {
                Context = sourceContext;
            }
            Criterion = criterion;
            _fieldBuilders = fieldBuilders;
            _title = title;
            _permersions = permersions;
            //Message = message; , IUserMessage message
        }
        

        public async Task DisplayAsync()
        {
            
            var embed = BuildEmbed();
            var message = await SocketChannel.SendMessageAsync("Reakt for at få tildelt en rolle", embed: embed).ConfigureAwait(false);
            Message = message;
            Interactive.AddReactionCallback(message, this);
            
            List<Emoji> Emojis = new List<Emoji>();
            _fieldBuilders.ForEach(j => Emojis.Add(new Emoji(j.Emoji)));
            if (!string.IsNullOrWhiteSpace(StopEmoji))
            {
                Emojis.Add(new Emoji(StopEmoji));
            }
            
            _ = Task.Run(async () =>
            {
                await message.AddReactionsAsync(Emojis.ToArray());
            });
				
        }

        public async Task UpdateOrReactive(string messageId)
        {
            Message = SocketChannel?.GetMessageAsync(ulong.Parse(messageId)).Result as IUserMessage;
            if (Message == null)
            {
                return;
            }
            try
            {
                Interactive.RemoveReactionCallback(Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            Interactive.AddReactionCallback(Message, this);




            List<Emoji> Emojis = new List<Emoji>();
            _fieldBuilders.ForEach(j => Emojis.Add(new Emoji(j.Emoji)));
            if (!string.IsNullOrWhiteSpace(StopEmoji))
            {
                Emojis.Add(new Emoji(StopEmoji));
            }
            _ = Task.Run(async () =>
            {
                await Message.RemoveAllReactionsAsync();
                _ = Task.Run(async () =>
                {
                    await Message.AddReactionsAsync(Emojis.ToArray());
                });
            });
           
            
            var embed = BuildEmbed();
            await Message.ModifyAsync(m => m.Embed = embed).ConfigureAwait(false);
            
        }
        

        public async Task<bool> HandleCallbackAsync(SocketReaction reaction)
        {
            var emote = reaction.Emote;
            //var embed = BuildEmbed();
            if (!string.IsNullOrWhiteSpace(StopEmoji))
            {
                if (emote.Equals(new Emoji(StopEmoji)))
                {
                    Interactive.RemoveReactionCallback(Message);
                    await Message.DeleteAsync().ConfigureAwait(false);
                    return true;
                } 
            }
            
            _fieldBuilders.ForEach(async f =>
            {
                if (emote.Equals(new Emoji(f.Emoji)))
                {
                    var user = reaction.User.Value;
                    var role = SocketGuild.Roles.FirstOrDefault(r => r.Id.ToString().Equals(f.RoleId));

                    var UserService = new UserGuildServices(SocketGuild);
                    
                    var userRoles = UserService.Get(user.Id.ToString()).Roles.Select(r => r.RoleId).ToHashSet();
                    
                    if (userRoles.Overlaps(_permersions))
                    {
                        await (user as IGuildUser).AddRoleAsync(role);
                        try
                        {
                            var MU = UserService.Get(user.Id.ToString());
                            var NewUserRole = new UserRoleModel(f.Id);
                            if (MU.AddRole(NewUserRole))
                            {
                                UserService.Update(MU);
                            };
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        
                    }
                    
                    return;
                };
            });
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

        
        protected virtual Embed BuildEmbed()
        {
            var builder = new EmbedBuilder()
                .WithColor(Color.Gold)
                .WithFooter(f => f.Text = "")
                .WithTitle(_title);
                //.WithAuthor("_pager.Author")        
            
                builder.Description = "";
                
                List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
                _fieldBuilders.ForEach(f => fields.Add(f.Field(SocketGuild)));
                
                builder.Fields = fields;
            

            return builder.Build();
        }
        private async Task RenderAsync()
        {
            var embed = BuildEmbed();
            await Message.ModifyAsync(m => m.Embed = embed).ConfigureAwait(false);
        }
        
        
       
    }

    
}