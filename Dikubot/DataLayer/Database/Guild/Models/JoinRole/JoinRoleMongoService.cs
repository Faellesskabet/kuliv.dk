using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Dikubot.Discord;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Guild.Models.JoinRole
{
    public class JoinRoleMongoService : GuildMongoService<JoinRoleCategoryMainModel>
    { 
        public JoinRoleMongoService(Database database, SocketGuild guild) : base(database, guild)
        {
        }

        public override string GetCollectionName()
        {
            return "JoinRole";
        }
    }

}