
namespace dotnet_ts_support.Models
{
    public class TrainMetricModel
    {
        public string train_id { get; init; }
        public int max_iteration { get; init; }
        public int current_iteration { get; init; }
        public double train_loss { get; init; }
        public double test_accuracy { get; init; }
        public double test_loss { get; init; }
        public double test_accuracy2 { get; init; }

        public TrainMetricModel() { }
    }
}
