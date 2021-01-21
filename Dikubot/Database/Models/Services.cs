using System.Collections.Generic;
using MongoDB.Driver;

namespace Dikubot.Database.Models
{
    /// <summary>
    /// Class for for retrieving information from a given collection.
    /// </summary>
    public abstract class Services<TModel> where TModel : Model
    {
        
        private readonly IMongoCollection<TModel> _models;

        /// <summary>
        /// Constructor for the services.
        /// </summary>
        protected Services(string databaseName,
                        string collectionName, 
                        MongoDatabaseSettings databaseSettings = null,
                        MongoCollectionSettings collectionSettings = null)
        {
            // The database to retrieve from.
            var database = Database.GetInstance.GetDatabase(databaseName, databaseSettings);
            // The collection to retrieve from.
            _models = database.GetCollection<TModel>(collectionName, collectionSettings);
        }

        /// <Summary>Retrieves all the elements in the collection.</Summary>
        /// <return>A list of some Model type.</return>
        public List<TModel> Get() =>
            _models.Find(model => true).ToList();

        /// <Summary>Retrieves a element in the collection with the specified ID.</Summary>
        /// <param name="id">The ID of the searched for model.</param>
        /// <return>A Model.</return>
        public TModel Get(string id) =>
            _models.Find<TModel>(model => model.Id == id).FirstOrDefault();

        /// <Summary>Inserts a Model in the collection.</Summary>
        /// <param name="model">The Model one wishes to be inserted.</param>
        /// <return>A Model.</return>
        public Model Create(TModel model)
        {
            _models.InsertOne(model);
            return model;
        }

        /// <Summary>Inserts a Model in the collection.</Summary>
        /// <param name="id">The ID of the Model to be updated.</param>
        /// <param name="model">The Model one wishes to Update with.</param>
        /// <return>Void.</return>
        public void Update(string id, TModel modelIn) =>
            _models.ReplaceOne(model => modelIn.Id == id, modelIn);

        /// <Summary>Removes a element from the collection.</Summary>
        /// <param name="model">The Model one wishes to remove.</param>
        /// <return>Void.</return>
        public void Remove(TModel modelIn) =>
            _models.DeleteOne(user => user.Id == modelIn.Id);

        /// <Summary>Removes a element from the collection by ID.</Summary>
        /// <param name="id">The ID of the Model to be removed.</param>
        /// <return>Void.</return>
        public void Remove(string id) => 
            _models.DeleteOne(model => model.Id == id);
    }
}