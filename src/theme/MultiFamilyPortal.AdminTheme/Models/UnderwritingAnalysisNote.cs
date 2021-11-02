namespace MultiFamilyPortal.AdminTheme.Models
{
    public class UnderwritingAnalysisNote
    {
        public Guid Id { get; set; }
        public string UnderwriterId { get; set; }
        public string Underwriter { get; set; }
        public string UnderwriterEmail { get; set; }
        public string Note { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
