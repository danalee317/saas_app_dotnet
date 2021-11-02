using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.Themes.Internals
{
    public partial class CascadingWrapper
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; } = default!;

        [Inject]
        private ISiteInfo SiteInfo { get; set; } = default!;

        [Inject]
        private IThemeFactory ThemeFactory { get; set; } = default!;
        private IPortalTheme _theme;
        private IPortalTheme Theme => _theme ??= ThemeFactory.GetCurrentTheme();

        //protected override void OnInitialized()
        //{
        //    _theme = ThemeFactory.GetCurrentTheme();
        //}
    }
}
