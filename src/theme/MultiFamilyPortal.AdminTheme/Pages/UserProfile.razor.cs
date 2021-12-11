using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Pages
{
    [Authorize(Policy = PortalPolicy.AdminPortalViewer)]
    public partial class UserProfile
    {
        [Inject]
        private HttpClient _client { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal _user { get; set; }

        private bool _siteUser => _user.IsInAnyRole(PortalRoles.BlogAuthor, PortalRoles.PortalAdministrator, PortalRoles.Underwriter);

        private SerializableUser SiteUser;
        private UnderwriterGoal Goals;
        private IEnumerable<EditableLink> Links;
        private PortalNotification notification;
        private ChangePasswordRequest ChangePassword = new();

        protected override async Task OnInitializedAsync()
        {
            SiteUser = await _client.GetFromJsonAsync<SerializableUser>("/api/admin/userprofile");
            Goals = SiteUser.Goals;

            var providers = await _client.GetFromJsonAsync<IEnumerable<SocialProvider>>("/api/admin/userprofile/social-providers");

            Links = providers.Select(x => new EditableLink {
                Icon = x.Icon,
                Id = x.Id,
                Name = x.Name,
                Placeholder = x.Placeholder,
                UriTemplate = x.UriTemplate,
                Value = SiteUser.SocialLinks.FirstOrDefault(l => l.SocialProviderId == x.Id)?.Value
            });
        }

        private async Task Update()
        {
            using var response = await _client.PostAsJsonAsync("/api/admin/userprofile/update/profile", SiteUser);

            if (response.IsSuccessStatusCode)
                notification.ShowSuccess("Profile Saved!");
            else
                notification.ShowWarning("Unexpected error occurred while saving the profile");
        }

        private async Task OnChangePassword(EditContext context)
        {
            var model = context.Model as ChangePasswordRequest;

            if (model.Password != model.ConfirmPassword)
            {
                notification.ShowError("The Password and Confirm Password Fields must match");
                return;
            }

            using var response = await _client.PostAsJsonAsync("/api/admin/userprofile/update/password", model);

            if (response.IsSuccessStatusCode)
                notification.ShowSuccess("Password updated");
            else
                notification.ShowWarning("Could not update password");
        }

        private class EditableLink : SocialProvider
        {
            public string Value { get; set; }
        }
    }
}
