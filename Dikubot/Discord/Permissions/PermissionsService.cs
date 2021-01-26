using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Commands;
using Dikubot.Database.Models;
using Discord;
using Discord.WebSocket;

namespace Dikubot.Discord
{
    /// <Summary>Class for talking between the database and Discord.</Summary>
    public class PermissionsService
    {
        /// <Summary>The constructor of PermissionServices.</Summary>
        /// <param name="modelIn">The context for which the PermissionService is being executed in.</param>
        
        SocketCommandContext context; // This can not be made private.
        private readonly RoleServices _services;
        public PermissionsService(SocketCommandContext context)
        {
            this.context = context;
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
            var socketRoles = context.Guild.Roles.ToList();
            
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
            var socketRoles = context.Guild.Roles.ToList();

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
                    await context.Guild.CreateRoleAsync(properties.Name.Value, 
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
    }
}