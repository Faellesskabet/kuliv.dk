using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Discord.Rest;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Permissions
{
    public partial class PermissionsService
    {
        /// <Summary>Will sync all the users on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void SetDatabaseUsers()
        {
            var userModels = _userServices.Get();
            var socketUsers = guild.Users.ToList();
            var toBeRemoved = new List<UserGuildModel>(userModels);

            Func<UserGuildModel, SocketGuildUser, bool> inDB = (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id;

            // Remove all the users from the database if they are not on the discord server.
            toBeRemoved.RemoveAll(m =>
                socketUsers.Exists(n => inDB(m, n)));

            // Remove the users from the database that is not on the discord server.
            toBeRemoved.ForEach(m => _userServices.Remove(m));

            // Makes an upsert of the users on the server so they match the ones in the database.
            socketUsers.ForEach(model =>
                _userServices.Upsert(_userServices.SocketToModel(model)));
        }

        /// <Summary>Will sync all the users on the database to the discord server.</Summary>
        /// <return>void.</return>
        public async void SetDiscordUsers()
        {
            var userModels = _userServices.Get();
            var socketUsers = guild.Users.ToList();
            foreach (var userModel in userModels)
            {
                // Finds the user discord server user that match the user in the database.
                var socketUser = socketUsers.Find(socket => Convert.ToUInt64(userModel.DiscordId) == socket.Id);

                if (socketUser == null)
                {
                    // If the user could not be found delete it from the database.
                    _userServices.Remove(userModel);
                }
                else
                {
                    var _properties = _userServices.ModelToProperties(guild, userModel);
                    await socketUser.ModifyAsync(properties =>
                    {
                        properties.Nickname = _properties.Nickname;
                        properties.Roles = _properties.Roles;
                        properties.RoleIds = _properties.RoleIds;
                    });
                }
            }
        }

        /// <summary>
        /// Grants the verified role to all users that are verified, and removes it from the ones that aren't.
        /// </summary>
        public async void UpdateUserDiscordRoles()
        {
            var socketUsers = guild.Users.ToList();
            foreach (var user in socketUsers)
            {
                SetDiscordUserRoles(user);
            }
            
        }

        /// <Summary>Add a user on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void AddOrUpdateDatabaseUser(SocketGuildUser user) =>
            _userServices.Upsert(_userServices.SocketToModel(user));

        /// <Summary>Add a user on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void AddOrUpdateDatabaseUser(RestGuildUser user) =>
            _userServices.Upsert(_userServices.RestToModel(user));

        /// <Summary>Add a user on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void AddOrUpdateDatabaseUser(UserGuildModel userMain) =>
            _userServices.Upsert(userMain);

        /// <Summary>Removes a user from the database.</Summary>
        /// <return>void.</return>
        public void RemoveDatabaseUser(SocketGuildUser user) =>
            _userServices.Remove(_userServices.SocketToModel(user));

        /// <Summary>Removes a user from the database.</Summary>
        /// <return>void.</return>
        public void RemoveDatabaseUser(RestGuildUser user) =>
            _userServices.Remove(_userServices.RestToModel(user));

        /// <Summary>Removes a user from the database.</Summary>
        /// <return>void.</return>
        public void RemoveDatabaseUser(UserGuildModel userMain) =>
            _userServices.Remove(userMain);

        /// <Summary>Updates a users info on discord and database, specifically nickname and roles.</Summary>
        /// <return>void.</return>
        public async Task UpdateUser(UserGuildModel userMain)
        {
            var socketUser = guild.GetUser(Convert.ToUInt64(userMain.DiscordId));
            var _properties = _userServices.ModelToProperties(guild, userMain);
            await socketUser.ModifyAsync(properties =>
            {
                properties.Nickname = _properties.Nickname;
                properties.Roles = _properties.Roles;
                properties.RoleIds = _properties.RoleIds;
            });
            AddOrUpdateDatabaseUser(userMain);
        }

        /// <Summary>Updates a users info on discord and database, specifically nickname and roles.</Summary>
        /// <return>void.</return>
        public async Task UpdateUser(SocketGuildUser user)
        {
            var _properties = _userServices.ModelToProperties(guild, _userServices.SocketToModel(user));
            await user.ModifyAsync(properties =>
            {
                properties.Nickname = _properties.Nickname;
                properties.Roles = _properties.Roles;
                properties.RoleIds = _properties.RoleIds;
            });
            AddOrUpdateDatabaseUser(user);
        }

        /// <Summary>Updates a users info on discord and database, specifically nickname and roles.</Summary>
        /// <return>void.</return>
        public async Task UpdateUser(RestGuildUser user)
        {
            var _properties = _userServices.ModelToProperties(guild, _userServices.RestToModel(user));
            await user.ModifyAsync(properties =>
            {
                properties.Nickname = _properties.Nickname;
                properties.Roles = _properties.Roles;
                properties.RoleIds = _properties.RoleIds;
            });
            AddOrUpdateDatabaseUser(user);
        }
    }
}