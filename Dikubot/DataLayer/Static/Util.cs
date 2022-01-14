using System;
using System.Linq;
using System.Text.RegularExpressions;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Static
{
    public static class Util
    {
        
        public static readonly string INVISIBLE_CHARACTER = "‎";
        
        /// <summary>
        /// Tells is whether an email is a valid email
        /// Email address: RFC 2822 Format
        /// Matches a normal email address. Does not check the top-level domain.
        /// </summary>
        /// <param name="email">string</param>
        /// <returns>bool</returns>
        public static bool isEmail(string email)
        {
            return Regex.IsMatch(email,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase);
        }

        public static bool IsKUEmail(string email)
        {
            email = email.ToLower();
            return isEmail(email) && (email.EndsWith(".ku.dk") || email.EndsWith("@ku.dk"));
        }


        private static Random random = new Random();

        /// <summary>
        /// SimpleRandomString returns a random string with the specified length. It's called simple because it has removed look-alike characters like O and 0.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SimpleRandomString(int length)
        {
            const string chars = "ABCDEFGHJKMNPRSTWXYZ23456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void UpdateDiscordName(SocketGuildUser socketGuildUser, string name)
        {
            try
            {
                socketGuildUser.ModifyAsync(properties =>
                {
                    properties.Nickname = name;
                });
            }
            catch (Exception e)
            {
                // ignored, this only happens when the bot doesn't have access to change the user's name
            }
        }

        public static void UpdateUserNameOnAllForcedGuilds(SocketUser user)
        {
            GuildSettingsService guildSettingsService = new GuildSettingsService();
            UserGlobalServices userGlobalServices = new UserGlobalServices();
            UserGlobalModel userGlobalModel = userGlobalServices.Get(user);
            foreach (SocketGuild guild in user.MutualGuilds)
            {
                GuildSettingsModel guildSettingsModel = guildSettingsService.Get(guild);
                if (!guildSettingsModel.ForceNameChange)
                {
                    continue;
                }
                UpdateDiscordName(guild.GetUser(user.Id), userGlobalModel.Name);
            }
        }

        public static bool IsGuildAdmin(ulong userId, ulong guildId)
        {
            SocketGuild guild = DiscordBot.Client.GetGuild(guildId);
            SocketGuildUser user = guild?.GetUser(userId);
            return guild != null && (guild.OwnerId == user.Id || user.Roles.Any(role => role.Permissions.Administrator));
        }
    }
}