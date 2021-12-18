using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingFloorPlans
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Parameter]
        public EventCallback OnPropertyChanged  { get; set; }

        private bool _showModel = false;
        private bool _isNew = false;
        private int _totalUnits = 0;
        private int _numberOfUnits = 0;
        private UnderwritingAnalysisModel _floorPlan = new();
        private IEnumerable<UnderwritingAnalysisModel> _floorPlans = Array.Empty<UnderwritingAnalysisModel>();

        protected override void OnParametersSet()
        {
            _showModel = false;
            _isNew = false;
            GetFloorsAsync();
            if (_floorPlans == null)
                return;

            _numberOfUnits = _floorPlans.Sum(x => x.TotalUnits);
            _totalUnits = Property.Units;
        }

        private void GetFloorsAsync()
        {
            _floorPlans = Array.Empty<UnderwritingAnalysisModel>();
            _floorPlans = Property.Models;
        }

        private void OnAddFloor()
        {
            _floorPlan = new UnderwritingAnalysisModel();
            _isNew = true;
            _showModel = true;
        }

        private void OnEditFloor(GridCommandEventArgs args)
        {
            _floorPlan = args.Item as UnderwritingAnalysisModel;
            _isNew = false;
            _showModel = true;
        }

        private async Task InformSuperParent() => await OnPropertyChanged.InvokeAsync();
    }
}
