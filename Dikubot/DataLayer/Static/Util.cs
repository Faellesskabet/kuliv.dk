using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.Discord;
using Discord;
using Discord.WebSocket;
using Microsoft.IdentityModel.Tokens;

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
                socketGuildUser.ModifyAsync(properties => { properties.Nickname = name; });
            }
            catch (Exception e)
            {
                // ignored, this only happens when the bot doesn't have access to change the user's name
            }
        }

        public static void UpdateUserNameOnAllForcedGuilds(SocketUser user)
        {
            if (user == null)
            {
                return;
            }
            GuildSettingsMongoService guildSettingsMongoService = new GuildSettingsMongoService();
            UserGlobalMongoService userGlobalMongoService = new UserGlobalMongoService();
            UserGlobalModel userGlobalModel = userGlobalMongoService.Get(user);
            foreach (SocketGuild guild in user.MutualGuilds)
            {
                GuildSettingsModel guildSettingsModel = guildSettingsMongoService.Get(guild);
                if (!guildSettingsModel.ForceNameChange)
                {
                    continue;
                }

                UpdateDiscordName(guild.GetUser(user.Id), userGlobalModel.Name);
            }
        }


        public static string ConcatListToString<T>(List<T> list)
        {
            return list.IsNullOrEmpty() ? "" : string.Join(", ", list);
        }
        
        public static bool IsGuildAdmin(ulong userId, ulong guildId)
        {
            SocketGuild guild = DiscordBot.ClientStatic.GetGuild(guildId);
            SocketGuildUser user = guild?.GetUser(userId);
            return guild != null &&
                   (guild.OwnerId == user.Id || user.Roles.Any(role => role.Permissions.Administrator));
        }

        public static IEnumerable<string> GetEmojiUnicodes()
        {
            return NamesAndUnicodes.Values;
        }
        
        public static IReadOnlyDictionary<string, string> NamesAndUnicodes { get; } =
            (IReadOnlyDictionary<string, string>)new Dictionary<string, string>()
            {
                [",:("] = "\uD83D\uDE13",
                [",:)"] = "\uD83D\uDE05",
                [",:-("] = "\uD83D\uDE13",
                [",:-)"] = "\uD83D\uDE05",
                [",=("] = "\uD83D\uDE13",
                [",=)"] = "\uD83D\uDE05",
                [",=-("] = "\uD83D\uDE13",
                [",=-)"] = "\uD83D\uDE05",
                ["0:)"] = "\uD83D\uDE07",
                ["0:-)"] = "\uD83D\uDE07",
                ["0=)"] = "\uD83D\uDE07",
                ["0=-)"] = "\uD83D\uDE07",
                ["8-)"] = "\uD83D\uDE0E",
                [":$"] = "\uD83D\uDE12",
                [":'("] = "\uD83D\uDE22",
                [":')"] = "\uD83D\uDE02",
                [":'-("] = "\uD83D\uDE22",
                [":'-)"] = "\uD83D\uDE02",
                [":'-D"] = "\uD83D\uDE02",
                [":'D"] = "\uD83D\uDE02",
                [":("] = "\uD83D\uDE26",
                [":)"] = "\uD83D\uDE42",
                [":*"] = "\uD83D\uDE17",
                [":+1:"] = "\uD83D\uDC4D",
                [":+1::skin-tone-1:"] = "\uD83D\uDC4D\uD83C\uDFFB",
                [":+1::skin-tone-2:"] = "\uD83D\uDC4D\uD83C\uDFFC",
                [":+1::skin-tone-3:"] = "\uD83D\uDC4D\uD83C\uDFFD",
                [":+1::skin-tone-4:"] = "\uD83D\uDC4D\uD83C\uDFFE",
                [":+1::skin-tone-5:"] = "\uD83D\uDC4D\uD83C\uDFFF",
                [":+1_tone1:"] = "\uD83D\uDC4D\uD83C\uDFFB",
                [":+1_tone2:"] = "\uD83D\uDC4D\uD83C\uDFFC",
                [":+1_tone3:"] = "\uD83D\uDC4D\uD83C\uDFFD",
                [":+1_tone4:"] = "\uD83D\uDC4D\uD83C\uDFFE",
                [":+1_tone5:"] = "\uD83D\uDC4D\uD83C\uDFFF",
                [":,'("] = "\uD83D\uDE2D",
                [":,'-("] = "\uD83D\uDE2D",
                [":,("] = "\uD83D\uDE22",
                [":,)"] = "\uD83D\uDE02",
                [":,-("] = "\uD83D\uDE22",
                [":,-)"] = "\uD83D\uDE02",
                [":,-D"] = "\uD83D\uDE02",
                [":,D"] = "\uD83D\uDE02",
                [":-$"] = "\uD83D\uDE12",
                [":-("] = "\uD83D\uDE26",
                [":-)"] = "\uD83D\uDE42",
                [":-*"] = "\uD83D\uDE17",
                [":-/"] = "\uD83D\uDE15",
                [":-1:"] = "\uD83D\uDC4E",
                [":-1::skin-tone-1:"] = "\uD83D\uDC4E\uD83C\uDFFB",
                [":-1::skin-tone-2:"] = "\uD83D\uDC4E\uD83C\uDFFC",
                [":-1::skin-tone-3:"] = "\uD83D\uDC4E\uD83C\uDFFD",
                [":-1::skin-tone-4:"] = "\uD83D\uDC4E\uD83C\uDFFE",
                [":-1::skin-tone-5:"] = "\uD83D\uDC4E\uD83C\uDFFF",
                [":-@"] = "\uD83D\uDE21",
                [":-D"] = "\uD83D\uDE04",
                [":-O"] = "\uD83D\uDE2E",
                [":-P"] = "\uD83D\uDE1B",
                [":-S"] = "\uD83D\uDE12",
                [":-Z"] = "\uD83D\uDE12",
                [":-\")"] = "\uD83D\uDE0A",
                [":-\\"] = "\uD83D\uDE15",
                [":-o"] = "\uD83D\uDE2E",
                [":-|"] = "\uD83D\uDE10",
                [":100:"] = "\uD83D\uDCAF",
                [":1234:"] = "\uD83D\uDD22",
                [":8ball:"] = "\uD83C\uDFB1",
                [":@"] = "\uD83D\uDE21",
                [":D"] = "\uD83D\uDE04",
                [":O"] = "\uD83D\uDE2E",
                [":P"] = "\uD83D\uDE1B",
                [":\")"] = "\uD83D\uDE0A",
                [":|"] = "\uD83D\uDE10",
                [";("] = "\uD83D\uDE2D",
                [";)"] = "\uD83D\uDE09",
                [";-("] = "\uD83D\uDE2D",
                [";-)"] = "\uD83D\uDE09",
                ["</3"] = "\uD83D\uDC94",
                ["<3"] = "❤️",
                ["<\\3"] = "\uD83D\uDC94",
                ["=$"] = "\uD83D\uDE12",
                ["='("] = "\uD83D\uDE22",
                ["=')"] = "\uD83D\uDE02",
                ["='-("] = "\uD83D\uDE22",
                ["='-)"] = "\uD83D\uDE02",
                ["='-D"] = "\uD83D\uDE02",
                ["='D"] = "\uD83D\uDE02",
                ["=("] = "\uD83D\uDE26",
                ["=)"] = "\uD83D\uDE42",
                ["=*"] = "\uD83D\uDE17",
                ["=,'("] = "\uD83D\uDE2D",
                ["=,'-("] = "\uD83D\uDE2D",
                ["=,("] = "\uD83D\uDE22",
                ["=,)"] = "\uD83D\uDE02",
                ["=,-("] = "\uD83D\uDE22",
                ["=,-)"] = "\uD83D\uDE02",
                ["=,-D"] = "\uD83D\uDE02",
                ["=,D"] = "\uD83D\uDE02",
                ["=-$"] = "\uD83D\uDE12",
                ["=-("] = "\uD83D\uDE26",
                ["=-)"] = "\uD83D\uDE42",
                ["=-*"] = "\uD83D\uDE17",
                ["=-/"] = "\uD83D\uDE15",
                ["=-@"] = "\uD83D\uDE21",
                ["=-D"] = "\uD83D\uDE04",
                ["=-O"] = "\uD83D\uDE2E",
                ["=-P"] = "\uD83D\uDE1B",
                ["=-S"] = "\uD83D\uDE12",
                ["=-Z"] = "\uD83D\uDE12",
                ["=-\")"] = "\uD83D\uDE0A",
                ["=-\\"] = "\uD83D\uDE15",
                ["=-o"] = "\uD83D\uDE2E",
                ["=-|"] = "\uD83D\uDE10",
                ["=@"] = "\uD83D\uDE21",
                ["=D"] = "\uD83D\uDE04",
                ["=O"] = "\uD83D\uDE2E",
                ["=P"] = "\uD83D\uDE1B",
                ["=\")"] = "\uD83D\uDE0A",
                ["=o"] = "\uD83D\uDE2E",
                ["=s"] = "\uD83D\uDE12",
                ["=z"] = "\uD83D\uDE12",
                ["=|"] = "\uD83D\uDE10",
                [">:("] = "\uD83D\uDE20",
                [">:-("] = "\uD83D\uDE20",
                [">=("] = "\uD83D\uDE20",
                [">=-("] = "\uD83D\uDE20",
                ["B-)"] = "\uD83D\uDE0E",
                ["O:)"] = "\uD83D\uDE07",
                ["O:-)"] = "\uD83D\uDE07",
                ["O=)"] = "\uD83D\uDE07",
                ["O=-)"] = "\uD83D\uDE07",
                ["X-)"] = "\uD83D\uDE06",
                ["]:("] = "\uD83D\uDC7F",
                ["]:)"] = "\uD83D\uDE08",
                ["]:-("] = "\uD83D\uDC7F",
                ["]:-)"] = "\uD83D\uDE08",
                ["]=("] = "\uD83D\uDC7F",
                ["]=)"] = "\uD83D\uDE08",
                ["]=-("] = "\uD83D\uDC7F",
                ["]=-)"] = "\uD83D\uDE08",
                ["o:)"] = "\uD83D\uDE07",
                ["o:-)"] = "\uD83D\uDE07",
                ["o=)"] = "\uD83D\uDE07",
                ["o=-)"] = "\uD83D\uDE07",
                ["x-)"] = "\uD83D\uDE06",
                ["♡"] = "❤️"
            };
    }

}