using dotnet_ts_support.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace dotnet_ts_support.Services
{
    public class DirectoryService
    {
        private readonly IMongoCollection<Models.Directory> _directories;

        public DirectoryService(IDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DBName);
            _directories = db.GetCollection<Models.Directory>("directories");
        }

        public Models.Directory Create(Models.Directory directory)
        {
            _directories.InsertOne(directory);
            return directory;
        }

        public List<Models.Directory> Get() => _directories.Find(directory => true).ToList();

        public Models.Directory Get(string id) => _directories.Find(directory => directory.id == id).FirstOrDefault();

        public void Update(string id, Models.Directory directoryIn) => _directories.ReplaceOne(directory => directory.id == id, directoryIn);

        public void Remove(string id) => _directories.DeleteOne(directory => directory.id == id);

        public string[] GetDatasets()
        {
            if (!System.IO.Directory.Exists(@"Y:/ts/datasets"))
                return null;
            else
            {
                var datasetsFullPath = System.IO.Directory.GetDirectories(@"Y:/ts/datasets");
                List<string> datasets = new List<string>();
                for(int i = 0; i < datasetsFullPath.Length; i++)
                {
                    var datasetName = new DirectoryInfo(datasetsFullPath[i]).Name;
                    datasets.Add(datasetName);
                }
                return datasets.ToArray();
            }
        }

        public string[] GetPretrains()
        {
            if (!System.IO.Directory.Exists(@"Y:/ts/PreTrained/Venus"))
                return null;
            else
            {
                var pretrainsFullPath = System.IO.Directory.GetFiles(@"Y:/ts/PreTrained/Venus");
                List<string> pretrains = new List<string>();
                for(int i = 0; i < pretrainsFullPath.Length; i++)
                {
                    var pretrainName = new DirectoryInfo(pretrainsFullPath[i]).Name;
                    pretrains.Add(pretrainName);
                }
                return pretrains.ToArray();
            }
        }
    }
}
