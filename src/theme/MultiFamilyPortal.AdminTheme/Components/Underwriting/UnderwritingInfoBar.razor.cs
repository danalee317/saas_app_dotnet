using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;

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
            ColorCode.Warning => "#ffd800",
            ColorCode.Danger => "salmon",
            ColorCode.Success => "#0e952f",
            _ => "var(--bs-light)"
        };

        private string GetTextColor() => Color switch
        {
            ColorCode.Danger => "var(--bs-light)",
            ColorCode.Warning => "var(--bs-dark)",
            ColorCode.Success => "var(--bs-light)",
            ColorCode.Info => "var(--bs-dark)",
            _=> "var(--bs-light)",
        };
    }
}
