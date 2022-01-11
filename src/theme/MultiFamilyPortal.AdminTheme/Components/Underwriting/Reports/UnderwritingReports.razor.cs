using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.SaaS.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.Reports
{
    public partial class UnderwritingReports
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [CascadingParameter]
        private Tenant _tenant { get; set; }

        private string _reportName;
        private bool _comingSoon;

        private void SelectReport(string name)
        {
            _comingSoon = true;
            _reportName = name;

            if (_comingSoon)
                _reportName = null;

              //  Property.Rep
        }
    }
}
