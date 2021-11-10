using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_ts_support.Models
{
    public class TrainInfoModel
    {
        public string id { get; init; }
        public int serverIndex { get; init; }
        public string serverTrainId { get; init; }
        public string name { get; init; }
        public string status { get; init; }
        public int progress { get; init; }
        public string createdAt { get; init; }
        public int train_loss { get; init; }
        public int test_loss { get; init; }
        public int test_accuracy { get; init; }
        public int iou { get; init; }
        public int iteration { get; init; }
        public int max_iteration { get; init; }
        public string directoryId { get; init; }
        public string trainSettingId { get; init; }

        public TrainInfoModel() { }
    }
}
