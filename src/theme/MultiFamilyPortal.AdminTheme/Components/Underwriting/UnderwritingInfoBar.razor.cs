using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingInfoBar
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public ColorCode Color { get; set; }

        private static string GetColor(ColorCode status) => status switch
        {
            ColorCode.Info => "var(--bs-info)",
            ColorCode.Warning => "salmon",
            ColorCode.Default => "#ffd800",
            ColorCode.Success => "#0e952f",
            ColorCode.Light => "var(--bs-light)",
            ColorCode.Dark => "var(--bs-dark)",
        };
    }
}