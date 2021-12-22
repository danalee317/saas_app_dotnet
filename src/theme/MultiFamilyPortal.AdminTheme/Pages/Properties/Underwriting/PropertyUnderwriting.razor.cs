using System.Collections.Specialized;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Pages.Properties.Underwriting
{
    [Authorize(Policy = PortalPolicy.UnderwritingViewer)]
    public partial class PropertyUnderwriting : IDisposable
    {
        [Parameter]
        public Guid propertyId { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal _user { get; set; }

        private UnderwritingAnalysis Property;
        private bool _disposedValue;
        private int currentIndex = 0;
        private bool showPrevious => currentIndex > 0;
        private bool showNext => currentIndex < 4;

        private PortalNotification notification { get; set; }
        private bool _editable;

        private readonly IEnumerable<UnderwritingStatus> AvailableStatus = Enum.GetValues<UnderwritingStatus>();

        protected override async Task OnInitializedAsync()
        {
            _editable = _user.IsAuthorizedInPolicy(PortalPolicy.Underwriter);
            Property = await _client.GetFromJsonAsync<UnderwritingAnalysis>($"/api/admin/underwriting/property/{propertyId}");
            _navigationManager.LocationChanged += OnNavigating;
            if(Property.OurExpense is INotifyCollectionChanged ncc)
            {
                ncc.CollectionChanged += OnCollectionChanged;
            }
        }

        private void OnNavigating(object sender, LocationChangedEventArgs e)
        {
            _navigationManager.LocationChanged -= OnNavigating;
            //Property.Update();
            //DbContext.Update(Property);
            //DbContext.SaveChanges();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            StateHasChanged();
        }

        private void Next() => currentIndex++;

        private void Previous() => currentIndex--;

        private void OnTabChanged()
        {
            //Property.Update();
        }

        private void OnExpensesUpdated()
        {
            StateHasChanged();
        }

        private async Task OnUpdateProperty()
        {
            // TODO : find out why units is received in end point as null
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

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _navigationManager.LocationChanged -= OnNavigating;
                    if (Property?.OurExpense is INotifyCollectionChanged ncc)
                    {
                        ncc.CollectionChanged -= OnCollectionChanged;
                    }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
