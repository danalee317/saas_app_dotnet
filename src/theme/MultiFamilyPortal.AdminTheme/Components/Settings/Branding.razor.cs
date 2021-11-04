using Microsoft.AspNetCore.Components;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Settings
{
    public partial class Branding
    {
        [Inject]
        private HttpClient _client { get; set; }

        private Logo _selected;

        private readonly IEnumerable<Logo> _logos = new[]
        {
            new Logo { DisplayName = "Default Logo", Href = "/theme/branding/logo", Name = "logo" },
            new Logo { DisplayName = "Dark Theme Logo", Href = "/theme/branding/logo-dark", Name = "logo-dark" },
            new Logo { DisplayName = "Default Logo - Horizontal", Href = "/theme/branding/logo-side", Name = "logo-side" },
            new Logo { DisplayName = "Dark Theme Logo - Horizontal", Href = "/theme/branding/logo-dark-side", Name = "logo-dark-side" },
        };

        private void UpdateLogo(GridCommandEventArgs args)
        {
            _selected = args.Item as Logo;
        }

        private record Logo
        {
            public string Href { get; init; }

            public string DisplayName { get; init; }

            public string Name { get; init; }
        }
    }
}
