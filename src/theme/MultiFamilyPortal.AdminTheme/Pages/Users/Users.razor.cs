using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.CoreUI;

namespace MultiFamilyPortal.AdminTheme.Pages.Users
{
    [Authorize(Roles = PortalRoles.PortalAdministrator)]
    public partial class Users
    {
        public Users()
        {
            var type = typeof(PortalRoles);
            _roles = type.GetFields(BindingFlags.Static | BindingFlags.Public).Select(x => x.Name);
        }

        [Inject]
        private HttpClient _client { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal User { get; set; }

        private PortalNotification notification { get; set; }

        private readonly IEnumerable<string> _roles;
        private readonly ObservableRangeCollection<UserAccountResponse> _data = new ObservableRangeCollection<UserAccountResponse>();
        private CreateUserRequest _createUser;

        protected override async Task OnInitializedAsync()
        {
            _data.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<UserAccountResponse>>("/api/admin/users"));
        }

        private void OnAddUser()
        {
            _createUser = new CreateUserRequest();
        }

        private async Task OnCreateNewUser()
        {
            using var response = await _client.PostAsJsonAsync("/api/admin/users/create", _createUser);
            switch(response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    notification.ShowWarning("The user already exists");
                    break;
                case HttpStatusCode.NoContent:
                    notification.ShowError("Failed to create user");
                    break;
                case HttpStatusCode.OK:
                    notification.ShowSuccess("The user was created successfully");
                    break;
                default:
                    notification.ShowError("An unknown error occurred while created the user");
                    break;
            }
            _createUser = null;
            await OnInitializedAsync();
        }
    }
}
