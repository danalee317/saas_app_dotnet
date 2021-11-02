using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.CoreUI
{
    public partial class SiteTitle
    {
        [Parameter]
        public string Title { get; set; }

        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; } = default !;
        private string FormattedTitle()
        {
            var siteTitle = SiteInfo.Title;
            if (string.IsNullOrEmpty(siteTitle))
            {
                siteTitle = "MultiFamily Portal";
            }

            if (string.IsNullOrEmpty(Title))
            {
                return $"{siteTitle} - {Title}";
            }

            return siteTitle;
        }
    }
}
