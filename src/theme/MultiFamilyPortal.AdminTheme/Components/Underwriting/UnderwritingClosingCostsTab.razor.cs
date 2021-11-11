using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwrting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingClosingCostsTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }
    }
}
