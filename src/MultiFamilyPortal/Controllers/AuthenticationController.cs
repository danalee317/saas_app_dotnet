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
        private SignInManager<SiteUser> _signinManager { get; } = default!;
        private UserManager<SiteUser> _userManager { get; } = default!;

        public AuthenticationController(SignInManager<SiteUser> signinManager, UserManager<SiteUser> userManager)
        {
            _signinManager = signinManager;
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

            var result = await _signinManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, loginRequest?.RememberMe ?? false, lockoutOnFailure: false);

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

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            if (_signinManager.IsSignedIn(User))
            {
                await _signinManager.SignOutAsync();
            }

            return Redirect("~/");
        }

    }
}
