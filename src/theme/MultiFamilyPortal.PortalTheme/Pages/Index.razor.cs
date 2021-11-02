using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.PortalTheme.Pages
{
    public partial class Index
    {
        [Inject]
        private SignInManager<SiteUser> _signinManager { get; set; } = default !;
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default !;
        [Inject]
        private NavigationManager _navigationManager { get; set; } = default !;
        private ElementReference submitForm { get; set; }

        private string _encodedUrl => HttpUtility.UrlEncode($"/account/login?username={Input.Email}&error=1");
        private LoginRequest Input { get; set; } = new LoginRequest();
        private IEnumerable<AuthenticationScheme> ExternalLogins { get; set; }



        private async Task SignInAsync(EditContext editContext)
        {
            await JSRuntime.InvokeVoidAsync("MFPortal.SubmitForm", submitForm);
        }
    }
}
