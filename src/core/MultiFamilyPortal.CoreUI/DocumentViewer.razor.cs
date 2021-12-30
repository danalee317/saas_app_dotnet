using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.CoreUI
{
    public partial class DocumentViewer : IAsyncDisposable
    {
        [Inject]
        private IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public UnderwritingAnalysisFile FileDocument { get; set; }

        ElementReference WrapperRef;
        private readonly string _widgetId = Guid.NewGuid().ToString();

        DotNetObjectReference<DocumentViewer> CurrentRazorComponent { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && Path.GetExtension(FileDocument.Name).ToLower() == ".pdf")
            {
                if (CurrentRazorComponent == null)
                {
                    CurrentRazorComponent = DotNetObjectReference.Create(this);
                }

                await JsRuntime.InvokeVoidAsync("createWidget", "createPdfViewer", WrapperRef, _widgetId, CurrentRazorComponent);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await JsRuntime.InvokeVoidAsync("destroyWidgets", WrapperRef);
            if (CurrentRazorComponent != null)
            {
                CurrentRazorComponent.Dispose();
            }
        }
    }
}