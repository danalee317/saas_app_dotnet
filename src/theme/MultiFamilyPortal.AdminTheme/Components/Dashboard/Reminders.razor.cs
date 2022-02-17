using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.CoreUI;
using System.Net.Http.Json;

namespace MultiFamilyPortal.AdminTheme.Components.Dashboard
{
    public partial class Reminders
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ILogger<Reminders> _logger { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private IEnumerable<ContactReminder> _reminders = Array.Empty<ContactReminder>();
        private ContactReminder _selectedReminder;
        private PortalNotification notification { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await GetRemindersAsync();
        }

        private async Task GetRemindersAsync()
        {
            try
            {
                _reminders = await _client.GetFromJsonAsync<IEnumerable<ContactReminder>>("/api/admin/dashboard/reminders");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching reminders : " + DateTimeOffset.UtcNow);
            }
        }

        private void RedictToContact(Guid id)
        {
            _navigationManager.NavigateTo($"/admin/contacts/detail/{id}");
        }

        private async Task DismissReminderAsync()
        {
            _selectedReminder.Dismissed = true;
            try
            {
               await _client.PutAsJsonAsync($"/api/admin/dashboard/reminders/{_selectedReminder.Id}", _selectedReminder);
               notification.ShowSuccess("Reminder dismissed.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error dismissing reminder : " + DateTimeOffset.UtcNow);
                notification.ShowError("failed to dismiss reminder");
            }
            await GetRemindersAsync();
            _selectedReminder = null;
        }

        private void CornfirmReminder(ContactReminder selectedReminder) => _selectedReminder = selectedReminder;

    }
}