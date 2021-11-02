using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class DashboardActivityResponse
    {
        public TimeSpan Total { get; set; }
        public Dictionary<ActivityType, TimeSpan> Breakdown { get; set; }
    }
}
