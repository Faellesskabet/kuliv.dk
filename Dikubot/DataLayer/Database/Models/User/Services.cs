using System;
using Discord.WebSocket;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Dikubot.Database.Models
{
    /// <summary>
    /// Class for for retrieving information from the User collection.
    /// </summary>
    public class UserServices : Services<UserModel>
    {
        public UserServices() : base("Main", "Users") { }
        public UserModel Get(SocketUser user)
        {
            UserModel model = this.Get(inmodel => inmodel.DiscordId == user.Id.ToString());
            if (model == null)
            {
                model = new UserModel();
                model.DiscordUser = user;
            }
            return model;
        }

        public bool Exists(SocketUser user)
        {
            return Exists(model => model.DiscordId == user.Id.ToString());
        }

        public bool EmailExists(string email)
        {
            email = email.ToLower();
            return Exists(model => model.Email == email);
        }

        public UserModel GetFromEmail(string email)
        {
            email = email.ToLower();
            return Get(model => model.Email == email);
        }
    }
}

