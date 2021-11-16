using System.Net.Http.Json;
using Humanizer;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Models;
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

        private ObservableRangeCollection<UnderwritingAnalysisFile> _files = new();

        private readonly IEnumerable<string> fileTypes = Enum.GetValues<UnderwritingProspectFileType>()
            .Select(x => x.Humanize(LetterCasing.Title));
        private string selectedFileType = UnderwritingProspectFileType.OfferMemorandum.Humanize(LetterCasing.Title);
        private string description;
        private bool showUploadFile;

        protected override async Task OnInitializedAsync()
        {
            await Update();
        }

        private void CloseUpload()
        {
            showUploadFile = false;
            selectedFileType = UnderwritingProspectFileType.OfferMemorandum.Humanize(LetterCasing.Title);
            description = null;
        }

        private async Task OnFileUploaded()
        {
            CloseUpload();
            await Update();
        }

        private async Task Update()
        {
            var files = await _client.GetFromJsonAsync<IEnumerable<UnderwritingAnalysisFile>>($"/api/admin/underwriting/files/{Property.Id}");
            _files.ReplaceRange(files.OrderByDescending(x => x.Timestamp));
        }

        private UnderwritingProspectFileType GetFileType() =>
            Enum.TryParse<UnderwritingProspectFileType>(selectedFileType.Pascalize(), true, out var result)
            ? result : UnderwritingProspectFileType.OfferMemorandum;

        public string SaveUrl => ToAbsoluteUrl($"api/admin/underwriting/upload/save/{Property.Id}?fileType={GetFileType()}&description={description}");

        public string ToAbsoluteUrl(string url)
        {
            return $"{_navigationManager.BaseUri}{url}";
        }
    }
}
