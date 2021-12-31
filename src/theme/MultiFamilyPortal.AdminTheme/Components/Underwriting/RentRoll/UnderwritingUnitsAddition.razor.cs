using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.RentRoll
{
    public partial class UnderwritingUnitsAddition
    {
        [Parameter]
        public UnderwritingAnalysisModel FloorPlan { get; set; }

        [Parameter]
        public EventCallback<UnderwritingAnalysisModel> UpdateModel { get; set; }

        private bool _showAddUnit = false;
        private void ShowAddUnit() => _showAddUnit = true;

        private void ClearAddUnit() => _showAddUnit = false;

        private async  Task OnAddUnit(UnderwritingAnalysisModel model) => await UpdateModel.InvokeAsync(model);
    }
}
