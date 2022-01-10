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

        private string GetColor() => Color switch
        {
            ColorCode.Info => "var(--portal-underwriting-info)",
            ColorCode.Warning => "var(--portal-underwriting-warning)",
            ColorCode.Danger => "var(--portal-underwriting-danger)",
            ColorCode.Success => "var(--portal-underwriting-success)",
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
