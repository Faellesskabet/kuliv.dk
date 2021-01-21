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
    }
}