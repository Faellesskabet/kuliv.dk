using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Dikubot.Database.Models
{
    /// <summary>
    /// Class for for retrieving information from a given collection.
    /// </summary>
    public abstract class Services<TModel> where TModel : Model
    {

        private readonly IMongoCollection<TModel> _models;
        private readonly Dictionary<string, bool> indexed = new Dictionary<string, bool>();

        /// <summary>
        /// Constructor for the services.
        /// </summary>
        protected Services(string databaseName,
            string collectionName,
            MongoDatabaseSettings databaseSettings = null,
            MongoCollectionSettings collectionSettings = null)
        {
            // The database to retrieve from.
            IMongoDatabase database = Database.GetInstance.GetDatabase(databaseName, databaseSettings);
            // The collection to retrieve from.
            _models = database.GetCollection<TModel>(collectionName, collectionSettings);
            
            //No reason to index the collection every single time
            string indexKey = databaseName + ";" + collectionName;
            if (!indexed.ContainsKey(indexKey))
            {
                SetUniqueIndexes(_models);
                indexed[indexKey] = true;
            }
        }

        /// <Summary>Retrieves all the elements in the collection.</Summary>
        /// <return>A list of some Model type.</return>
        public List<TModel> Get() =>
            _models.Find(model => true).ToList();

        /// <Summary>Retrieves a element in the collection with the specified ID.</Summary>
        /// <param name="id">The ID of the searched for model.</param>
        /// <return>A Model.</return>
        public TModel Get(string id) =>
            Get(model => model.Id == id);

        /// <Summary>Retrieves a element in the collection based on a custom filter</Summary>
        /// <param name="filter">The filter is what determines what is returned. Example of a  filter is: (model =>
        /// model.Id == id)</param>
        /// <return>A Model.</return>
        public TModel Get(Expression<Func<TModel, bool>> filter)
        {
            try
            {
                return _models.Find<TModel>(filter).FirstOrDefault();
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
        public bool Exists(string id) =>
            Get(id) != null;

        /// <Summary>Inserts a Model in the collection. If a model with the same ID already exists, then we imply
        /// invoke Update() on the model instead.</Summary>
        /// <param name="model">The Model one wishes to be inserted.</param>
        /// <return>A Model.</return>
        public TModel Upsert(TModel model)
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
        public TModel Insert(TModel model) 
        { 
            try
            {
                _models.InsertOne(model);
            }
            catch (MongoDB.Driver.MongoWriteException e)
            {
                Console.WriteLine("ILLEGAL INSERT OPERATION " + model.Id + " WAS NOT INSERTED");
            }
            return model;
        }

        /// <Summary>Updates a Model in the collection.</Summary>
        /// <param name="predicate">The predicate used to replace with.</param>
        /// <param name="modelIn">The Model one wishes to Update with.</param>
        /// <return>Void.</return>
        public void Update(Expression<Func<TModel, bool>> predicate, TModel modelIn, ReplaceOptions options = null)
        {
            try
            {
                _models.ReplaceOne(predicate, modelIn, options);
            }
            catch (MongoWriteException e)
            {
                Console.WriteLine("ILLEGAL INSERT OPERATION " + modelIn + " WAS NOT INSERTED");
            }
        }

        /// <Summary>Updates a Model in the collection.</Summary>
        /// <param name="model">The Model one wishes to Update with.</param>
        /// <return>Void.</return>
        public void Update(TModel model, ReplaceOptions options = null) =>
            Update(m => m.Id == model.Id, model, options);
        
        /// <Summary>Removes a element from the collection.</Summary>
        /// <param name="modelIn">The Model one wishes to remove.</param>
        /// <return>Void.</return>
        public void Remove(TModel modelIn) =>
            Remove(modelIn.Id);

        /// <Summary>Removes a element from the collection by ID.</Summary>
        /// <param name="id">The ID of the Model to be removed.</param>
        /// <return>Void.</return>
        public void Remove(string id) => 
            _models.DeleteOne(model => model.Id == id);
        
        /// <Summary>Sets up the unique indexes for the current collection and service.</Summary>
        /// <param name="collection">The collection where we setup the Unique indexes.</param>
        /// <return>Void.</return>
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
                            {   Unique = true, 
                                Sparse = true
                            }));
            }

        }
    }
}