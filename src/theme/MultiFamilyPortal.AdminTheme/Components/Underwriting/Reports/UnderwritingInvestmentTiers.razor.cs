using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.Reports
{
    public partial class UnderwritingInvestmentTiers
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }
    }
}
