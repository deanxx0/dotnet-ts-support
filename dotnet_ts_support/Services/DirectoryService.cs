using dotnet_ts_support.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_ts_support.Services
{
    public class DirectoryService
    {
        private readonly IMongoCollection<Directory> _directories;

        public DirectoryService(IDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DBName);
            _directories = db.GetCollection<Directory>("directories");
        }

        public Directory Create(Directory directory)
        {
            _directories.InsertOne(directory);
            return directory;
        }

        public List<Directory> Get() => _directories.Find(directory => true).ToList();

        public Directory Get(string id) => _directories.Find(directory => directory.id == id).FirstOrDefault();

        public void Update(string id, Directory directoryIn) => _directories.ReplaceOne(directory => directory.id == id, directoryIn);

        public void Remove(string id) => _directories.DeleteOne(directory => directory.id == id);
    }
}
