using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwrting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingSummaryTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }
    }
}
