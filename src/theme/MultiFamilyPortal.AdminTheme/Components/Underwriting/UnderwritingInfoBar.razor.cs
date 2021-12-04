using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingInfoBar
    {
        [Parameter] 
        public string Title { get; set; }

         [Parameter] 
        public string Value { get; set; }

         [Parameter] 
        public string Colour { get; set; }
    }
}