using MultiFamilyPortal.QuarterRealEstateTheme.Layouts;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.QuarterRealEstateTheme
{
    public class QuarterRealEstateTheme : IPortalFrontendTheme
    {
        public Type Layout { get; } = typeof(MainLayout);
        public string Name { get; } = "Quarter Real Estate";
        public Type _404 { get; } = typeof(Components._404);
        public string[] RequiredStyles { get; } = new[] { "_content/MultiFamilyPortal.QuarterRealEstateTheme/css/style.css" };
    }
}
