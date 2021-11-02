using Microsoft.AspNetCore.Authorization;
using MultiFamilyPortal.Authentication;

namespace MultiFamilyPortal.AdminTheme.Pages.Properties.AssetsUnderManagement
{
    [Authorize(Policy = PortalPolicy.Underwriter)]
    public partial class Index
    {
    }
}
