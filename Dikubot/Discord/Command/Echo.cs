using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.Database.Models;

namespace Dikubot.Discord.Command
{
    public class Echo : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        [Summary("Echoes a message.")]
        public async Task SayAsync([Remainder][Summary("The text to echo")] string echo)
        {
            UserServices userServices = new UserServices();
            UserModel userModel = userServices.Get(Context.User);
            
            userModel.Name = Context.User.Username;
            userModel.LastMessage = Context.Message.ToString();
            userServices.Insert(userModel);
            
            await ReplyAsync(echo);
        }
    }
}
