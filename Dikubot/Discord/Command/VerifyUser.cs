using System.Collections.Generic;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.DataLayer.Permissions;
using Dikubot.Discord.EventListeners;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

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

        UserGlobalModel userModel = userGlobalServices.Get(userId);
        userModel.DiscordIdLong = user.Id;
        userModel.Email = email;
        userModel.Verified = true;
        userGlobalServices.Upsert(userModel);
        
        IReadOnlyCollection<SocketGuild> mutualGuilds = DiscordBot.ClientStatic.GetUser(userModel.DiscordIdLong)?.MutualGuilds;
        if (mutualGuilds != null)
        {
            foreach (SocketGuild mutualGuild in mutualGuilds)
            {
                UserGuildServices userGuildServices = new UserGuildServices(mutualGuild);
                PermissionsService permissionsService = new PermissionsService(mutualGuild);
                UserGuildModel newUser = userGuildServices.Get(userModel.DiscordId);
                await permissionsService.SetDiscordUserRoles(newUser);
            }
        }
        
        
        await ReplyAsync($"User with ID {user.Id} and name {user.Username} has been verified with email {email}");
    }
}