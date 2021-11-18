
namespace dotnet_ts_support.Models
{
    public class TrainInfoModel
    {
        public string id { get; set; }
        public int serverIndex { get; set; }
        public string serverTrainId { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public double progress { get; set; }
        public string createdAt { get; set; }
        public double train_loss { get; set; }
        public double test_loss { get; set; }
        public double test_accuracy { get; set; }
        public double iou { get; set; }
        public int iteration { get; set; }
        public int max_iteration { get; set; }
        public string directoryId { get; set; }
        public string trainSettingId { get; set; }

        public TrainInfoModel(
            string id,
            int serverIndex,
            string serverTrainId,
            string name,
            string status,
            double progress,
            string createdAt,
            double train_loss,
            double test_loss,
            double test_accuracy,
            double iou,
            int iteration,
            int max_iteration,
            string directoryId,
            string trainSettingId
            ) 
        {
            this.id = id;
            this.serverIndex = serverIndex;
            this.serverTrainId = serverTrainId;
            this.name = name;
            this.status = status;
            this.progress = progress;
            this.createdAt = createdAt;
            this.train_loss = train_loss;
            this.test_loss = test_loss;
            this.test_accuracy = test_accuracy;
            this.iou = iou;
            this.iteration = iteration;
            this.max_iteration = max_iteration;
            this.directoryId = directoryId;
            this.trainSettingId = trainSettingId;
        }

        //public class TrainInfoModel
        //{
        //    public string id { get; init; }
        //    public int serverIndex { get; init; }
        //    public string serverTrainId { get; init; }
        //    public string name { get; init; }
        //    public string status { get; init; }
        //    public double progress { get; init; }
        //    public string createdAt { get; init; }
        //    public double train_loss { get; init; }
        //    public double test_loss { get; init; }
        //    public double test_accuracy { get; init; }
        //    public double iou { get; init; }
        //    public int iteration { get; init; }
        //    public int max_iteration { get; init; }
        //    public string directoryId { get; init; }
        //    public string trainSettingId { get; init; }

        //    public TrainInfoModel() { }
    }
}
