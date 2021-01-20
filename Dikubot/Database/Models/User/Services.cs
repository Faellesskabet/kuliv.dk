using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Dikubot.Database.Models.User
{
    public class Services
    {
        public class BookService
        {
            private readonly IMongoCollection<Model> _users;

            public BookService()
            {
                var database = Database.GetInstance.GetDatabase("Users");
                _users = database.GetCollection<Model>("Users");
            }

            public List<Model> Get() =>
                _users.Find(book => true).ToList();

            public Model Get(string id) =>
                _users.Find<Model>(book => book.Id == id).FirstOrDefault();

            public Model Create(Model user)
            {
                _users.InsertOne(user);
                return user;
            }

            public void Update(string id, Model userIn) =>
                _users.ReplaceOne(user => userIn.Id == id, userIn);

            public void Remove(Model userIn) =>
                _users.DeleteOne(user => user.Id == userIn.Id);

            public void Remove(string id) => 
                _users.DeleteOne(user => user.Id == id);
        }
    }
}