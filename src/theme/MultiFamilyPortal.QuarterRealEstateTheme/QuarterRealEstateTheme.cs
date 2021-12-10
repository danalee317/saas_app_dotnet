using MultiFamilyPortal.QuarterRealEstateTheme.Layouts;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.QuarterRealEstateTheme
{
    public class QuarterRealEstateTheme : IPortalFrontendTheme
    {
        private const string baseContentPath = "_content/MultiFamilyPortal.QuarterRealEstateTheme";
        public Type Layout { get; } = typeof(MainLayout);
        public string Name { get; } = "Quarter Real Estate";
        public Type _404 { get; } = typeof(Components._404);
        public string[] RequiredStyles { get; } = new[] { $"{baseContentPath}/css/style.css" };
        public ThemeResource[] Resources { get; } = new[]
        {
            new ThemeResource
            {
                Height = 773,
                Width = 870,
                Name = "default-home",
                Path = $"/{baseContentPath}/img/bg/default-home.png"
            }
        };
    }
}
