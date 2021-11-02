using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.DefaultTheme.Pages
{
    public partial class PrivacyPolicy
    {
        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; }
    }
}
