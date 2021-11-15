using dotnet_ts_support.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace dotnet_ts_support.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DBName);
            _users = db.GetCollection<User>("users");
        }

        public List<User> Get() => _users.Find(user => true).ToList();

        public User Get(string id) => _users.Find(user => user.id == id).FirstOrDefault();

        public User GetByName(string name) => _users.Find(user => user.username == name).FirstOrDefault();

        public User Create(User userIn)
        {
            _users.InsertOne(userIn);
            return userIn;
        }

        public void Update(string id, User userIn) => _users.ReplaceOne(user => user.id == id, userIn);

        public void Remove(string id) => _users.DeleteOne(user => user.id == id);
    }
}
