using System.Reflection;using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Themes.Internals;

namespace MultiFamilyPortal.Themes
{
    public partial class ThemeApp
    {
        private Assembly Assembly = Assembly.GetEntryAssembly() ?? throw new Exception("The Entry Assembly is null");

        [Inject]
        private IThemeFactory _themeFactory { get; set; } = default!;
        private IPortalTheme Theme => _themeFactory.GetCurrentTheme();
    }
}