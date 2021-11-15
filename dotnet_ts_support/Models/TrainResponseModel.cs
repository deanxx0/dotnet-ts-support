using System;

namespace dotnet_ts_support.Models
{
    public class TrainResponseModel
    {
        public string id { get; set; }
        public string status { get; set; }
        public string error_message { get; set; }
        public DateTime created_time { get; set; }
        public double remain_time_ms { get; set; }
        public double process_time_ms { get; set; }

        public TrainResponseModel() { }
    }
}
