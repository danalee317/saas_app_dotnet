using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private SignInManager<SiteUser> _signInManager { get; } = default!;
        private UserManager<SiteUser> _userManager { get; } = default!;

        public AuthenticationController(SignInManager<SiteUser> signinManager, UserManager<SiteUser> userManager)
        {
            _signInManager = signinManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest loginRequest, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = "/";

            if(!ModelState.IsValid || string.IsNullOrEmpty(loginRequest?.Email))
            {
                return Redirect(returnUrl);
            }

            var result = await _signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, loginRequest?.RememberMe ?? false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(loginRequest.Email);
                if (await _userManager.IsInRoleAsync(user, PortalRoles.Investor) ||
                    await _userManager.IsInRoleAsync(user, PortalRoles.Sponsor))
                {
                    // navigate to Investor Portal
                    return Redirect("~/investor-portal");
                }
                else if (await _userManager.IsInRoleAsync(user, PortalRoles.PortalAdministrator) ||
                    await _userManager.IsInRoleAsync(user, PortalRoles.BlogAuthor) ||
                    await _userManager.IsInRoleAsync(user, PortalRoles.Underwriter) ||
                    await _userManager.IsInRoleAsync(user, PortalRoles.Mentor))
                {
                    // navigate to Admin Portal
                    return Redirect("~/admin");
                }
                else
                {
                    return Redirect("~/");
                }
            }
            else if (result.IsLockedOut)
            {
                // navigate to lockout page
                return Redirect("~/account/lockout");
            }
            else if (result.RequiresTwoFactor)
            {
                // handle 2fa
                throw new NotImplementedException();
            }
            else
            {
                // add model error
                return Redirect(returnUrl);
            }
        }

        [AllowAnonymous]
        [HttpPost("external")]
        public async Task<IActionResult> ExternalLogin([FromForm]AuthenticationScheme requestedScheme, string callbackUrl)
        {
            var externalSchemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            if (string.IsNullOrEmpty(callbackUrl))
                callbackUrl = "/";

            var scheme = externalSchemes.FirstOrDefault(x => x.Name == requestedScheme.Name);
            if (scheme == null)
                return Redirect(callbackUrl);

            var redirectUrl = $"{Request.Scheme}://{Request.Host}/api/authentication/callback";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(scheme.Name, redirectUrl);
            return new ChallengeResult(scheme.Name, properties);
        }

        [AllowAnonymous]
        [HttpGet("callback")]
        public async Task<IActionResult> ExternalCallback(string returnUrl = null, string remoteError = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info is null)
                return Redirect("/");

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.IsLockedOut)
                return Redirect("/account/lockout");

            if (!result.Succeeded)
                return Redirect("/");

            if (info.Principal.IsInAnyRole(PortalRoles.Underwriter, PortalRoles.Mentor, PortalRoles.BlogAuthor, PortalRoles.PortalAdministrator))
                return Redirect("/admin");
            else if (info.Principal.IsInAnyRole(PortalRoles.Investor, PortalRoles.Sponsor))
                return Redirect("/investor-portal");

            return Redirect("/");
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            if (_signInManager.IsSignedIn(User))
            {
                await _signInManager.SignOutAsync();
            }

            return Redirect("~/");
        }

    }
}
