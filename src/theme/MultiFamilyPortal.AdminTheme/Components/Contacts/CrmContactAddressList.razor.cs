using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactAddressList
    {
        [Parameter]
        public CRMContact Contact { get; set; }

        private PortalNotification notification;
        private readonly IEnumerable<CRMContactAddressType> _addressTypes = Enum.GetValues<CRMContactAddressType>();
        private CRMContactAddress _newAddress = new CRMContactAddress();

        private void AddAddress()
        {
            var address = new CRMContactAddress 
            {
                ContactId = Contact.Id,
                Address1 = _newAddress.Address1.Trim(),
                Address2 = _newAddress.Address2.Trim(),
                City = _newAddress.City.Trim(),
                State = _newAddress.State.Trim(),
                PostalCode = _newAddress.PostalCode.Trim(),
                Type = _newAddress.Type,
            };
            Contact.Addresses.Add(address);
            _newAddress = new ();
        }

        private async Task SetPrimaryAddress(CRMContactAddress address)
        {
            var addresses = Contact.Addresses.ToArray();
            Contact.Addresses = new List<CRMContactAddress>();
            foreach (var record in addresses)
            {
                record.Primary = record.Address1 == address.Address1;
            }
            await Task.Delay(1);
            Contact.Addresses = new List<CRMContactAddress>(addresses);
        }

        private void OnDeleteAddress(CRMContactAddress address)
        {
            if (address is not null && Contact.Addresses.Contains(address))
                Contact.Addresses.Remove(address);
        }

        private bool CanAddAddress()
        {
            if (string.IsNullOrEmpty(_newAddress.Address1) ||
                string.IsNullOrEmpty(_newAddress.Address2) ||
                string.IsNullOrEmpty(_newAddress.City) ||
                string.IsNullOrEmpty(_newAddress.State) ||
                string.IsNullOrEmpty(_newAddress.PostalCode) ||
                Contact.Addresses.Any(x => x.Address1.Trim().ToLowerInvariant() == _newAddress.Address1.Trim().ToLowerInvariant() ||
                Contact.Addresses.Any(x => x.Address2.Trim().ToLowerInvariant() == _newAddress.Address2.Trim().ToLowerInvariant()) ||
                Contact.Addresses.Any(x => x.Address1.Trim().ToLowerInvariant() == _newAddress.Address2.Trim().ToLowerInvariant()) ||
                Contact.Addresses.Any(x => x.Address2.Trim().ToLowerInvariant() == _newAddress.Address1.Trim().ToLowerInvariant())
                ))
                return false;

            return true;
        }
    }
}
