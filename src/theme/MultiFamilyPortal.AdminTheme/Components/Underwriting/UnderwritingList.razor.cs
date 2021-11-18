using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;
using Telerik.Blazor.Components;
using MultiFamilyPortal.Collections;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingList
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ILogger<UnderwritingList> Logger { get; set; }

        [Inject]
        public NavigationManager _navigationManager { get; set; }

        private CreateUnderwritingPropertyRequest NewProspect;

        [Parameter]
        public ObservableRangeCollection<ProspectPropertyResponse> Prospects { get; set; }

        private void CreateProperty()
        {
            NewProspect = new CreateUnderwritingPropertyRequest();
        }

        private async Task StartUnderwriting()
        {
            try
            {
                using var response = await _client.PostAsJsonAsync("/api/admin/underwriting/create", NewProspect);
                var property = await response.Content.ReadFromJsonAsync<ProspectPropertyResponse>();
                Prospects.Add(property);
                NavigateToProperty(property);
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void ViewProperty(GridCommandEventArgs args)
        {
            var property = args.Item as ProspectPropertyResponse;
            NavigateToProperty(property);
        }

        private void NavigateToProperty(ProspectPropertyResponse property)
        {
            _navigationManager.NavigateTo($"/admin/underwriting/property/{property.Id}");
        }
    }
}
