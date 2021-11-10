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
        public int baseLearningRate { get; init; }
        public int gamma { get; init; }
        public int stepCount { get; init; }
        public int maxIteration { get; init; }

        public bool mirror { get; init; }
        public bool flip { get; init; }
        public bool rotation90 { get; init; }
        public int zoom { get; init; }
        public int tilt { get; init; }
        public int shift { get; init; }
        public int rotation { get; init; }
        public int contrast { get; init; }
        public int brightness { get; init; }
        public int smoothFiltering { get; init; }
        public int noise { get; init; }
        public int colorNoise { get; init; }
        public int partialFocus { get; init; }
        public int shade { get; init; }
        public int hue { get; init; }
        public int saturation { get; init; }
        public int maxRandomAugmentCount { get; init; }
        public int probability { get; init; }
        public int borderMode { get; init; }
        
        public TrainSetting() { }
    }
}
