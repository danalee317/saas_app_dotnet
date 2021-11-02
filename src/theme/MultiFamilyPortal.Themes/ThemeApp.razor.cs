using System.Reflection;using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Themes.Internals;

namespace MultiFamilyPortal.Themes
{
    public partial class ThemeApp
    {
        private Assembly Assembly = Assembly.GetEntryAssembly() ?? throw new Exception("The Entry Assembly is null");

        [CascadingParameter]
        public ClaimsPrincipal User { get; set; }

        [Inject]
        private IEnumerable<IApplicationPart> _applicationParts { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        [Inject]
        private IThemeFactory _themeFactory { get; set; } = default!;
        private IPortalTheme Theme => _themeFactory.GetCurrentTheme();

        private bool IsAdmin()
        {
            var uri = new Uri(_navigationManager.Uri);
            return uri.AbsolutePath.StartsWith("/admin");
        }

        private IEnumerable<Assembly> AdditionalAssemblies()
        {
            if (Theme?.Layout is null)
                return Array.Empty<Assembly>();

            var list = new List<Assembly>
            {
                Theme.Layout.Assembly,
            };

            if(Theme is IApplicationPart part)
            {
                list.AddRange(part.Assemblies);
            }

            if(_applicationParts?.Any() ?? false)
            {
                var additional = _applicationParts.SelectMany(x => x.Assemblies);
                if(additional is not null && additional.Any())
                {
                    list.AddRange(additional);
                }
            }

            return list.Distinct();
        }
    }
}