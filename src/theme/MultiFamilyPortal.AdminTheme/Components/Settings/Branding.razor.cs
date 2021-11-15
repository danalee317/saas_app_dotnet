using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Telerik.Blazor;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Settings
{
    public partial class Branding
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ILogger<Branding> Logger { get; set; }

        private Logo _selected;
        private readonly List<string> AllowedFileTypes = new() { ".png", ".svg", ".ico" };
        public string LogoUrl(string name) => ToAbsoluteUrl($"branding/{name}");
        private bool showWindow = false;
        private IEnumerable<Logo> _logos = Array.Empty<Logo>();

        protected override void OnInitialized()
        {
            var logi = new[]
            {
            new Logo { DisplayName = "Browser Icon", Href = "/apple-touch-icon.png", Name = "favicon" },
            new Logo { DisplayName = "Default Logo", Href = "/theme/branding/logo", Name = "logo" },
            new Logo { DisplayName = "Dark Theme Logo", Href = "/theme/branding/logo-dark", Name = "logo-dark" },
            new Logo { DisplayName = "Default Logo - Horizontal", Href = "/theme/branding/logo-side", Name = "logo-side" },
            new Logo { DisplayName = "Dark Theme Logo - Horizontal", Href = "/theme/branding/logo-dark-side", Name = "logo-dark-side" },
            };

            _logos = logi;
        }

        private string ToAbsoluteUrl(string url) => $"{_client.BaseAddress}api/admin/settings/{url}";

        private void UpdateLogo(GridCommandEventArgs args)
        {
            _selected = args.Item as Logo;
            showWindow = true;
        }

        private async Task OnSuccessHandler(UploadSuccessEventArgs e)
        {
            if (e.Operation == UploadOperationType.Upload)
            {
                showWindow = false;
                var logi = _logos.ToList();
                _logos = Array.Empty<Logo>();
                await Task.Delay(100);
                _logos = logi;
                await InvokeAsync((StateHasChanged));
            }
            else Logger.LogWarning("Upload failure");
        }

        private record Logo
        {
            public string Href { get; init; }

            public string DisplayName { get; init; }

            public string Name { get; init; }
        }
    }
}
