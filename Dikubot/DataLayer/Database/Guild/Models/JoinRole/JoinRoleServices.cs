using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Guild.Models.JoinRole
{
    public class JoinChannelCategoryServices : GuildServices<JoinRoleCategoryMainModel>
    { 
        public JoinChannelCategoryServices(SocketGuild guild) : base("JoinRoleCategory", guild)
        {
        }
        
    }

}