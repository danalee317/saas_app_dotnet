using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingFloorPlans
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        private bool _showModel = false;
        private bool _isNew = false;
        private int _totalUnits = 0;
        private int _numberOfUnits = 0;
        private UnderwritingAnalysisModel _floorPlan = new();
        private IEnumerable<UnderwritingAnalysisModel> FloorPlans = Array.Empty<UnderwritingAnalysisModel>();

        protected override void OnParametersSet()
        {
            _showModel = false;
            _isNew = false;
            GetFloorsAsync();
            if (FloorPlans == null)
                return;

            _numberOfUnits = FloorPlans.Sum(x => x.TotalUnits);
            _totalUnits = Property.Units;
        }

        private void GetFloorsAsync()
        {
            FloorPlans = Array.Empty<UnderwritingAnalysisModel>();
            FloorPlans = Property.Models;
        }

        private void OnAddFloor()
        {
            // TODO : Add new Floor support
            _floorPlan = new UnderwritingAnalysisModel();
            _isNew = true;
            _showModel = true;
        }

        private void OnEditFloor(GridCommandEventArgs args)
        {
            // TODO : Update Edit existing floor
            _floorPlan = args.Item as UnderwritingAnalysisModel;
            _isNew = false;
            _showModel = true;
        }
    }
}
