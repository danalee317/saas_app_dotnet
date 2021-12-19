using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Pages.Contacts
{
    public partial class ContactDetails
    {
        [Parameter]
        public Guid id { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private CRMContact _contact;
        private bool _showMarketInfo;

        protected override async Task OnInitializedAsync()
        {
            _contact = await _client.GetFromJsonAsync<CRMContact>($"/api/admin/contacts/crm-contact/{id}");
        }

        private void OnNavigateBack()
        {
            _navigationManager.NavigateTo("/admin/contacts");
        }
    }
}
