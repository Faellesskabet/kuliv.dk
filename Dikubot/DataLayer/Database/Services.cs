using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dikubot.DataLayer.Database.Interfaces;
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
    public abstract class Services<TModel> where TModel : MainModel
    {
        internal readonly IMongoCollection<TModel> _models;
        private readonly Dictionary<string, bool> indexed = new Dictionary<string, bool>();
        private readonly Dictionary<string,IMongoCollection<TModel>> _allDatabases = new Dictionary<string, IMongoCollection<TModel>>();
        private readonly Dictionary<string,IMongoCollection<TModel>> _userDatabases = new Dictionary<string, IMongoCollection<TModel>>();
        
        public readonly MongoDatabaseSettings DatabaseSettings;
        public readonly MongoCollectionSettings CollectionSettings;
        public readonly IMongoDatabase database;

        /// <summary>
        /// Constructor for the services.
        /// </summary>
        protected Services(string databaseName,
            string collectionName,
            MongoDatabaseSettings databaseSettings = null,
            MongoCollectionSettings collectionSettings = null)
        {
            this.DatabaseSettings = databaseSettings;
            this.CollectionSettings = collectionSettings;
            
            // The database to retrieve from.
            this.database = Database.GetInstance.GetDatabase($"{databaseName}", databaseSettings);
            
            // The collection to retrieve from.
            _models = SetModels(this.database, collectionName, collectionSettings);

            //No reason to index the collection every single time
            string indexKey = databaseName + ";" + collectionName;
            if (this is IIndexed<TModel> && !indexed.ContainsKey(indexKey))
            {
                // this dropall is temporary. It is inefficient that we drop all the indexes, just to set them again
                // but because of some breaking indexes in the past, we have to do this until the database has been updated
                _models.Indexes.DropAll();
                // MongoDB is smart enough to not duplicate indexes (i think ...) 
                _models.Indexes.CreateMany(((IIndexed<TModel>)this).GetIndexes().Select(definition => new CreateIndexModel<TModel>(definition)));
                indexed[indexKey] = true;
            }
        }


        protected virtual IMongoCollection<TModel> SetModels(IMongoDatabase database, string collectionName, MongoDB.Driver.MongoCollectionSettings collectionSettings )
        {
            return database.GetCollection<TModel>(collectionName, collectionSettings);
        }
        


        /// <Summary>Retrieves all the elements in the collection.</Summary>
        /// <return>A list of some Model type.</return>
        public virtual List<TModel> Get() =>
            _models.Find(_ => true).ToList();

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
                return _models.Find(filter).FirstOrDefault();
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
            Get(_ => true, count, 0);

        /// <summary>
        /// Get the nth first element of page count. The pages have the size of count. So if page = 3, and count = 10, then it'll skip the 30 first elements.
        /// </summary>
        /// <param name="count">The maximal amount of elements that will be returned.</param>
        /// <param name="page">The page specifies how many elements will be skipped. Page size is page number multiplied by count.</param>
        /// <returns></returns>
        public List<TModel> Get(int count, int page) =>
            Get(_ => true, count, page);

        /// <summary>
        /// Get the nth first elements that matches the filter.
        /// </summary>
        /// <param name="filter">The filter is what determines what is returned. Example of a  filter is: (model =>
        /// model.Id == id)</param>
        /// <param name="count">The maximal amount of elements that will be returned. The method will only return as many elements as fit the filter.</param>
        /// <returns></returns>
        public List<TModel> Get(Expression<Func<TModel, bool>> filter, int count) => 
            Get(filter, count, 0);

        /// <summary>
        /// Get the nth first element of page count that matches the filter.
        /// The pages have the size of count. So if page = 3, and count = 10, then it'll skip the 30 first elements.
        /// </summary>
        /// <param name="filter">The filter is what determines what is returned. Example of a  filter is: (model =>
        /// model.Id == id)</param>
        /// <param name="count">The maximal amount of elements that will be returned. The method will only return as many elements as fit the filter.</param>
        /// <param name="page">The page specifies how many elements will be skipped. Page size is page number multiplied by count.</param>
        /// <returns></returns>
        public List<TModel> Get(Expression<Func<TModel, bool>> filter, int count, int page)
        {
            try
            {
                return _models.Find(filter).Skip(count * page).Limit(count).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        /// <Summary>Retrieves a list of elements in the collection based on a custom filter</Summary>
        /// <param name="filter">The filter is what determines what is returned. Example of a  filter is: (model =>
        /// model.Id == id)</param>
        /// <return>A list of some Model type.</return>
        public List<TModel> GetAll(Expression<Func<TModel, bool>> filter) =>
            _models.Find<TModel>(filter).ToList();

        /// THIS DOES NOT BELONG HERE AT ALL
        /// <Summary>Don't use this. Retrieves a Dictionary of a list of elements in the collection based on a custom filter</Summary>
        /// <param name="filter">The filter is what determines what is returned. Example of a  filter is: (model =>
        /// model.Id == id)</param>
        /// <return>A of a list list of some Model type.</return>
        [Obsolete]
        public virtual Dictionary<string, List<TModel>> GetAllAsDictionary(Expression<Func<TModel, bool>> filter = null)
        {
            var res = new Dictionary<string, List<TModel>>();
            foreach (var collection in _allDatabases)
            {
                if (filter == null)
                {
                    var item = collection.Value.Find(_ => true).ToList();
                    res.Add(collection.Key,item);
                }
                else
                {
                    var item = collection.Value.Find(filter).ToList();
                    res.Add(collection.Key,item);
                }
            }
            return res;
        }
        
        /// <Summary>Retrieves a Dictionary of a list of elements in the collection based on a custom filter</Summary>
        /// <param name="filter">The filter is what determines what is returned. Example of a  filter is: (model =>
        /// model.Id == id)</param>
        /// <return>A of a list list of some Model type.</return>
        public virtual Dictionary<string, List<TModel>> GetAllWithUser(Expression<Func<TModel, bool>> filter = null)
        {
            var res = new Dictionary<string, List<TModel>>();
            foreach (var collection in _userDatabases)
            {
                if (filter == null)
                {
                    var item = collection.Value.Find(_ => true).ToList();
                    res.Add(collection.Key,item);
                }
                else
                {
                    var item = collection.Value.Find(filter).ToList();
                    res.Add(collection.Key,item);
                }
                
            }
            return res;
        }

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
                _models.InsertOne(model);
            }
            catch (MongoWriteException e)
            {
                Console.WriteLine("ILLEGAL INSERT OPERATION " + model.Id + " WAS NOT INSERTED");
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
                _models?.ReplaceOne(predicate, merged, options);
            }
            catch (MongoWriteException e)
            {
                Console.WriteLine("ILLEGAL INSERT OPERATION " + modelIn.Id + " WAS NOT INSERTED");
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
            _models.Aggregate(aggregateOptions);

        /// <summary>
        /// Returns n amount of samples from a given collection, selected at random
        /// </summary>
        /// <param name="amount">Amount of samples to be selected</param>
        /// <returns>The given amount of samples is randomly chosen from the collection and returned</returns>
        public List<TModel> GetSamples(int amount)
        {
            return _models.AsQueryable().Sample(amount).ToList();
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
            _models.DeleteOne(model => model.Id == id);

        /// <Summary>Removes a element from the collection by a predicate.</Summary>
        /// <param name="predicate">The predicate which is used to delete with.</param>
        /// <return>Void.</return>
        public virtual void Remove(Expression<Func<TModel, bool>> predicate) =>
            _models.DeleteMany(predicate);

        /// <Summary>Removes all elements for which the predicate is true.</Summary>
        /// <param name="predicate">The predicate which is used to delete with.</param>
        /// <return>Void.</return>
        public virtual void RemoveAll(Expression<Func<TModel, bool>> predicate) =>
            _models.DeleteMany(predicate);

        /// <summary>
        /// Get the estimated document count of the document.
        /// </summary>
        /// <returns></returns>
        public long EstimatedCount()
        {
            return _models.EstimatedDocumentCount();
        }
        
        /// <Summary>Sets up the unique indexes for the current collection and service.</Summary>
        /// <param name="collection">The collection where we setup the Unique indexes.</param>
        /// <return>Void.</return>
        [Obsolete]
        private void SetUniqueIndexes(IMongoCollection<TModel> collection)
        {
            // It's time for some fun reflection!
            Type type = typeof(TModel); // Get the type of our model, an example could be UserModel
            IEnumerable<PropertyInfo> uniques = type.GetProperties().Where(
                // We retrieve all the functions/properties which have the BsonUnique attribute
                prop => Attribute.IsDefined(prop, typeof(BsonUniqueAttribute)));

            foreach (PropertyInfo property in uniques) //We loop over our properties
            {
                BsonElementAttribute bsonElementAttribute = //We get the BsonElement attribute
                    (BsonElementAttribute) Attribute.GetCustomAttribute(property, typeof(BsonElementAttribute));
                if (bsonElementAttribute == null) //Continue if the function doesn't have a BsonElement attribute
                    continue;

                /*
                 * We create a unique and sparse index for the element name found in our BsonElement attribute.
                 */
                collection.Indexes.CreateOne(
                    new CreateIndexModel<TModel>(new IndexKeysDefinitionBuilder<TModel>()
                            .Ascending(bsonElementAttribute.ElementName),
                        new CreateIndexOptions<TModel>
                        {
                            Unique = true,
                            Sparse = true
                        }));
            }
        }
    }
}