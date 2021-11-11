using System.Security.Claims;
using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Layouts.Sections
{
    public partial class HeaderArea
    {
        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; } = default !;

        [CascadingParameter]
        public ClaimsPrincipal User { get; set; } = default !;
    }
}
