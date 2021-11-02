using System.Net;
using System.Text;

namespace MultiFamilyPortal.Data.Models
{
    public class Subscriber
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; } = true;

        private DateTimeOffset _timestamp = DateTimeOffset.UtcNow;
        public DateTimeOffset Timestamp => _timestamp;

        public string UserAgent { get; set; }

        public string UserAgentFamily { get; set; }

        public string UserAgentVersion { get; set; }

        public string UserAgentOSFamily { get; set; }

        public string UserAgentDeviceBrand { get; set; }

        public string UserAgentDeviceFamily { get; set; }

        public string UserAgentDeviceModel { get; set; }

        public bool UserAgentIsSpider { get; set; }

        public IPAddress IpAddress { get; set; }

        public string Type { get; set; }

        public string Continent { get; set; }

        public string Country { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        public string Source { get; set; }

        public string Medium { get; set; }

        public string Campaign { get; set; }

        public string Term { get; set; }

        public string Content { get; set; }

        public DateTimeOffset? Unsubscribed { get; set; }

        private Guid _confirmationCode = Guid.NewGuid();
        public Guid ConfirmationCode => _confirmationCode;

        public List<Post> Notifications { get; set; }

        public string UnsubscribeCode() =>
            Convert.ToBase64String(Encoding.Default.GetBytes($"{Email}:{ConfirmationCode}"));
    }
}
