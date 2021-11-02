using MultiFamilyPortal.DefaultTheme.Layouts;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.DefaultTheme
{
    public class DefaultTheme : IPortalFrontendTheme
    {
        public Type Layout { get; } = typeof(MainLayout);
        public string Name { get; } = "Quarter Real Estate";
        public Type _404 { get; } = typeof(Components._404);
        public string[] RequiredStyles { get; } = new[] { "_content/MultiFamilyPortal.DefaultTheme/css/style.css" };
    }
}
