using System.ComponentModel.DataAnnotations;

namespace p2p.models
{
    public class P2PItems
    {
        public string? Id { get; set; } = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid()}";
        public string? DeviceName { get; set; }
        public string? DeviceType { get; set; }
        public string? DeviceIp { get; set; }

    }

    public class P2PItemsDto
    {
        public string? DeviceName { get; set; }
        public string? DeviceType { get; set; }
        public string? DeviceIp { get; set; }
    }

}