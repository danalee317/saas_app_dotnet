using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.RentRoll
{
    public partial class UnderwritingRollUnitsTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal _user { get; set; }

        private bool _editable;

        private IEnumerable<DisplayUnit> _allUnits;
        private ObservableRangeCollection<DisplayUnit> _filteredUnits = new ();
        private DisplayUnit _unit;
        private string _query;

        protected override void OnInitialized()
        {
            _editable = _user.IsAuthorizedInPolicy(PortalPolicy.Underwriter);
            UpdateRentRoll();
        }

        private void UpdateRentRoll()
        {
            if(!Property.Models.SelectMany(x => x.Units).Any())
            {
                _allUnits = Array.Empty<DisplayUnit>();
                _filteredUnits.Clear();
                return;
            }

            _allUnits = Property.Models.SelectMany(m => m.Units.Select(u => new DisplayUnit(u, m)));

            var filtered = _allUnits;
            if (!string.IsNullOrEmpty(_query))
                filtered = _allUnits.Where(x =>
                    x.FloorPlanName.Contains(_query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.UnitName.Contains(_query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.Renter.Contains(_query, StringComparison.CurrentCultureIgnoreCase));

            _filteredUnits.ReplaceRange(filtered);
        }


        private void UpdateUnit(GridCommandEventArgs args)
        {
            _unit = args.Item as DisplayUnit;
        }

        private async Task UpdateUnitList(UnderwritingAnalysisUnit unit)
        {
            //await OnPropertyChanged.InvokeAsync(Property);
            //_showAddUnit = false;
        }

        private async Task RemoveUnit(UnderwritingAnalysisUnit unit)
        {
            var model = Property.Models.FirstOrDefault(m => m.Units.Contains(unit));
            model?.Units.Remove(unit);

            //await OnPropertyChanged.InvokeAsync(Property);
            //_showAddUnit = false;
        }
    }
}
