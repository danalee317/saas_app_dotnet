using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace MultiFamilyPortal.CoreUI
{
    public partial class PortalErrorBoundary
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Inject]
        private ILogger<PortalErrorBoundary> _logger { get; set; }

        private ErrorBoundary errorBoundary;
        private void RecoverFromError()
        {
            errorBoundary?.Recover();
        }

        private void HandleError(Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception caught by Error Boundary");
        }
    }
}
