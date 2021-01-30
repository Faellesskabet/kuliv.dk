using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.Database.Models;
using Dikubot.Database.Models.Role;
using Dikubot.Database.Models.SubModels;
using Dikubot.Permissions;

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
            List<RoleModel> roles =  new RoleServices().Get();
            userModel.AddRole(new UserRoleModel(roles[0]) { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(12.382)});
            userModel.AddRole(new UserRoleModel(roles[1]) { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(148.32193482)});
            userModel.AddRole(new UserRoleModel(roles[2]) { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(9999.8389)});
            userModel.AddRole(new UserRoleModel(roles[3]) { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(9999.8389)});
            userModel.AddRole(new UserRoleModel(roles[4]) { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(9999.8389)});
            userServices.Upsert(userModel);
            var permissionsService = new PermissionsService(Context.Guild);
            permissionsService.SetDiscordUserRoles(userModel);
            
            await ReplyAsync(echo);
        }
    }
}
