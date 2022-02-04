using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Discord;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners;

public class GreetingListener
{
    public async Task UserJoined(SocketGuildUser user)
    {
        if (user.MutualGuilds.Count == 1)
        {
            await user.SendMessageAsync(
                $"Hej {user.Mention}! Du har tilsluttet dig **{user.Guild.Name}**, " +
                $"som er en del af **https://kuliv.dk/** netværket!\n\n" +
                $"Du kan finde andre KU relaterede netværk, få hjælp til Discord, " +
                $"vælge roller, bekræfte at du går på KU og meget mere på vores hjemmeside https://kuliv.dk/");
        }

        GuildSettingsService guildSettingsService = new GuildSettingsService();
        GuildSettingsModel guildSettingsModel = guildSettingsService.Get(user.Guild);

        if (!guildSettingsModel.WelcomeMessageEnabled)
        {
            return;
        }

        string welcomeMessage = guildSettingsModel.WelcomeMessage?.Message ?? "";
        if (string.IsNullOrWhiteSpace(welcomeMessage))
        {
            return;
        }

        await user.SendMessageAsync($"**Du har en velkomst besked fra {user.Guild.Name}:**\n\n{welcomeMessage}");
    }
}