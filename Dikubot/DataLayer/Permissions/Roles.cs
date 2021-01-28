using System;
using System.Collections.Generic;
using System.Linq;
using Dikubot.Database.Models.Role;
using Discord;
using Discord.WebSocket;

namespace Dikubot.Permissions
{
    public partial class PermissionsService
    {
        
        /// <Summary>Will sync all the roles on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void UploadRoles()
        {
            var roleModels = _roleServices.Get();
            var socketRoles = context.Guild.Roles.ToList();
            var toBeRemoved = new List<RoleModel>(roleModels);
            
            Func<RoleModel, SocketRole, bool> inDB = (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id ||
                                                                 m0.Name == m1.Name;
            
            // Remove all the roles from the database if they are not on the discord server.
            roleModels.RemoveAll(m => socketRoles.Exists(n => inDB(m, n)));

            // Remove the roles from the database that is not on the discord server.
            toBeRemoved.ForEach(m => _roleServices.Remove(m));
            
            // Makes an upsert of the roles on the server so they match the ones in the database.
            socketRoles.ForEach(model => _roleServices.Upsert(_roleServices.SocketToModel(model)));
        }
        
        /// <Summary>Will sync all the roles on the database to the discord server.</Summary>
        /// <return>void.</return>
        public async void DownloadRoles()
        {
            var roleModels = _roleServices.Get();
            var socketRoles = context.Guild.Roles.ToList();
            var toBeRemoved = new List<SocketRole>(socketRoles);
            
            Func<RoleModel, SocketRole, bool> inDB = (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id ||
                                                                 m0.Name == m1.Name;
            
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
    }
}