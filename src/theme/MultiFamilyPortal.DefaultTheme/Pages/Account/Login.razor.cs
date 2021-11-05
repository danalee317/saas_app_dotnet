using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Web;

namespace MultiFamilyPortal.DefaultTheme.Pages.Account
{
    public partial class Login
    {
        [Inject]
        private SignInManager<SiteUser> _signinManager { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        [Inject]
        private NavigationManager _navigationManager { get; set; } = default!;

        private ElementReference submitForm { get; set; }
        private string _encodedUrl => HttpUtility.UrlEncode($"/account/login?username={Input.Email}&error=1");

        private LoginRequest Input { get; set; } = new LoginRequest();

        private AuthenticationScheme _micrsoftScheme;
        private AuthenticationScheme _googleScheme;

        private ServerSideValidator serverSideValidator { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var externalSchemes = await _signinManager.GetExternalAuthenticationSchemesAsync();
            if (externalSchemes?.Any() ?? false)
            {
                _micrsoftScheme = externalSchemes.FirstOrDefault(x => x.Name == "Microsoft");
                _googleScheme = externalSchemes.FirstOrDefault(x => x.Name == "Google");
            }
        }

        private async Task SignInAsync(EditContext editContext)
        {
            await JSRuntime.InvokeVoidAsync("MFPortal.SubmitForm", submitForm);
        }
    }
}
