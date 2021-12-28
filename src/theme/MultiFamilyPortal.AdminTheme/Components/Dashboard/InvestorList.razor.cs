using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Dashboard
{
    public partial class InvestorList
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public IEnumerable<InvestorProspect> Investors { get; set; }

        [Parameter]
        public EventCallback<List<InvestorProspect>> InvestorsChanged { get; set; }

        private List<InvestorProspect> _contactedInvestors = new List<InvestorProspect>();
        private InvestorProspect _selectedInvestor;
        private bool _showDetail = false;

        //TODO : Find a way to fix the changing time issue
        private void ShowContact(InvestorProspect investor)
        {
            _selectedInvestor = investor;
            _showDetail = true;
        }

        private void ProcessContact(InvestorProspect investor)
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