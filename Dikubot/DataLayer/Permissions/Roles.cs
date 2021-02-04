using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Commands;
using Dikubot.Database.Models;
using Dikubot.Database.Models.Interfaces;
using Dikubot.Database.Models.SubModels;
using Dikubot.DataLayer.Static;
using Dikubot.Database.Models.Role;
using Discord;
using Discord.WebSocket;

namespace Dikubot.Permissions
{
    public partial class PermissionsService
    {
        /// <Summary>Finds all the RoleModels which should be removed from the database since it is not a role on the
        /// discord server.</Summary>
        /// <param name="roleModels">List of all the roles in the database.</param>
        /// <param name="socketRoles">List of all the roles on the discord server.</param>
        /// <return>A list of all the roles which should be removed from the database.</return>
        private List<RoleModel> ToBeRemovedFromDatabase(List<RoleModel> roleModels, List<SocketRole> socketRoles)
        {
            Func<RoleModel, SocketRole, bool> inDB = (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id;
            roleModels.RemoveAll(m => socketRoles.Exists(n => inDB(m, n)));
            return roleModels;
        }

        /// <Summary>Will sync all the roles on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void SetDatabaseRoles()
        {
            var roleModels = _roleServices.Get();
            var socketRoles = guild.Roles.ToList();
            var toBeRemoved = new List<RoleModel>(roleModels);

            Func<RoleModel, SocketRole, bool> inDB = (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id;

            // Remove all the roles from the database if they are not on the discord server.
            roleModels.RemoveAll(m => socketRoles.Exists(n => inDB(m, n)));

            // Remove the roles from the database that is not on the discord server.
            toBeRemoved.ForEach(m => _roleServices.Remove(m));
            
            // Makes an upsert of the roles on the server so they match the ones in the database.
            socketRoles.ForEach(model => _roleServices.Upsert(_roleServices.SocketToModel(model)));
        }
        
        /// <Summary>Will sync all the roles on the database to the discord server.</Summary>
        /// <return>void.</return>
        public async void SetDiscordRoles()
        {
            var roleModels = _roleServices.Get();
            var socketRoles = guild.Roles.ToList();
            
            var toBeRemoved = new List<SocketRole>(socketRoles);

            Func<RoleModel, SocketRole, bool> inDB = (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id;
            

            // Remove all the roles from the discord server if they are not in the database.
            toBeRemoved.RemoveAll(m => roleModels.Exists(n => inDB(n, m)));
            
            foreach (var socketRole in toBeRemoved)
                await socketRole.DeleteAsync();

            // Roles which will not be be changed.
            var unchangeableRoles = new List<String> { "Admin" };
            
            foreach(var roleModel in roleModels)
            {
                // Skip this role if is in unchangeableRoles.
                if (unchangeableRoles.Contains(roleModel.Name))
                    continue;
                
                // Finds the role discord server role that match the role in the database.
                var socketRole = socketRoles.Find(socket => Convert.ToUInt64(roleModel.DiscordId) == socket.Id ||
                                                            roleModel.Name == socket.Name);
                
                if (socketRole == null)
                {
                    // If the role could not be found create it.
                    var properties = _roleServices.ModelToRoleProperties(roleModel);
                    await guild.CreateRoleAsync(properties.Name.Value, 

                        properties.Permissions.Value, 
                        properties.Color.Value, 
                        properties.Hoist.Value, 
                        properties.Mentionable.Value, 
                        RequestOptions.Default);
                }
                else
                {
                    // If the role could be found modify it so it matches the database.
                    var _properties = _roleServices.ModelToRoleProperties(roleModel);
                    await socketRole.ModifyAsync(properties =>
                    {
                        properties.Color = _properties.Color;
                        properties.Hoist = _properties.Hoist;
                        properties.Mentionable = _properties.Mentionable;
                        properties.Name = _properties.Name;
                        properties.Permissions = _properties.Permissions;
                        // Can not get this to work it will not change the role position.
                        // properties.Position = _properties.Position;
                    });
                }
            }
        }

        /// <summary>
        /// This functions downloads a user's roles from the Database and adds them to their Discord profile. The user's roles will match exactly what is in the database,
        /// which means roles not specified in the database will be removed from the user. Expired roles will also be removed from the user
        /// </summary>
        /// <param name="userModel"></param>
        public async void SetDiscordUserRoles(UserModel userModel)
        {
            
            //We get all the user's roles in the database
            HashSet<UserRoleModel> userRoleModels = new HashSet<UserRoleModel>(userModel.Roles);
            SocketUser discordUser = userModel.DiscordUser;
            if (discordUser == null)
            {
                return;
            }

            //We get the user in the context of the current guilds
            SocketGuildUser guildUser = guild.GetUser(discordUser.Id);
            if (guildUser == null)
            {
                return;
            }

            // We get the user's roles and remove all the roles not in the database.
            // We also remove the role if it has expired
            IReadOnlyCollection<SocketRole> discordRoles = guildUser.Roles;
            IEnumerable<IRole> removeRoles = discordRoles.Where((role, i) => 
                !userRoleModels.Contains(new UserRoleModel(_roleServices.SocketToModel(role))) 
                || !userModel.IsRoleActive(_roleServices.SocketToModel(role)));
            foreach (IRole role in removeRoles)
            {
                try
                {
                    Logger.Debug($"Removing {role.Name} from {guildUser.Username}");
                    await guildUser.RemoveRoleAsync(role, RequestOptions.Default);
                }
                catch (Exception)
                {
                    Logger.Debug($"Could not remove {role.Name} from {guildUser.Username}");
                }
            }

            //We add the roles in the database to the user, but only if the role is currently active
            IEnumerable<IRole> addRoles =
                userRoleModels.Where(model => ((IActiveTimeFrame)model).IsActive())
                    .Select((model) => guild.GetRole(Convert.ToUInt64(model.RoleModel.DiscordId))).Where(role => role != null);
            foreach (IRole role in addRoles)
            {
                try
                {
                    Logger.Debug($"Adding {role.Name} to {guildUser.Username}");
                    await guildUser.AddRoleAsync(role, RequestOptions.Default);
                }
                catch (Exception)
                { 
                    Logger.Debug($"Could not add {role.Name} to {guildUser.Username}");
                }
            }

        }

        public void SetDiscordUserRoles(SocketUser user)
        {
            SetDiscordUserRoles(new UserServices().Get(user));
        }
        
        /// <Summary>Add a role on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void AddOrUpdateDatabaseRole(SocketRole role) =>
            _roleServices.Upsert(_roleServices.SocketToModel(role));
        
        /// <Summary>Removes a role on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void RemoveDatabaseRole(SocketRole role) =>
            _roleServices.Remove(_roleServices.SocketToModel(role));
    }
}