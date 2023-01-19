using System.Collections.Generic;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.DataLayer.Permissions;
using Dikubot.Discord.EventListeners;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Dikubot.Discord.Command;

public class VerifyUser : InteractionModuleBase<SocketInteractionContext>
{

    private UserGlobalMongoService _userGlobalMongoService;
    private IGuildMongoFactory _guildMongoFactory;
    private IPermissionServiceFactory _permissionServiceFactory;
    private DiscordSocketClient _discordSocketClient;
    public VerifyUser(UserGlobalMongoService userGlobalMongoService, IGuildMongoFactory guildMongoFactory,
        IPermissionServiceFactory permissionServiceFactory, DiscordSocketClient discordSocketClient)
    {
        _userGlobalMongoService = userGlobalMongoService;
        _guildMongoFactory = guildMongoFactory;
        _permissionServiceFactory = permissionServiceFactory;
        _discordSocketClient = discordSocketClient;
    }
    [SlashCommand("verify", "verify a user")]
    public async Task Verify([Summary("user discord id")] ulong userId, [Summary("user email")] string email)
    {
        if (!_userGlobalMongoService.Get(Context.User).IsAdmin)
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

        UserGlobalModel userModel = _userGlobalMongoService.Get(userId);
        userModel.DiscordIdLong = user.Id;
        userModel.Email = email;
        userModel.Verified = true;
        _userGlobalMongoService.Upsert(userModel);
        
        IReadOnlyCollection<SocketGuild> mutualGuilds = _discordSocketClient.GetUser(userModel.DiscordIdLong)?.MutualGuilds;
        if (mutualGuilds != null)
        {
            foreach (SocketGuild mutualGuild in mutualGuilds)
            {
                UserGuildMongoService userGuildServices = _guildMongoFactory.Get<UserGuildMongoService>(mutualGuild);
                PermissionsService permissionsService = _permissionServiceFactory.Get(mutualGuild);
                UserGuildModel newUser = userGuildServices.Get(userModel.DiscordId);
                await permissionsService.SetDiscordUserRoles(newUser);
            }
        }
        
        
        await ReplyAsync($"User with ID {user.Id} and name {user.Username} has been verified with email {email}");
    }
}