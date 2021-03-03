using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dikubot.Database.Models;
using Dikubot.DataLayer.Logic.User;
using Dikubot.Webapp.Shared;
using Dikubot.Webapp.Shared.Login;
using Discord;
using Discord.Commands;

namespace Dikubot.Discord.Command
{
[Group("Minecraft")]
[Alias("Mc")]
    public class Minecraft : ModuleBase<SocketCommandContext>
    {
        // !Mincraft join -> "Skriv i minecraft `/mcjoin 15ag6`"
        	[Command("join")]
        	[Summary("Whitelist on the Minecraft Server and connects discord ID with Minecraft ID")]
        	public async Task MinecraftJoin(string code = null)
        	{
	            var userInfo = Context.Client.CurrentUser;
	            string message = "";
	            
	            if (code != null)
	            {
		            message = userInfo.Username + " er nu Whitelist og din bruger Discord og Mincraft er sync";
		            await ReplyAsync(message);
	            }
	            else
	            {
		            message = userInfo.Username + "Du har fået en privat besked";
		            
		            await Context.User.SendMessageAsync("Skriver i minecraft `/mcjoin 15ag6`");
		            await ReplyAsync(message);
	            }
            }
	    // !Mincraft leave -> "User now removed for whitlist, and removed discord ID from the Minecraft Server"
            [Command("leave")]
            [Summary("remove Whitelist on the Minecraft Server and disconnects discord ID with Minecraft ID")]
            public async Task MinecraftLeave()
            {
	            var userInfo = Context.Client.CurrentUser;
	            string message = "";
	            
	            message = userInfo.Username + " Du er fjernet fra Whitlist og vi har disconnects discord ID og Minecraft ID på Serveren";
	            await ReplyAsync(message);
	            await ReplyAsync("Du har fået en privat besked");
            }
	    
	    
    }
}