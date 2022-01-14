using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.CoreUI
{
    public partial class CapXInput
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Parameter]
        public bool Enabled { get; set; } = true;

        [Parameter]
        public string Id { get; set; }

        protected override void OnParametersSet()
        {
            _capx = Property.CapX.ToString("C2");
            CostType = Property.CapXType;
        }
        private string _capx;
        private readonly CostType[] _costTypes = { CostType.PerDoor, CostType.Total };

        private void FormatToDecimal()
        {
            Property.CapX = double.Parse(_capx);
            _capx = Property.CapX.ToString("C");
        }

        private void FormatToNumber() => _capx = double.Parse(_capx[1..]).ToString("N");

        private CostType CostType
        {
            get => Property.CapXType;
            set
            {
                ProcessCapXType(value, Property.CapXType);
                Property.CapXType = value;
            }
        }

        private void ProcessCapXType(CostType newValue, CostType oldvalue)
        {
            if(newValue != oldvalue)
            {
                if (newValue == CostType.PerDoor)
                    Property.CapX = Property.CapX / Property.Units;
                else
                    Property.CapX = Property.CapX * Property.Units;
                
                _capx = Property.CapX.ToString("C2");
            }
        }
    }
}
