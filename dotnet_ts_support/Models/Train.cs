using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dotnet_ts_support.Models
{
    public class Train
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; init; }
        public int serverIndex { get; init; }
        public string name { get; init; }
        public string serverTrainId { get; init; }
        public string directoryId { get; init; }
        public string trainSettingId { get; init; }

        public Train() { }
    }
}
