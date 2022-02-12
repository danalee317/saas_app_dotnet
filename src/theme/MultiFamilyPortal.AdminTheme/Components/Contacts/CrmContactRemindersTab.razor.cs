using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactRemindersTab
    {
        [Parameter]
        public CRMContact Contact { get; set; }

        private CRMContactReminder _newReminder = null;
        private CRMContactReminder _selectedReminder = null;
        private CRMContactReminder _tempReminder = null;
        private bool _confirmation = false;

        private void showReminder()
        {
            _newReminder = new CRMContactReminder
            {
                ContactId = Contact.Id,
                Date = DateTime.Now
            };
        }

        private void HandleValidSubmit()
        {
            _newReminder.Description = _newReminder.Description.Trim();
            if (Contact.Reminders == null)
                Contact.Reminders = new List<CRMContactReminder> { _newReminder };
            else
                Contact.Reminders.Add(_newReminder);

            _newReminder = null;
        }

        private void EditLoop(CRMContactReminder reminder)
        {
            _selectedReminder = reminder;
            _tempReminder = new();
            _tempReminder.Date = _selectedReminder.Date;
            _tempReminder.Dismissed = _selectedReminder.Dismissed;
            _tempReminder.Description = _selectedReminder.Description;
        }

        private void HandleValidEdit()
        {
            _selectedReminder = null;
        }

        private void HandleOnCancel()
        {
            var old = Contact.Reminders.FirstOrDefault(r => r.Id == _tempReminder.Id);
            old.Date = _tempReminder.Date;
            old.Description = _tempReminder.Description;
            old.Dismissed = _tempReminder.Dismissed;
            _selectedReminder = null;
        }

        private void RemoveReminder()
        {
            Contact.Reminders.Remove(_selectedReminder);
            _selectedReminder = null;
        }
    }
}
