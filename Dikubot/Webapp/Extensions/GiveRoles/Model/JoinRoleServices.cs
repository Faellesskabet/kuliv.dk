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
    public class JoinRoleServices : GuildServices<JoinRoleCategoryMainModel>
    { 
        public JoinRoleServices(SocketGuild guild) : base("JoinRole", guild)
        {
        }
        
        public JoinRoleServices(string guidId) : base("JoinRole", DiscordBot.ClientStatic.Guilds?.FirstOrDefault(g => g.Id.ToString().Equals(guidId)))
        {
        }
        
    }

}