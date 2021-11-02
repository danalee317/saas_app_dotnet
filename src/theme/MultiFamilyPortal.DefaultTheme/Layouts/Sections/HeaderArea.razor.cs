using System.Security.Claims;
using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.DefaultTheme.Layouts.Sections
{
    public partial class HeaderArea
    {
        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; } = default !;
        [CascadingParameter]
        public ClaimsPrincipal User { get; set; } = default !;
    }
}
