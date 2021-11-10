using dotnet_ts_support.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_ts_support.Services
{
    public class TrainService
    {
        private readonly IMongoCollection<Train> _trains;

        public TrainService(IDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DBName);
            _trains = db.GetCollection<Train>("trains");
        }

        public Train Create(Train train)
        {
            _trains.InsertOne(train);
            return train;
        }

        public List<Train> Get() => _trains.Find(train => true).ToList();

        public Train Get(string id) => _trains.Find(train => train.id == id).FirstOrDefault();

        public void Update(string id, Train trainIn) => _trains.ReplaceOne(train => train.id == id, trainIn);

        public void Remove(string id) => _trains.DeleteOne(train => train.id == id);
    }
}
