using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Guild.Models.JoinRole
{
    public class JoinRoleServices : GuildServices<JoinRoleMainModel>
    {
        public JoinRoleServices(SocketGuild guild) : base("JoinRole", guild)
        {
        }
        
        
    }

    public class JoinChannelCategoryServices : GuildServices<JoinRoleCategoryMainModel>
    {
        public JoinChannelCategoryServices(SocketGuild guild) : base("JoinRoleCategory", guild)
        {
        }


        public override Dictionary<string, List<JoinRoleCategoryMainModel>> GetAllAsDictionary(Expression<Func<JoinRoleCategoryMainModel, bool>> filter = null)
        {
            
            var service = new RoleServices(Guild);
            var roles = service.GetAllAsDictionary();
            var dictionary = base.GetAllAsDictionary(filter);
            foreach (var item in dictionary)
            {
                var list = item.Value;
                foreach (var joinRoleCategoryModel in list)
                {
                    joinRoleCategoryModel.GuildId = item.Key;
                }
                list.ForEach(t => t.TextRoles.ToList()?.ForEach(j => j?.JoinRoleDate(roles[item.Key])));
                dictionary[item.Key] = list;
            }
            return dictionary;
        }
        public override List<JoinRoleCategoryMainModel> Get()
        {
            
            var service = new RoleServices(Guild);
            var roles = service.Get();
            var dictionary = base.Get();
            dictionary.ForEach(t => t.TextRoles.ToList()?.ForEach(j => j?.JoinRoleDate(roles)));
            foreach (var joinRoleCategoryModel in dictionary)
            {
                joinRoleCategoryModel.GuildId = Guild.Id.ToString();
            }
            return dictionary;
        }


        public override void Update(Expression<Func<JoinRoleCategoryMainModel, bool>> predicate, JoinRoleCategoryMainModel mainModelIn, ReplaceOptions options = null)
        {
            foreach (var VARIABLE in Get(mainModelIn.Id).Listeners)
            {
                mainModelIn.Listeners.TryAdd(VARIABLE.Key,VARIABLE.Value);
            }
            base.Update(predicate, mainModelIn, options);
        }

        public override void Remove(Guid id)
        {
            if (Get(id)?.Listeners.Any() ?? false)
            {
                foreach (var VARIABLE in Get(id).Listeners)
                {
                    (Guild.Channels.FirstOrDefault(c => c.Id.ToString().Equals(VARIABLE.Key)) as ISocketMessageChannel)?
                        .GetMessageAsync(ulong.Parse(VARIABLE.Value))?.Result?.DeleteAsync();
                }
            }
            base.Remove(id);
        }


        public override Dictionary<string, List<JoinRoleCategoryMainModel>> GetAllWithUser(Expression<Func<JoinRoleCategoryMainModel, bool>> filter = null)
        {
            
            var service = new RoleServices(Guild);
            var roles = service.GetAllAsDictionary();
            var dictionary = base.GetAllWithUser();
            foreach (var item in dictionary)
            {
                var list = item.Value;
                foreach (var joinRoleCategoryModel in list)
                {
                    joinRoleCategoryModel.GuildId = item.Key;
                }
                list.ForEach(t => t.TextRoles.ToList()?.ForEach(j => j?.JoinRoleDate(roles[item.Key])));
                dictionary[item.Key] = list;
            }
            return dictionary;
        }
        
  
    }

}