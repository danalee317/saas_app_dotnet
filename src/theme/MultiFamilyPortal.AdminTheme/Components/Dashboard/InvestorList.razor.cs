using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Dashboard
{
    public partial class InvestorList
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public IEnumerable<DashboardInvestor> Investors { get; set; }

        [Parameter]
        public EventCallback<List<DashboardInvestor>> InvestorsChanged { get; set; }

        private List<DashboardInvestor> _contactedInvestors = new List<DashboardInvestor>();
        private DashboardInvestor _selectedInvestor;
        private bool _showDetail = false;

        private void ShowContact(DashboardInvestor investor)
        {
            _selectedInvestor = investor;
            _showDetail = true;
        }

        private void ProcessContact(DashboardInvestor investor)
        {
            if (_contactedInvestors.Contains(investor))
            {
                investor.Contacted = false;
               _contactedInvestors.Remove(investor);
            } 
            else
            {
                investor.Contacted = true;
               _contactedInvestors.Add(investor);
            }
        }

        private async Task SaveContactAsync()
        {
            if(_contactedInvestors.Count() == 0)
             return;

             await InvestorsChanged.InvokeAsync(_contactedInvestors);
        }
    }
}