using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Guild.Models.Guild
{
    public class GuildMongoService : GuildMongoService<GuildMainModel>
    {
        public GuildMongoService(SocketGuild guild) : base("Guild", guild)
        {
        }

        /// <Summary>Inserts a Model in the collection. If a RoleModel with the same ID, Name or discordID already
        /// exists, then we imply invoke Update() on the model instead.</Summary>
        /// <param name="mainModelIn">The Model one wishes to be inserted.</param>
        /// <return>A Model.</return>
        public new GuildMainModel Upsert(GuildMainModel mainModelIn)
        {
            bool idCollision = Exists(model => model.DiscordId == mainModelIn.DiscordId);

            if (idCollision)
            {
                Update(m => m.DiscordId == mainModelIn.DiscordId, mainModelIn, new ReplaceOptions() {IsUpsert = true});
                return mainModelIn;
            }
            
            Insert(mainModelIn);
            return mainModelIn;
        }
        
        
        
        public bool Exists(string name)
        {
            return Exists(model => model.Name.ToLower() == name.ToLower());
        }

        /// <Summary>Checks if a RoleModel is already in the database.</Summary>
        /// <param name="mainModelIn">A boolean which tells if the models is in the database.</param>
        /// <return>A bool, true if the value already exist false if not.</return>
        public new bool Exists(GuildMainModel mainModelIn)
        {
            bool idCollision = Exists(model => model.Id == mainModelIn.Id);
           
            return idCollision;
        }

        /// <Summary>Removes a element from the collection by it's unique elements.</Summary>
        /// <param name="mainModelIn">The Model one wishes to remove.</param>
        /// <return>Void.</return>
        public new void Remove(GuildMainModel mainModelIn)
        {
            Remove(model => model.Id == mainModelIn.Id);
        }

        /// <Summary>Gets a role by it's discord id.</Summary>
        /// <param name="discordId">The discord id.</param>
        /// <return>A Model.</return>
        public new GuildMainModel Get(ulong discordId) =>
            Get(model => model.DiscordId == discordId) as GuildMainModel;

        /// <Summary>Converts a RoleSocket to a RoleModel.</Summary>
        /// <param name="role">The SocketRole model one wishes to be converted.</param>
        /// <return>A RoleModel.</return>
        public GuildMainModel SocketToModel(SocketGuild guild)
        {
            GuildMainModel guildMain = new GuildMainModel(guild);

            return guildMain;
        }
        
    }

    
}