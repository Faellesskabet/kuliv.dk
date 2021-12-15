using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild.Models.JoinRole;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.Discord.Interactive;
using Dikubot.Discord.Interactive.Criterions;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;

namespace Dikubot.Discord.Command
{
	[Group("gruppe")]
	public class Group : InteractiveBase<SocketCommandContext>
	{
		
		[Command("Overview")]
		[Alias("oversigt", "overview")]
		[Summary("Se oversigten af grupper")]
		public async Task Oversigt()
		{
			await JoinRoleIndex(null);
		}

		[Command("join", RunMode = RunMode.Async)]
	    [Alias("vælg","roller", "join")]
	    [Summary("Se oversigten af grupper, du kan blive en del af.")]
		public async Task JoinRoleIndex(string? categoryNumber = null)
		{
			await ReplyAsync("This command is temporarily disabled.");
			return;
				
				SocketMessage response = null;
				IUserMessage message = null;
				await Context.Message.DeleteAsync();
				
				if (categoryNumber == null || string.Equals(categoryNumber,"Overview" ,StringComparison.InvariantCultureIgnoreCase))
				{
					var embed = BuildEmbed();
					message = await ReplyAsync(embed:embed);
					response = await NextMessageAsync();
					await message.DeleteAsync();
				}

				if (!string.IsNullOrEmpty(categoryNumber) || response != null)
				{
					int index = 0;
					try
					{
						index = Int32.Parse((response?.Content ?? categoryNumber)) - 1;
					}
					catch (FormatException e)
					{
						Console.WriteLine(e.Message);
					}

					var service = new JoinChannelCategoryServices(Context.Guild);
					var userGuildServices = new UserGuildServices(Context.Guild);
					var sUserRoles = userGuildServices.Get(this.Context.User.Id.ToString()).Roles.Select(r => r.RoleId).ToHashSet();
					var jc = service.Get().Where(j => j.Permission.Overlaps(sUserRoles)).ToList();
					var criterion = new Criteria<SocketReaction>();
					var cr = new EnsureReactionFromSourceUserCriterion();
					criterion.AddCriterion(cr);
					if (0 <= index && index < jc.Count)
					{
						var op = jc[index]
							.TextRoles
							.ToList()
							.FindAll(_ => true);
						List<JoinRoleMainModel> fields = new List<JoinRoleMainModel>();
						foreach (var variable in op)
						{
							fields.Add(variable);
						}
						joinRolesCallBack callBack = new joinRolesCallBack(Interactive,Context, Context.Channel, Context.Guild, fields,jc[index].Permission ,criterion, jc[index].Name);
						callBack.Timeout = TimeSpan.FromMinutes(1);
						await callBack.DisplayAsync().ConfigureAwait(false);
					}
					
				}

				if (response != null)
				{
					response.DeleteAsync();
				}
			}
			
			protected virtual Embed BuildEmbed()
			{
				var builder = new EmbedBuilder()
					.WithColor(Color.Gold)
					.WithFooter(f => f.Text = "")
					.WithTitle("Categoryes");
				//.WithAuthor("_pager.Author")        
            
				var service = new JoinChannelCategoryServices(Context.Guild);
					
				var jc = service.Get();
            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
            
            int n = 1;
            var userGuildServices = new UserGuildServices(Context.Guild);
  
            var sUserRoles = userGuildServices.Get(this.Context.User.Id.ToString()).Roles.Select(r => r.RoleId).ToHashSet();
            
            
            foreach (var VARIABLE in jc)
            {
	            if (VARIABLE.Permission.Overlaps(sUserRoles))
	            {
		            EmbedFieldBuilder fieldBuilder = new EmbedFieldBuilder();
		            fieldBuilder.Name = $"{n} : {VARIABLE.Name}";
		            fieldBuilder.Value = VARIABLE.Decs;
		            fields.Add(fieldBuilder);
		            n++;
	            }
            }
				builder.Description = "";
				

				builder.Fields = fields;
            

				return builder.Build();
			}
			
    }

}
	