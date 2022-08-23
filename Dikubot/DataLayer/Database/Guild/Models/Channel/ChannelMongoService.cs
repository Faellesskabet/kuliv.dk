using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Guild.Models.Channel
{
    public abstract class ChannelMongoService<TCModel> : GuildMongoService<TCModel> where TCModel : ChannelMainModel
    {
        protected ChannelMongoService(
            string collectionName, SocketGuild guild)
            : base(collectionName, guild)
        {
        }

        /// <Summary>Inserts a ChannelModel in the collection. If a ChannelModel with the same ID or discordID
        /// already exists, then we imply invoke Update() on the model instead.</Summary>
        /// <param name="modelIn">The Model one wishes to be inserted.</param>
        /// <return>A Model.</return>
        public new TCModel Upsert(TCModel modelIn)
        {
            bool idCollision = Exists(model => model.Id == modelIn.Id);
            bool discordIdCollision = Exists(model => model.DiscordId == modelIn.DiscordId);

            if (idCollision)
            {
                Update(modelIn, new ReplaceOptions() {IsUpsert = true});
                return modelIn;
            }

            if (discordIdCollision)
            {
                Update(m => m.DiscordId == modelIn.DiscordId, modelIn, new ReplaceOptions() {IsUpsert = true});
                return modelIn;
            }

            Insert(modelIn);
            return modelIn;
        }

        /// <Summary>Removes a ChannelModel from the collection by it's unique elements.</Summary>
        /// <param name="modelIn">The Model one wishes to remove.</param>
        /// <return>Void.</return>
        public new void Remove(TCModel modelIn)
        {
            Remove(model => model.Id == modelIn.Id);
            Remove(model => model.DiscordId == modelIn.DiscordId);
        }
    }
}