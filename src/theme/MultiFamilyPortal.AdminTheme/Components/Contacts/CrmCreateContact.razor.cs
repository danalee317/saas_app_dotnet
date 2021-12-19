using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmCreateContact
    {
        [Parameter]
        public CRMContact Contact { get; set; }

        [Parameter]
        public EventCallback<CRMContact> ContactChanged { get; set; }

        private async Task OnNavigateBack()
        {
            Contact = null;
            await ContactChanged.InvokeAsync(null);
        }
    }
}
