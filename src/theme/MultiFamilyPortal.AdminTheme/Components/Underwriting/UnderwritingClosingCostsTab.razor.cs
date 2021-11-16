using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingClosingCostsTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }
    }
}
