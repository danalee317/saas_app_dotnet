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

        [Inject]
        public NavigationManager _navigationManager { get; set; }

        private void AdminPortal() => _navigationManager.NavigateTo("/admin", true);

        private void InvestorPortal() => _navigationManager.NavigateTo("investor-portal", true);
    }
}
