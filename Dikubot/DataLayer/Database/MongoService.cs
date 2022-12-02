using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dikubot.DataLayer.Database.Interfaces;
using Dikubot.DataLayer.Static;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Dikubot.DataLayer.Database
{
    /// <summary>
    /// Class for for retrieving information from a given collection.
    /// </summary>
    public abstract class MongoService<TModel> where TModel : MainModel
    {
        private readonly IMongoCollection<TModel> _collection;
        private readonly Database _database;

        /// <summary>
        /// Constructor for the services.
        /// </summary>
        protected MongoService(Database database, string databaseName)
        {
            this._database = database;
            this._collection = GetCollection(databaseName);
        }
        
        public abstract string GetCollectionName();
        
        /// <summary>
        /// Indexes the service's collection. This is a test
        /// </summary>
        public void IndexCollection()
        {
            try
            {
                // MongoDB is smart enough to not duplicate indexes (i think ...) 
                // not that it matters because we drop all indexes on the first init
                _collection.Indexes.DropAll();
                _collection.Indexes.CreateMany(((IIndexed<TModel>)this).GetIndexes());
            }
            catch (Exception e)
            {
                Logger.Debug($"Indexing failed {e.Message}");
            }
        }
        
        protected IMongoCollection<TModel> GetCollection()
        {
            return GetCollection($"KULIV_GLOBAL");
        }
        
        protected IMongoCollection<TModel> GetCollection(SocketGuild guild)
        {
            return GetCollection($"KULIV_{guild?.Id.ToString() ?? "NULL"}");
        }
        protected IMongoCollection<TModel> GetCollection(string databaseName)
        {
            return GetCollection(databaseName, null);
        }
        protected IMongoCollection<TModel> GetCollection(string databaseName, MongoDB.Driver.MongoCollectionSettings collectionSettings)
        {
            return _database.GetDatabase(databaseName)
                .GetCollection<TModel>(GetCollectionName(), collectionSettings);
        }
        

        /// <Summary>Retrieves all the elements in the collection.</Summary>
        /// <return>A list of some Model type.</return>
        public virtual List<TModel> Get() =>
            _collection.Find(_ => true).ToList();

        /// <Summary>Retrieves a element in the collection with the specified ID.</Summary>
        /// <param name="id">The ID of the searched for model.</param>
        /// <return>A Model.</return>
        public virtual TModel Get(Guid id) =>
            Get(model => model.Id == id);

        /// <Summary>Retrieves a element in the collection based on a custom filter</Summary>
        /// <param name="filter">The filter is what determines what is returned. Example of a  filter is: (model =>
        /// model.Id == id)</param>
        /// <return>A Model.</return>
        public virtual TModel Get(Expression<Func<TModel, bool>> filter)
        {
            try  
            {
                return _collection.Find(filter).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }
        
        /// <summary>
        /// Get the nth first elements.
        /// </summary>
        /// <param name="count">The maximal amount of elements that will be returned.</param>
        /// <returns></returns>
        public List<TModel> Get(int count) =>
            Get(_ => true, count, 0, null);
        
        public List<TModel> Get(int count, Expression<Func<TModel, object>> sortBy) =>
            Get(_ => true, count, 0, sortBy);

        /// <summary>
        /// Get the nth first element of page count. The pages have the size of count. So if page = 3, and count = 10, then it'll skip the 30 first elements.
        /// </summary>
        /// <param name="count">The maximal amount of elements that will be returned.</param>
        /// <param name="page">The page specifies how many elements will be skipped. Page size is page number multiplied by count.</param>
        /// <returns></returns>
        public List<TModel> Get(int count, int page) =>
            Get(_ => true, count, page, null);
        
        public List<TModel> Get(int count, int page, Expression<Func<TModel, object>> sortBy) =>
            Get(_ => true, count, page, sortBy);

        /// <summary>
        /// Get the nth first elements that matches the filter.
        /// </summary>
        /// <param name="filter">The filter is what determines what is returned. Example of a  filter is: (model =>
        /// model.Id == id)</param>
        /// <param name="count">The maximal amount of elements that will be returned. The method will only return as many elements as fit the filter.</param>
        /// <returns></returns>
        public List<TModel> Get(Expression<Func<TModel, bool>> filter, int count) => 
            Get(filter, count, 0, null);
        
        public List<TModel> Get(Expression<Func<TModel, bool>> filter, int count, Expression<Func<TModel, object>> sortBy) => 
            Get(filter, count, 0, sortBy);

        /// <summary>
        /// Get the nth first element of page count that matches the filter.
        /// The pages have the size of count. So if page = 3, and count = 10, then it'll skip the 30 first elements.
        /// </summary>
        /// <param name="filter">The filter is what determines what is returned. Example of a  filter is: (model =>
        /// model.Id == id)</param>
        /// <param name="count">The maximal amount of elements that will be returned. The method will only return as many elements as fit the filter.</param>
        /// <param name="page">The page specifies how many elements will be skipped. Page size is page number multiplied by count.</param>
        /// <returns></returns>
        public List<TModel> Get(Expression<Func<TModel, bool>> filter, int count, int page) =>
            Get(filter, count, page, null);
        public List<TModel> Get(Expression<Func<TModel, bool>> filter, int count, int page, Expression<Func<TModel, object>> sortBy)
        {
            try
            {
                if (sortBy != null)
                {
                    return _collection.Find(filter).SortByDescending(sortBy).Skip(count * page).Limit(count).ToList();
                }
                return _collection.Find(filter).Skip(count * page).Limit(count).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return new List<TModel>();
        }

        /// <Summary>Retrieves a list of elements in the collection.</Summary>
        /// <return>A list of some Model type.</return>
        public List<TModel> GetAll() =>
            GetAll(model => true);

        /// <Summary>Retrieves a list of elements in the collection based on a custom filter</Summary>
        /// <param name="filter">The filter is what determines what is returned. Example of a  filter is: (model =>
        /// model.Id == id)</param>
        /// <return>A list of some Model type.</return>
        public List<TModel> GetAll(Expression<Func<TModel, bool>> filter) =>
            _collection.Find<TModel>(filter).ToList();

        /// <Summary>Returns whether there exists which fits the filter</Summary>
        /// <param name="filter">The filter is what determines what is returned. Example of a  filter is: (model =>
        /// model.Id == id)</param>
        /// <return>A Boolean.</return>
        /// 
        public bool Exists(Expression<Func<TModel, bool>> filter) =>
            Get(filter) != null;

        /// <Summary>Returns whether there exists an element with a matching id</Summary>
        /// <param name="id">The ID of the searched for model.</param>
        /// <return>A Boolean.</return>
        /// 
        public bool Exists(Guid id) =>
            Get(id) != null;

        /// <Summary>Inserts a Model in the collection. If a model with the same ID already exists, then we simply
        /// invoke Update() on the model instead.</Summary>
        /// <param name="model">The Model one wishes to be inserted.</param>
        /// <return>A Model.</return>
        public virtual TModel Upsert(TModel model)
        {
            if (Exists(model.Id))
            {
                Update(model, new ReplaceOptions() {IsUpsert = true});
                return model;
            }

            Insert(model);
            return model;
        }

        /// <Summary>Inserts a Model in the collection. If there is a collision it will write an error.</Summary>
        /// <param name="model">The model which will be inserted.</param>
        /// <return>Void.</return>
        public virtual TModel Insert(TModel model)
        {
            try
            {
                _collection.InsertOne(model);
            }
            catch (MongoWriteException e)
            {
                Console.WriteLine("ILLEGAL INSERT OPERATION " + model.Id + " WAS NOT INSERTED");
                Console.WriteLine(e.StackTrace);
            }

            return model;
        }

        /// <Summary>Updates a Model in the collection.</Summary>
        /// <param name="predicate">The predicate used to replace with.</param>
        /// <param name="modelIn">The Model one wishes to Update with.</param>
        /// <return>Void.</return>
        public virtual void Update(Expression<Func<TModel, bool>> predicate, TModel modelIn, ReplaceOptions options = null)
        {
            try
            {
                TModel merged = BsonSerializer.Deserialize<TModel>(Get(predicate)
                    .ToBsonDocument()
                    .Merge((modelIn.ToBsonDocument()),
                        true));
                _collection?.ReplaceOne(predicate, merged, options);
            }
            catch (MongoWriteException e)
            {
                Console.WriteLine("ILLEGAL INSERT OPERATION " + modelIn.Id + " WAS NOT INSERTED");
                Console.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// Aggregation operations process data records and return computed results.
        /// Aggregation operations group values from multiple documents together,
        /// and can perform a variety of operations on the grouped data to return a single result.
        /// MongoDB provides three ways to perform aggregation: the aggregation pipeline, the map-reduce function, and single purpose aggregation methods.
        /// </summary>
        /// <param name="aggregateOptions">The options which will be used for the aggregation</param>
        /// <returns>Aggregation result</returns>
        public IAggregateFluent<TModel> Aggregate(AggregateOptions aggregateOptions) => 
            _collection.Aggregate(aggregateOptions);

        /// <summary>
        /// Returns n amount of samples from a given collection, selected at random
        /// </summary>
        /// <param name="amount">Amount of samples to be selected</param>
        /// <returns>The given amount of samples is randomly chosen from the collection and returned</returns>
        public List<TModel> GetSamples(int amount)
        {
            return _collection.AsQueryable().Sample(amount).ToList();
        }

        /// <Summary>Updates a Model in the collection.</Summary>
        /// <param name="model">The Model one wishes to Update with.</param>
        /// <return>Void.</return>
        public virtual void Update(TModel model, ReplaceOptions options = null) =>
            Update(m => m.Id == model.Id, model, options);

        /// <Summary>Removes a element from the collection.</Summary>
        /// <param name="modelIn">The Model one wishes to remove.</param>
        /// <return>Void.</return>
        public virtual void Remove(TModel modelIn) =>
            Remove(modelIn.Id);

        /// <Summary>Removes a element from the collection by ID.</Summary>
        /// <param name="id">The ID of the Model to be removed.</param>
        /// <return>Void.</return>
        public virtual void Remove(Guid id) =>
            _collection.DeleteOne(model => model.Id == id);

        /// <Summary>Removes all elements for which the predicate is true.</Summary>
        /// <param name="predicate">The predicate which is used to delete with.</param>
        /// <return>Void.</return>
        public virtual void Remove(Expression<Func<TModel, bool>> predicate) =>
            _collection.DeleteMany(predicate);

        /// <Summary>Removes all elements for which the predicate is true.</Summary>
        /// <param name="predicate">The predicate which is used to delete with.</param>
        /// <return>Void.</return>
        public virtual void RemoveAll(Expression<Func<TModel, bool>> predicate) =>
            _collection.DeleteMany(predicate);

        /// <summary>
        /// Get the estimated document count of the document.
        /// </summary>
        /// <returns></returns>
        public long EstimatedCount()
        {
            return _collection.EstimatedDocumentCount();
        }

    }
}