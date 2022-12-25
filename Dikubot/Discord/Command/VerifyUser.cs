using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.Discord.EventListeners;
using Discord;
using Discord.Commands;

namespace Dikubot.Discord.Command;

public class VerifyUser : ModuleBase<SocketCommandContext>
{
    [Command("verify")]
    [Summary("Get help with a specific command")]
    public async Task Verify([Summary("user discord id")] ulong userId, [Summary("user email")] string email)
    {
        UserGlobalServices userGlobalServices = new UserGlobalServices();
        if (!userGlobalServices.Get(Context.User).IsAdmin)
        {
            await ReplyAsync("Only system admins may use this command");
            return;
        }

        IUser user = DiscordBot.ClientStatic.GetUserAsync(userId).Result;

        if (user == null)
        {
            await ReplyAsync("Invalid user id. Can't verify");
            return;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            await ReplyAsync("Missing user email.");
            return;
        }

        UserGlobalModel userModel = new UserGlobalModel();
        userModel.DiscordIdLong = user.Id;
        userModel.Email = email;
        userModel.Verified = true;
        userGlobalServices.Upsert(userModel);
        await ReplyAsync($"User with ID {user.Id} and name {user.Username} has been verified with email ${email}");
    }
}