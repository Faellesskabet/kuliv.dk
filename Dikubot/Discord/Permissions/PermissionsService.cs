using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Commands;
using Dikubot.Database.Models;
using Dikubot.Database.Models.SubModels;
using Dikubot.DataLayer.Static;
using Discord;
using Discord.WebSocket;

namespace Dikubot.Discord
{
    /// <Summary>Class for talking between the database and Discord.</Summary>
    public class PermissionsService
    {
        /// <Summary>The constructor of PermissionServices.</Summary>
        /// <param name="modelIn">The context for which the PermissionService is being executed in.</param>
        
        SocketGuild guild; // This can not be made private.
        private readonly RoleServices _services;
        public PermissionsService(SocketGuild guild)
        {
            this.guild = guild;
            _services = new RoleServices();
        }

        /// <Summary>Finds all the RoleModels which should be removed from the database since it is not a role on the
        /// discord server.</Summary>
        /// <param name="roleModels">List of all the roles in the database.</param>
        /// <param name="socketRoles">List of all the roles on the discord server.</param>
        /// <return>A list of all the roles which should be removed from the database.</return>
        private List<RoleModel> ToBeRemovedFromDatabase(List<RoleModel> roleModels, List<SocketRole> socketRoles)
        {
            Func<RoleModel, SocketRole, bool> inDB = (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id ||
                                                                m0.Name == m1.Name;
            roleModels.RemoveAll(m => socketRoles.Exists(n => inDB(m, n)));
            return roleModels;
        }
        
        /// <Summary>Will sync all the roles on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void UploadRoles()
        {
            var roleModels = _services.Get();
            var socketRoles = guild.Roles.ToList();
            
            // Remove all the roles from the database if they are not on the discord server.
            var toBeRemoved = ToBeRemovedFromDatabase(roleModels, socketRoles);

            // Remove the roles from the database that is not on the discord server.
            toBeRemoved.ForEach(m => _services.Remove(m));
            
            // Makes an upsert of the roles on the server so they match the ones in the database.
            socketRoles.ForEach(model => _services.Upsert(_services.SocketToModel(model)));
        }

        /// <Summary>Finds all the RoleModels which should be removed from the discord server since it is not a role in
        /// the database.</Summary>
        /// <param name="roleModels">List of all the roles in the database.</param>
        /// <param name="socketRoles">List of all the roles on the discord server.</param>
        /// <return>A list of all the roles which should be removed from the discord server.</return>
        private List<SocketRole> ToBeRemovedFromDiscord(List<RoleModel> roleModels, List<SocketRole> socketRoles)
        {
            Func<RoleModel, SocketRole, bool> inDB = (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id ||
                                                                 m0.Name == m1.Name;
            socketRoles.RemoveAll(m => roleModels.Exists(n => inDB(n, m)));
            return socketRoles;
        }
        
        /// <Summary>Will sync all the roles on the database to the discord server.</Summary>
        /// <return>void.</return>
        public async void DownloadRoles()
        {
            var roleModels = _services.Get();
            var socketRoles = guild.Roles.ToList();

            // Remove all the roles from the discord server if they are not in the database.
            var toBeRemoved = ToBeRemovedFromDiscord(roleModels, socketRoles.ToList());
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
                    var properties = _services.ModelToRoleProperties(roleModel);
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
                    var _properties = _services.ModelToRoleProperties(roleModel);
                    await socketRole.ModifyAsync(properties =>
                    {
                        properties.Color = _properties.Color;
                        properties.Hoist = _properties.Hoist;
                        properties.Mentionable = _properties.Mentionable;
                        properties.Name = _properties.Name;
                        properties.Permissions = _properties.Permissions;
                        // Can't get this to work it won't change the role position.
                        // properties.Position = _properties.Position;
                    });
                }
            }
        }

        /// <summary>
        /// This functions downloads a user's roles from the Database and adds them to their Discord profile. The user's roles will match exactly what is in the database,
        /// which means roles not specified in the database will be removed from the user. 
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
                !userRoleModels.Contains(new UserRoleModel(_services.SocketToModel(role))) 
                || !new UserRoleModel(_services.SocketToModel(role)).IsActive());
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
                userRoleModels.Where(model => model.IsActive())
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
        
    }
}