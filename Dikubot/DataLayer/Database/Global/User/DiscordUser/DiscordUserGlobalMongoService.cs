using System.Collections.Generic;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.DataLayer.Database.Interfaces;
using Discord;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Global.User.DiscordUser;

public class DiscordUserGlobalMongoService : GlobalMongoService<DiscordUserGlobalModel>, IIndexed<DiscordUserGlobalModel>
{
    public DiscordUserGlobalMongoService(Database database) : base(database)
    {
    }

    public IEnumerable<CreateIndexModel<DiscordUserGlobalModel>> GetIndexes()
    {
        CreateIndexOptions options = new CreateIndexOptions { Unique = true };
        return new List<CreateIndexModel<DiscordUserGlobalModel>>
        {
            new(Builders<DiscordUserGlobalModel>.IndexKeys.Ascending(model => model.DiscordId), options),
            new(Builders<DiscordUserGlobalModel>.IndexKeys.Ascending(model => model.Email))
        };
    }

    public DiscordUserGlobalModel Get(SocketUser user)
    {
        return Get(user.Id.ToString());
    }

    public DiscordUserGlobalModel Get(ulong discordId)
    {
        return Get(discordId.ToString());
    }

    public DiscordUserGlobalModel Get(string discordId)
    {
        DiscordUserGlobalModel mainModel = Get(inmodel => inmodel.DiscordId == discordId);
        if (mainModel == null)
        {
            mainModel = new DiscordUserGlobalModel();
            mainModel.DiscordId = discordId;
        }

        return mainModel;
    }

    public bool Exists(SocketUser user)
    {
        return Exists(model => model.DiscordId == user.Id.ToString());
    }

    public bool Exists(IUser user)
    {
        return Exists(model => model.DiscordId == user.Id.ToString());
    }

    public bool EmailExists(string email)
    {
        email = email.ToLower();
        return Exists(model => model.Email == email);
    }

    public DiscordUserGlobalModel GetFromEmail(string email)
    {
        email = email.ToLower();
        return Get(model => model.Email == email);
    }

    public override string GetCollectionName()
    {
        return "Users";
    }

    /// <Summary>
    ///     Inserts a UserModel in the collection. If a UserModel with the same ID or discordID
    ///     already exists, then we imply invoke Update() on the model instead.
    /// </Summary>
    /// <param name="mainModelIn">The Model one wishes to be inserted.</param>
    /// <return>A Model.</return>
    public new DiscordUserGlobalModel Upsert(DiscordUserGlobalModel mainModelIn)
    {
        bool idCollision = Exists(model => model.Id == mainModelIn.Id);
        bool discordIdCollision = Exists(model => model.DiscordId == mainModelIn.DiscordId);

        if (idCollision)
        {
            Update(mainModelIn, new ReplaceOptions { IsUpsert = true });
            return mainModelIn;
        }

        if (discordIdCollision)
        {
            Update(m => m.DiscordId == mainModelIn.DiscordId, mainModelIn, new ReplaceOptions { IsUpsert = true });
            return mainModelIn;
        }

        Insert(mainModelIn);
        return mainModelIn;
    }

    /// <Summary>Removes a UserModel from the collection by it's unique elements.</Summary>
    /// <param name="mainModelIn">The Model one wishes to remove.</param>
    /// <return>Void.</return>
    public void Remove(UserGuildModel mainModelIn)
    {
        Remove(model => model.Id == mainModelIn.Id);
        Remove(model => model.DiscordId == mainModelIn.DiscordId);
    }
}