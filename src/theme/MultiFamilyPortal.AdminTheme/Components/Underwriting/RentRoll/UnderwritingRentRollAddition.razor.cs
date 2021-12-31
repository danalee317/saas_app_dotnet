using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.RentRoll
{
    public partial class UnderwritingRentRollAddition
    {
        [Parameter]
        public EventCallback<UnderwritingAnalysisModel> UpdateModel { get; set; }

        [Parameter]
        public UnderwritingAnalysisModel Model { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        private async Task OnNavigateBack() => await OnCancel.InvokeAsync();

        public UnderwritingAnalysisUnit _unit  = new();

        protected override void OnInitialized() => _unit.Rent = Model.CurrentRent;

        private async Task UpdateUnitList(UnderwritingAnalysisUnit unit)
        {
            if (Model.Units == null)
                Model.Units = new List<UnderwritingAnalysisUnit>(); 

            unit = _unit;
            Model.Units.Add(unit);
            await UpdateModel.InvokeAsync(Model);   
        }
    }
}
