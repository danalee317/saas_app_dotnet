using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwrting;

namespace MultiFamilyPortal.AdminTheme.Pages.Properties.Underwriting
{
    [Authorize(Policy = PortalPolicy.Underwriter)]
    public partial class PropertyUnderwriting
    {
        [Parameter]
        public Guid propertyId { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private UnderwritingAnalysis Property;
        private PortalNotification notification { get; set; }

        private readonly IEnumerable<UnderwritingStatus> AvailableStatus = Enum.GetValues<UnderwritingStatus>();

        protected override async Task OnInitializedAsync()
        {
            Property = await _client.GetFromJsonAsync<UnderwritingAnalysis>($"/api/admin/underwriting/property/{propertyId}");
            _navigationManager.LocationChanged += OnNavigating;
        }

        private void OnNavigating(object sender, LocationChangedEventArgs e)
        {
            _navigationManager.LocationChanged -= OnNavigating;
            //Property.Update();
            //DbContext.Update(Property);
            //DbContext.SaveChanges();
        }

        private void OnTabChanged()
        {
            //Property.Update();
        }

        private async Task OnUpdateProperty()
        {
            using var response = await _client.PostAsJsonAsync<UnderwritingAnalysis>($"/api/admin/underwriting/update/{propertyId}", Property);

            if(response.IsSuccessStatusCode)
            {
                notification.ShowSuccess("Property successfully updated");
                Property = await response.Content.ReadFromJsonAsync<UnderwritingAnalysis>();
            }
            else
            {
                notification.ShowWarning("An error occurred while trying to save the updated underwrting");
            }
        }
    }
}
