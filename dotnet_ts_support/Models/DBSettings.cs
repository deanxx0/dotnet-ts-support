
namespace dotnet_ts_support.Models
{
    public class DBSettings : IDBSettings
    {
        public string ConnectionString { get; set; }
        public string DBName { get; set; }
    }

    public interface IDBSettings
    {
        string ConnectionString { get; set; }
        string DBName { get; set; }
    }
}
