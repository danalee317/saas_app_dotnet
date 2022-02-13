using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactRemindersTab
    {
        [Parameter]
        public CRMContact Contact { get; set; }
        private List<CRMContactReminder> _reminders = new();
        private CRMContactReminder _newReminder = null;
        private CRMContactReminder _selectedReminder = null;
        private CRMContactReminder _tempReminder = null;
        private bool _confirmation = false;
        private string _time;
        private string _status;
        private string _type;
        private int _pageSize = 10;
        private int _page = 1;
        private readonly string[] _times = new[] { "All Time", "Today", "This Week", "This Month" };
        private readonly string[] _statuses = new[] { "All", "Active", "Dismissed" };
        private readonly string[] _types = new[] { "All", "Mine", "System" };

        protected override void OnParametersSet()
        {
            _time = _times[0];
            _status = _statuses[1];
            _type = _types[0];
            Filter();
        }

        private void showReminder()
        {
            _newReminder = new CRMContactReminder
            {
                ContactId = Contact.Id,
                Date = DateTime.Now,
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
            _reminders = Contact.Reminders?.ToList();
        }

        private void EditLoop(CRMContactReminder reminder)
        {
            _selectedReminder = reminder;
            _tempReminder = new();
            _tempReminder.Date = _selectedReminder.Date;
            _tempReminder.Dismissed = _selectedReminder.Dismissed;
            _tempReminder.Description = _selectedReminder.Description;
        }

        private void HandleValidEdit() => _selectedReminder = null;

        private void HandleOnCancel()
        {
            _selectedReminder.Date = _tempReminder.Date;
            _selectedReminder.Description = _tempReminder.Description;
            _selectedReminder.Dismissed = _tempReminder.Dismissed;
            _selectedReminder = null;
        }

        private void RemoveReminder()
        {
            Contact.Reminders.Remove(_selectedReminder);
            _reminders = Contact.Reminders?.ToList();
            _selectedReminder = null;
        }

        private void OnSearch(ChangeEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Value.ToString()))
            {
                _reminders = Contact.Reminders?.Where(r => r.Description.ToLower().Contains(args.Value.ToString().ToLower())).ToList();
            }
            else
            {
                _reminders = Contact.Reminders?.ToList();
            }
        }

        private void KeyboardEventHandler(KeyboardEventArgs args)
        {
            if (args.Code == "Escape")
                _reminders = Contact.Reminders?.ToList();
        }

        private void FilterByTime(ChangeEventArgs args)
        {
            _time = args.Value.ToString();
            Filter();
        }
        private void FilterByStatus(ChangeEventArgs args)
        {
            _status = args.Value.ToString();
            Filter();
        }

        private void FilterByType(ChangeEventArgs args)
        {
            _type = args.Value.ToString();
            Filter();
        }

        private void Filter()
        {
            var date = DateTime.Now.Date;
            switch (_time)
            {
                case "All Time":
                    _reminders = Contact.Reminders?.ToList();
                    break;

                case "Today":
                    _reminders = Contact.Reminders?.Where(r => r.Date.Date == date).ToList();
                    break;

                case "This Week":
                    _reminders = Contact.Reminders?.Where(r => r.Date >= date.AddDays(-(int)date.DayOfWeek) && r.Date < date.AddDays(-(int)date.DayOfWeek).AddDays(7)).ToList();
                    break;

                case "This Month":
                    _reminders = Contact.Reminders?.Where(r => r.Date.Month == date.Month && r.Date.Year == date.Year).ToList();
                    break;
            }

            if (_status == "Active")
                _reminders = _reminders?.Where(r => r.Dismissed == false).ToList();
            else if (_status == "Dismissed")
                _reminders = _reminders?.Where(r => r.Dismissed == true).ToList();

            if (_type == "Mine")
                _reminders = _reminders?.Where(r => r.SystemGenerated == false).ToList();
            else if (_type == "System")
                _reminders = _reminders?.Where(r => r.SystemGenerated == true).ToList();
        }
    }
}
