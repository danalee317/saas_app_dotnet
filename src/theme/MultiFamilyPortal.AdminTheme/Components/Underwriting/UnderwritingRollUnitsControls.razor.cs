using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingRollUnitsControls
    {
        [Parameter]
        public UnderwritingAnalysisUnit Unit { get; set; }

        [Parameter]
        public EventCallback<UnderwritingAnalysisUnit> OnUnitChanged { get; set; }

        private async Task UpdateUnit()
        {
            if(string.IsNullOrEmpty(Unit.Unit))
                return;
            
            await OnUnitChanged.InvokeAsync(Unit);
        }
    }
}