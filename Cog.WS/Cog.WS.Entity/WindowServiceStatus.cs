namespace Cog.WS.Entity
{

    public interface IWindowServiceStatus
    {
        string Status { get; set; }
        string ErrorMessage { get; set; }
        string ServiceName { get; set; }
        string ServiceStatus { get; set; }
        string SystemName { get; set; }
        string MonitorStatus { get; set; }
    }
    public class WindowServiceStatus : IWindowServiceStatus
    {
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public string ServiceName { get; set; }
        public string ServiceStatus { get; set; }
        public string SystemName { get; set; }
        public string MonitorStatus { get; set; }
    }
}