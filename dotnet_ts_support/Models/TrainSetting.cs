using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dotnet_ts_support.Models
{
    public class TrainSetting
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; init; }
        public int batchSize { get; init; }
        public string pretrainData { get; init; }
        public int width { get; init; }
        public int height { get; init; }
        public int channels { get; init; }
        public double baseLearningRate { get; init; }
        public double gamma { get; init; }
        public int stepCount { get; init; }
        public int maxIteration { get; init; }

        public bool mirror { get; init; }
        public bool flip { get; init; }
        public bool rotation90 { get; init; }
        public double zoom { get; init; }
        public double tilt { get; init; }
        public double shift { get; init; }
        public double rotation { get; init; }
        public double contrast { get; init; }
        public double brightness { get; init; }
        public double smoothFiltering { get; init; }
        public double noise { get; init; }
        public double colorNoise { get; init; }
        public double partialFocus { get; init; }
        public double shade { get; init; }
        public double hue { get; init; }
        public double saturation { get; init; }
        public int maxRandomAugmentCount { get; init; }
        public double probability { get; init; }
        public int borderMode { get; init; }
        
        public TrainSetting() { }
    }
}
