using System.Collections.ObjectModel;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactsTab
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private readonly ObservableRangeCollection<CRMContact> _contacts = new();
        private bool _showList = true;
        private CRMContact _newContact = null;

        protected override async Task OnInitializedAsync()
        {
            await UpdateContacts();
        }

        private async Task UpdateContacts()
        {
            var contacts = await _client.GetFromJsonAsync<IEnumerable<CRMContact>>("/api/admin/contacts/crm-contacts");
            _contacts.ReplaceRange(contacts);
        }

        private void AddNewContact()
        {
            _newContact = new CRMContact
            {
                Addresses = new ObservableCollection<CRMContactAddress>(),
                Emails = new ObservableCollection<CRMContactEmail>(),
                Phones = new ObservableCollection<CRMContactPhone>(),
                Logs = new ObservableRangeCollection<CRMContactLog>(),
                Markets = new ObservableRangeCollection<CRMContactMarket>(),
                Roles = new ObservableCollection<CRMContactRole>(),
            };
        }

        private void ShowContactDetails(CRMContact contact)
        {
            _navigationManager.NavigateTo($"details/{contact.Id}");
        }
    }
}
