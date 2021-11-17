using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.Themes.Internals
{
    public partial class CascadingWrapper
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; } = default!;

        [Parameter]
        public IPortalTheme Theme { get; set; } = default!;

        [Inject]
        private ISiteInfo SiteInfo { get; set; } = default!;
    }
}
