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
        public EventCallback<List<InvestorProspect>> OnInvestorSaved { get; set; }

        private List<InvestorProspect> _ConatactedInvestors = new List<InvestorProspect>();
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
            // TODO : add the contact to list
        }

         private void SaveContact()
        {
            // TODO : add the contact to list
        }
    }
}