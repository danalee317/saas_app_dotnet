using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MultiFamilyPortal.CoreUI
{
    public partial class DocumentViewer : IAsyncDisposable
    {
        [Inject]
        private IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public DocumentType Type { get; set; }

        [Parameter]
        public string Link { get; set; }

        private ElementReference _wrapperRef;
        private readonly string _widgetId = Guid.NewGuid().ToString();
        private DotNetObjectReference<DocumentViewer> _currentRazorComponent;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && Type == DocumentType.PDF)
            {
                if (_currentRazorComponent == null)
                {
                    _currentRazorComponent = DotNetObjectReference.Create(this);
                }

                await JsRuntime.InvokeVoidAsync("MFPortal.KendoIntialiase", _wrapperRef, _widgetId, _currentRazorComponent);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await JsRuntime.InvokeVoidAsync("MFPortal.DisposeKendo", _wrapperRef);
            if (_currentRazorComponent != null)
            {
                _currentRazorComponent.Dispose();
            }
        }
    }
}