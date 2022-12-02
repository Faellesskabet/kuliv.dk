using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild.Models.Guild;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Permissions
{
    public partial class PermissionsService
    {
        /// <Summary>Will sync all the Guilds on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void SetDatabaseGuild()
        {
            var GuildModels = _guildMongoService.Get();
            var toBeRemoved = new List<GuildMainModel>(GuildModels);

            Func<GuildMainModel, SocketGuild, bool> inDB = (m0, m1) =>
                Convert.ToUInt64(m0.Id) == m1.Id;

            
        }

        /// <Summary>Will sync all the text channels on the database to the discord server.</Summary>
        /// <return>void.</return>
        public async void SetDiscordGuilds()
        {
            
        }
        

        /// <Summary>Add a text channel on the discord server to the database.</Summary>
        /// <return>void.</return>
        public void AddOrUpdateDatabaseGuild(GuildMainModel guildMain) =>
            _guildMongoService.Upsert(guildMain);

        /// <Summary>Removes a text channel from the database.</Summary>
        /// <return>void.</return>
        public void RemoveDatabaseGuild(SocketGuild Guild) =>
            _guildMongoService.Remove(_guildMongoService.SocketToModel(Guild));

        /// <Summary>Removes a text channel from the database.</Summary>
        /// <return>void.</return>
        

        /// <Summary>Removes a text channel from the database.</Summary>
        /// <return>void.</return>
        public void RemoveDatabaseGuild(GuildMainModel guildMain) =>
            _guildMongoService.Remove(guildMain);

        /// <Summary>Removes a text channel from the database and the discord server.</Summary>
        /// <return>void.</return>
        public async Task RemoveGuild(GuildMainModel guildMain)
        {
            RemoveDatabaseGuild(guildMain);
        }

       
        

        /// <Summary>Adds a text channel to the database and the discord server.</Summary>
        /// <return>void.</return>
        public async Task<GuildMainModel> AddGuild(GuildMainModel guildMain)
        {
            
            AddOrUpdateDatabaseGuild(guildMain);
            return guildMain;
        }
    }
}