using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.CoreUI;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Pages.Users
{
    [Authorize(Roles = PortalRoles.PortalAdministrator)]
    public partial class Users
    {
        public Users()
        {
            var type = typeof(PortalRoles);
            _roles = type.GetFields(BindingFlags.Static | BindingFlags.Public).Select(x => new SelectableRole
            {
                Display = x.Name.Humanize(LetterCasing.Title),
                Role = x.Name
            });
        }

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ILogger<Users> Logger { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal User { get; set; }

        private PortalNotification notification { get; set; }

        private readonly IEnumerable<SelectableRole> _roles;
        private readonly ObservableRangeCollection<UserAccountResponse> _data = new ObservableRangeCollection<UserAccountResponse>();
        private CreateUserRequest _createUser;
        private CreateUserRequest _editUser;
        private string _editId;
        protected override async Task OnInitializedAsync()
        {
            _data.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<UserAccountResponse>>("/api/admin/users"));
        }

        private void OnAddUser()
        {
            _createUser = new CreateUserRequest();
        }

        private bool CanCreateNewUser()
        {
            if (_createUser is null || string.IsNullOrEmpty(_createUser.Email) || string.IsNullOrEmpty(_createUser.FirstName) || string.IsNullOrEmpty(_createUser.LastName) || !(_createUser.Roles?.Any() ?? false))
                return false;

            return true;
        }

        private async Task OnCreateNewUser()
        {
            try
            {
                Logger.LogInformation("Creating new user account");
                using var response = await _client.PostAsJsonAsync("/api/admin/users/create", _createUser);
                Logger.LogInformation($"User creation : {response.StatusCode}");

                switch(response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
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
            }
            catch (Exception ex)
            {
                Logger.LogError($"User password creation failed : {ex.Message}");
            }
            _createUser = null;
            await OnInitializedAsync();
        }

        private async Task DeleteUser(GridCommandEventArgs args)
        {
            var user = args.Item as UserAccountResponse;

            try
            {
                Logger.LogInformation($"Deleting user account {user.Email}");
                using var response = await _client.DeleteAsync($"/api/admin/users/{user.Id}");
                Logger.LogInformation($"User deletion : {response.StatusCode}");

                switch (response.StatusCode)
                {
                    case HttpStatusCode.NoContent:
                        notification.ShowSuccess("The user was deleted successfully");
                        await OnInitializedAsync();
                        break;
                    case HttpStatusCode.NotFound:
                        notification.ShowWarning("The user was not found");
                        break;
                    default:
                        notification.ShowError("An unknown error occurred while deleting the user");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"User deletion failed : {ex.Message}");
            }
        }

        private void OnEditUser(GridCommandEventArgs args)
        {
            var user = args.Item as UserAccountResponse;
            _editId = user.Id;
            
            _editUser = new CreateUserRequest
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone= user.Phone,
                UseLocalAccount = user.LocalAccount,
                Roles = user.Roles.ToList()
            };
        }

        private async Task UpdateUser()
        {
            try
            {
                Logger.LogInformation("Updating user");
                using var response = await _client.PutAsJsonAsync($"/api/admin/users/{_editId}", _editUser);
                Logger.LogInformation($"User update : {response.StatusCode}");

                 switch (response.StatusCode)
                {
                    case HttpStatusCode.NoContent:
                        notification.ShowSuccess("The user was updated successfully");
                        await OnInitializedAsync();
                        break;
                    case HttpStatusCode.NotFound:
                        notification.ShowWarning("The user was not found");
                        break;
                    default:
                        notification.ShowError("An unknown error occurred while updating the user");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"User update failed : {ex.Message}");
            }

            _editUser = null;
        }

        public class SelectableRole
        {
            public string Display { get; set; }

            public string Role { get; set; }
        }
    }
}
