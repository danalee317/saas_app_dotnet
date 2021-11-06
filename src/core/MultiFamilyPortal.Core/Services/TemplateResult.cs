namespace MultiFamilyPortal.Services
{
    public record TemplateResult
    {
        public string Subject { get; init; }
        public string Html { get; init; }
        public string PlainText { get; init; }
    }
}
