using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.DefaultTheme.Pages
{
    public partial class TermsConditions
    {
        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; }
    }
}
