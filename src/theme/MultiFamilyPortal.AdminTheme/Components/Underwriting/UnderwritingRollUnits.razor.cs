using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingRollUnits
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Parameter]
        public EventCallback<UnderwritingAnalysis> OnPropertyChanged { get; set; }

        private IEnumerable<UnderwritingAnalysisUnit> _rollUnits = Array.Empty<UnderwritingAnalysisUnit>();
        private bool _showAddUnit = false;
        private UnderwritingAnalysisUnit _unit = new();

        protected override void OnParametersSet()
        {
            if (Property?.Models != null)
                _rollUnits = Property.Models.Where(x => x.Units != null).SelectMany(x => x.Units);
        }

        private void UpdateUnit(GridCommandEventArgs args)
        {
            _unit = args.Item as UnderwritingAnalysisUnit;
            _showAddUnit = true;
        }

        private async Task UpdateUnitList(UnderwritingAnalysisUnit unit)
        {
            await OnPropertyChanged.InvokeAsync(Property);
            _showAddUnit = false;
        }

        private async Task RemoveUnit(UnderwritingAnalysisUnit unit)
        {
            var model = Property.Models.FirstOrDefault(m => m.Units.Contains(unit));
            model?.Units.Remove(unit);

            await OnPropertyChanged.InvokeAsync(Property);
            _showAddUnit = false;
        }
    }
}
