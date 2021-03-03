using System.Collections.Generic;
using Dikubot.DataLayer.Caching;
using Dikubot.Webapp.Shared.Login;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Logic.User
{
    public class DiscordWebConnector
    {
        /*
         * Implement auto-removal of pending_passwords to avoid filling memory
         */
        private static Cache<string, ConnectDiscord> pending_passwords = new Cache<string, ConnectDiscord>(20);

        /// <summary>
        /// Adds a password to be typed into Discord, to make a connection between a webcircuit and Discord.
        /// </summary>
        /// <param name="password">A case insensitive password string</param>
        /// <param name="webcircuit">ConnectDiscord component webcircuit</param>
        public static void Add(string password, ConnectDiscord webcircuit)
        {
            pending_passwords.Add(password.ToUpper(), webcircuit);
        }

        /// <summary>
        /// Validates a user and tells the web circuit
        /// </summary>
        /// <param name="password">A case insensitive password string</param>
        /// <param name="user">The SocketUser which will be connected to the webcircuit</param>
        /// <returns>Whether or not the validation succeeded as a boolean</returns>
        public static bool Validate(string password, SocketUser user)
        {
            if (!pending_passwords.ContainsKey(password.ToUpper()))
            {
                return false;
            }

            pending_passwords[password.ToUpper()].DiscordConnected(user);
            pending_passwords.Remove(password.ToUpper());
            return true;
        }
    }
}