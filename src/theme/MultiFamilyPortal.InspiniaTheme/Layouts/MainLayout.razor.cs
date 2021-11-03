using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace MultiFamilyPortal.InspiniaTheme.Layouts
{
    public partial class MainLayout
    {
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; } = default !;
        private ClaimsPrincipal User = new ClaimsPrincipal();
        protected override async Task OnInitializedAsync()
        {
            var authState = await authenticationStateTask;
            User = authState.User;
        }
    }
}
