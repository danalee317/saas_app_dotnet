using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingFilesTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private void CloseUpload()
        {
        }

        public string SaveUrl => ToAbsoluteUrl($"api/admin/underwriting/upload/save/{Property.Id}");
        public string RemoveUrl => ToAbsoluteUrl($"api/admin/underwriting/upload/remove/{Property.Id}");
        public string ToAbsoluteUrl(string url)
        {
            return $"{_navigationManager.BaseUri}{url}";
        }
    }
}
