using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.RentRoll
{
    public partial class UnderwritingRollUnitsControls
    {
        [Parameter]
        public DisplayUnit Unit { get; set; }

        [Parameter]
        public bool Editing { get; set; }

        [Parameter]
        public EventCallback<DisplayUnit> OnUnitChanged { get; set; }

        [Parameter]
        public EventCallback<DisplayUnit> RemoveUnitRequested { get; set; }

        private bool _conformation = false;

        private async Task UpdateUnit()
        {
            if (string.IsNullOrEmpty(Unit.UnitName))
                return;

            await OnUnitChanged.InvokeAsync(Unit);
        }

        private async Task RemoveUnit()
        {
            await RemoveUnitRequested.InvokeAsync(Unit);
        }
    }
}
