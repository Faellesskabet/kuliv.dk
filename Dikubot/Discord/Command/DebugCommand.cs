#if DEBUG
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Global.User;
using Discord.Commands;

namespace Dikubot.Discord.Command;

[Group("debug")]

public class DebugCommand : ModuleBase<SocketCommandContext>
{
    [Command("op")]
    [Summary("Get owner permissions in the local KULiv system")]
    public async Task OpCommand()
    {
        await ReplyAsync($"@everyone THIS COMMAND (debug op) SHOULD ONLY BE USED IN TEST DEVELOPMENT. USED BY {Context.User.Mention} ({Context.User.Id})");
        UserGlobalServices userGlobalServices = new UserGlobalServices();
        UserGlobalModel userGlobalModel = userGlobalServices.Get(Context.User);
        userGlobalModel.IsAdmin = true;
        userGlobalServices.Upsert(userGlobalModel);
        await ReplyAsync("Congrats, you can now do everything!");
    }
}
#endif