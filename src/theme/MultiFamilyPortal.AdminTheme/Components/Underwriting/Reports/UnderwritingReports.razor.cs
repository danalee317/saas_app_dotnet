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

        [Inject]
        NavigationManager NavigationManager { get; set; }

        private string _reportName;
        private string _reportLink;
        private bool _comingSoon;

        private static readonly string[] _supported = new string[]
        {
            "manager-report",
            "cash-flow",
            "full-report",
        };

        private void SelectReport(string name)
        {
            _comingSoon = false;
            _reportName = name;

            if(!_supported.Contains(name))
               _comingSoon= true;

            _reportLink = $"{NavigationManager.BaseUri}api/reports/{_reportName}/{Property.Id}";

            if (_comingSoon)
                _reportName = null;
        }
    }
}
