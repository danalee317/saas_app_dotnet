using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MultiFamilyPortal.CoreUI
{
    public partial class PortalErrorBoundary
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private ErrorBoundary errorBoundary;
        private void RecoverFromError()
        {
            errorBoundary?.Recover();
        }
    }
}
