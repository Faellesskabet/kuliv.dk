using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database;

/// <summary>
/// TODO: Implement system for foreign keys and detecting when the foreign object no longer exists
/// </summary>
/// <typeparam name="T"></typeparam>
public class ForeignKey<Service, Model> where Service : Services<Model> where Model : MainModel
{
    public ForeignKey(Guid guid)
    {
        ForeignId = guid;
    }
    public ForeignKey(Model model)
    {
        ForeignId = model.Id;
    }

    [BsonElement("ForeignId")]
    private Guid ForeignId { get; set; }

    public Model Get(Service service)
    {
        return service.Get(ForeignId);
    }

    public bool IsRelevant(Service service)
    {
        return service.Exists(ForeignId);
    }
}

public class ForeignKeySet<Service, Model> where Service : Services<Model> where Model : MainModel
{
    [BsonElement("ForeignKeys")]
    private HashSet<ForeignKey<Service, Model>> ForeignKeys { get; set; }

    public IEnumerable<Guid> GetForeignIds(Service service) =>
        GetModels(service).Select(model => model.Id);

    public HashSet<Model> GetModels(Service service)
    {
        HashSet<Model> models = new HashSet<Model>();
        foreach (var key in ForeignKeys)
        {
            Model model = key.Get(service);
            if (model == null)
            {
                ForeignKeys.Remove(key);
                continue;
            }

            models.Add(model);
        }

        return models;
    }

    public void Add(ForeignKey<Service, Model> key)
    {
        ForeignKeys.Add(key);
    }

    public void Set(IEnumerable<Guid> ids)
    {
        ForeignKeys = new HashSet<ForeignKey<Service, Model>>(ids.Select(guid => new ForeignKey<Service, Model>(guid)));
    }
}