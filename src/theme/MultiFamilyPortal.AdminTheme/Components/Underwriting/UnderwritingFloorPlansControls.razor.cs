using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingFloorPlansControls
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

        private async Task AddFloorAsync()
        {
            if (string.IsNullOrEmpty(FloorPlan.Name))
                return;

            if(Property.Models == null)
            {
                Property.Models = new List<UnderwritingAnalysisModel>();
                Property.Models.Add(FloorPlan);
            } 
            else if (Property.Models.Any(x => !string.Equals(x.Name, FloorPlan.Name.Trim(), StringComparison.CurrentCultureIgnoreCase)))
            {
                Property.Models.Add(FloorPlan);
            }

            await FloorPlanChanged.InvokeAsync(Property);
        }

        private async Task EditFloorAsync()
        {
            if (string.IsNullOrEmpty(FloorPlan.Name))
                return;

            await FloorPlanChanged.InvokeAsync(Property);
        }

        private async Task DeleteFloorAsync()
        {
            Property.Models.Remove(Property.Models.FirstOrDefault(x => x.Name == FloorPlan.Name));
            await FloorPlanChanged.InvokeAsync(Property);
        }

        private void ClearCurrent() => FloorPlan = new UnderwritingAnalysisModel();

    }
}
