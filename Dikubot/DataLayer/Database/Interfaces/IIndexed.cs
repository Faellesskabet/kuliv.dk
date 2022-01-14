using System.Collections.Generic;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Interfaces;

public interface IIndexed<TModel> where TModel : MainModel
{
    public IEnumerable<IndexKeysDefinition<TModel>> GetIndexes();
}