using dotnet_ts_support.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace dotnet_ts_support.Services
{
    public class TrainServerInfoService
    {
        private readonly IMongoCollection<TrainServerInfo> _trainServerInfos;

        public TrainServerInfoService(IDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DBName);
            _trainServerInfos = db.GetCollection<TrainServerInfo>("trainServerInfos");
        }

        public TrainServerInfo Create(TrainServerInfo trainServerInfo)
        {
            _trainServerInfos.InsertOne(trainServerInfo);
            return trainServerInfo;
        }

        public List<TrainServerInfo> Get() => _trainServerInfos.Find(ti => true).ToList();

        public TrainServerInfo Get(int serverIndex) => _trainServerInfos.Find(ti => ti.serverIndex == serverIndex).FirstOrDefault();

    }
}
