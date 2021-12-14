using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.CoreUI;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingFloorPlansControls
    {
        [Inject]
        private ILogger<UnderwritingFloorPlansControls> _logger { get; set; }

        [Parameter]
        public bool ShowWindow { get; set; }

        // Accept an object paramter

        private PortalNotification _notification;

        private async Task AddFloorAsync()
        {
            // Todo : send data to add endpoint and check response
        }

        private async Task EditFloorAsync()
        {
            // Todo : send data to edit endpoint and check response
        }

        private async Task DeleteFloorAsync()
        {
            // Todo : send data to delete endpoint and check response
        }

        private void ClearCurrent()
        {
            // Could be called by addFloor or button
        }

        private void VerifyInfo()
        {
             // Use this to verify user data or use in-built EditForm
        }
    }

}