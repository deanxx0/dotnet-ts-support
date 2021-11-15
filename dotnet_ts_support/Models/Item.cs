using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dotnet_ts_support.Models
{
    public enum Status
    {
        Ready,
        Processing,
        Done
    }

    public class Item
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string outputDir { get; set; }
        public string tempDir { get; set; }
        public string filePath { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Status status { get; set; }
    }
}
