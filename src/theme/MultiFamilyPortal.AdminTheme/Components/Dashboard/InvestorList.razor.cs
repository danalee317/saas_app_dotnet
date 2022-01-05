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
        public EventCallback<DashboardInvestor> InvestorChanged { get; set; }

        private DashboardInvestor _selectedInvestor;
        private bool _showDetail = false;

        private void ShowContact(DashboardInvestor investor)
        {
            _selectedInvestor = investor;
            _showDetail = true;
        }

        private async Task SaveContactAsync(DashboardInvestor investor)
        {
            await InvestorChanged.InvokeAsync(investor);
            _showDetail = false;
        }
    }
}