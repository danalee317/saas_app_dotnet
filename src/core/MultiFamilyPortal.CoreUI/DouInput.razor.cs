using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.CoreUI
{
    public partial class DouInput
    {
        [Parameter]
        public double Value {get; set;}

        [Parameter]
        public CostType Type {get; set;}

        [Parameter]
        public bool Enabled {get; set;}
        private readonly CostType[] CostTypes = new[] { CostType.PerDoor, CostType.Total };
    }
}