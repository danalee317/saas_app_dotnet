namespace MultiFamilyPortal.AdminTheme.Models
{
    public class DashboardAnalyticsResponse
    {
        public int ExpectedUnderwrittenLast7Days { get; set; }
        public int ExpectedUnderwrittenLast30Days { get; set; }
        public int UnderwrittenLast7Days { get; set; }
        public int UnderwrittenLast30Days { get; set; }
        public int Prospects7Days { get; set; }
        public int Prospects30Days { get; set; }
        public int ProspectsContacted7Days { get; set; }
        public int ProspectsContacted30Days { get; set; }

        public DashboardActivityResponse Activity { get; set; }
    }
}
