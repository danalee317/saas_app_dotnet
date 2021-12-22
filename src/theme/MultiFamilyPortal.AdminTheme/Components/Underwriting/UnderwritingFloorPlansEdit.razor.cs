using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingFloorPlansEdit
    {
        [Parameter]
        public bool ShowWindow { get; set; }

        [Parameter]
        public bool IsNew { get; set; }

        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Parameter]
        public UnderwritingAnalysisModel FloorPlan { get; set; }

        [Parameter]
        public EventCallback FloorPlanChanged { get; set; }

        private async Task PropertyUpdated()
        {
            ShowWindow = false;
            await FloorPlanChanged.InvokeAsync(Property);
        }
    }
}
