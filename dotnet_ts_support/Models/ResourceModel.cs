
namespace dotnet_ts_support.Models
{
    public class ResourceModel
    {
        public UsageInfo cpu { get; init; }
        public UsageInfo gpu { get; init; }
        public UsageInfo ram { get; init; }

        public ResourceModel() { }
    }

    public class UsageInfo
    {
        public string name { get; init; }
        public string total { get; init; }
        public double usage { get; init; }

        public UsageInfo() { }
    }
}
