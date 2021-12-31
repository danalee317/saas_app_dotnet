using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.RentRoll
{
    public partial class UnderwritingFloorPlans
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        private UnderwritingAnalysisModel _floorPlan;

        private void OnAddFloor()
        {
            _floorPlan = new UnderwritingAnalysisModel();
        }

        private void OnEditFloor(GridCommandEventArgs args)
        {
            var model = args.Item as UnderwritingAnalysisModel;
            _floorPlan = new UnderwritingAnalysisModel
            {
                Id = model.Id,
                Area = model.Area,
                Baths = model.Baths,
                Beds = model.Beds,
                CurrentRent = model.CurrentRent,
                MarketRent = model.MarketRent,
                Name = model.Name,
                TotalUnits = model.TotalUnits,
                Units = model.Units,
                Upgraded = model.Upgraded,
            };
        }
    }
}
