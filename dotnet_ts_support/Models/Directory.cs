using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_ts_support.Models
{
    public class Directory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; init; }
        public string[] directories { get; init; }

        public Directory() { }

    }
}
