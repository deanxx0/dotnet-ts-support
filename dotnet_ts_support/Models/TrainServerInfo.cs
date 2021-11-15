using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dotnet_ts_support.Models
{
    public class TrainServerInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; init; }
        public int serverIndex { get; init; }
        public string uri { get; init; }
    }
}
