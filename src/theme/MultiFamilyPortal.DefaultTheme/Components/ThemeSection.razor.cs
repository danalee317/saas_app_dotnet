using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.DefaultTheme.Components
{
    public partial class ThemeSection
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
