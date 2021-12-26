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

        private IEnumerable<CRMContact> _allContacts;
        private readonly ObservableRangeCollection<CRMContact> _contacts = new();
        private readonly ObservableRangeCollection<CRMContactRole> _roles = new();
        private bool _showList = true;
        private CRMContact _newContact = null;
        private string _query;
        private string _selectedRole;

        protected override async Task OnInitializedAsync()
        {
            var roles = await _client.GetFromJsonAsync<IEnumerable<CRMContactRole>>("/api/admin/contacts/crm-roles");
            _roles.Clear();
            _roles.Add(new CRMContactRole
            {
                Name = "All"
            });
            _roles.AddRange(roles);
            _selectedRole = _roles.First().Name;
            await UpdateContacts();
        }

        private async Task UpdateContacts()
        {
            _allContacts = await _client.GetFromJsonAsync<IEnumerable<CRMContact>>("/api/admin/contacts/crm-contacts");
            FilterResults();
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

        private async Task OnNewContactSaved(CRMContact contact)
        {
            _newContact = contact;
            await UpdateContacts();
        }

        private void ShowContactDetails(CRMContact contact)
        {
            _navigationManager.NavigateTo($"/admin/contacts/detail/{contact.Id}");
        }

        private void OnQueryChanged(string query)
        {
            _query = query;
            FilterResults();
        }

        private void OnRoleTypeChanged()
        {
            FilterResults();
        }

        private void FilterResults()
        {
            var filtered = _allContacts;
            if(!string.IsNullOrEmpty(_query))
                filtered = filtered.Where(x =>
                    x.FirstName.Contains(_query, StringComparison.InvariantCultureIgnoreCase) ||
                    x.LastName.Contains(_query, StringComparison.InvariantCultureIgnoreCase) ||
                    x.Company.Contains(_query, StringComparison.InvariantCultureIgnoreCase) ||
                    x.Emails.Any(e => e.Email.Contains(_query, StringComparison.InvariantCultureIgnoreCase)));

            if (_roles.First(x => x.Name == _selectedRole).Id != default)
                filtered = filtered.Where(x => x.Roles.Any(x => x.Name == _selectedRole));

            _contacts.ReplaceRange(filtered.OrderBy(x => x.LastName));
        }
    }
}
