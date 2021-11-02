using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.PortalTheme
{
    public class PortalTheme : IPortalFrontendTheme
    {
        public Type Layout { get; } = typeof(Layouts.MainLayout);
        public string Name { get; } = "Portal Theme";
        public Type _404 { get; } = typeof(Components._404);
        public string[] RequiredStyles { get; } = new[] { "_content/MultiFamilyPortal.PortalTheme/css/site.css" };
    }
}
