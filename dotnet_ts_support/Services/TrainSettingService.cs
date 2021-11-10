using dotnet_ts_support.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_ts_support.Services
{
    public class TrainSettingService
    {
        private readonly IMongoCollection<TrainSetting> _trainSettings;

        public TrainSettingService(IDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DBName);
            _trainSettings = db.GetCollection<TrainSetting>("trainSettings");
        }

        public TrainSetting Create(TrainSetting trainSetting)
        {
            _trainSettings.InsertOne(trainSetting);
            return trainSetting;
        }

        public List<TrainSetting> Get() => _trainSettings.Find(trainSetting => true).ToList();

        public TrainSetting Get(string id) => _trainSettings.Find(trainSetting => trainSetting.id == id).FirstOrDefault();

        public void Update(string id, TrainSetting trainSettingIn) => _trainSettings.ReplaceOne(trainSetting => trainSetting.id == id, trainSettingIn);

        public void Remove(string id) => _trainSettings.DeleteOne(trainSetting => trainSetting.id == id);
    }
}
