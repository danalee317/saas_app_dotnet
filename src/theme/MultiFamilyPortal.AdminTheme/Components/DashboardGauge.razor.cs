using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.AdminTheme.Components
{
    public partial class DashboardGauge
    {
        [Parameter]
        public int Value { get; set; }

        [Parameter]
        public int Expected { get; set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string Color { get; set; }

        private double Max => Expected * 1.5;
        private string CalculatedColor
        {
            get
            {
                if (!string.IsNullOrEmpty(Color))
                    return Color;
                else if (Value <= Expected / 2)
                    return "red";
                else if (Value <= Expected * .9)
                    return "yellow";
                return "green";
            }
        }
    }
}
