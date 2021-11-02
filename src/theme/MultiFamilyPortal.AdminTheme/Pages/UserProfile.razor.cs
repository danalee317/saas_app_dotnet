using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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

        private SiteUser SiteUser;
        private UnderwriterGoal Goals;
        private IEnumerable<EditableLink> Links;
        private PortalNotification notification;
        private ChangePasswordRequest ChangePassword = new();
        protected override async Task OnInitializedAsync()
        {
            SiteUser = await _client.GetFromJsonAsync<SiteUser>("/api/admin/userprofile");
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

        private void Update()
        {
            notification.ShowSuccess("Profile Saved!");
        }

        private async Task OnChangePassword(EditContext context)
        {
            var model = context.Model as ChangePasswordRequest;

            if (model.Password != model.ConfirmPassword)
                System.Diagnostics.Debugger.Break();
        }

        private class EditableLink : SocialProvider
        {
            public string Value { get; set; }
        }
    }
}
