using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.AdminTheme.Components;
using MultiFamilyPortal.AdminTheme.Models;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Editor;
using MultiFamilyPortal.Collections;
using System.Collections;
using System.Net.Http.Json;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingList
    {
        [Parameter]
        public UnderwriterResponse Profile { get; set; }

        [Parameter]
        public EventCallback<UnderwriterResponse> ProfileChanged { get; set; }

        [Parameter]
        public DateTimeOffset Start { get; set; }

        [Parameter]
        public EventCallback<DateTimeOffset> StartChanged { get; set; }

        [Parameter]
        public DateTimeOffset End { get; set; }

        [Parameter]
        public EventCallback<DateTimeOffset> EndChanged { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        public NavigationManager _navigationManager { get; set; }

        private CreateUnderwritingPropertyRequest NewProspect;
        private ObservableRangeCollection<ProspectPropertyResponse> Prospects = new ObservableRangeCollection<ProspectPropertyResponse>();
        

        protected override async Task OnInitializedAsync()
        {
            await Update();
        }

        public async Task Update()
        {
            var underwriterId = Profile.Id;
            var properties = await _client.GetFromJsonAsync<IEnumerable<ProspectPropertyResponse>>($"/api/admin/underwriting?start={Start}&end={End}&underwriterId={underwriterId}");

            Prospects.ReplaceRange(properties);
        }

        private void CreateProperty()
        {
            NewProspect = new CreateUnderwritingPropertyRequest();
        }

        private async Task StartUnderwriting()
        {
            using var response = await _client.PostAsJsonAsync("/api/admin/underwriting/create", NewProspect);
            var property = await response.Content.ReadFromJsonAsync<ProspectPropertyResponse>();
            Prospects.Add(property);
            NavigateToProperty(property);
        }

        private void ViewProperty(GridCommandEventArgs args)
        {
            var property = args.Item as ProspectPropertyResponse;
            NavigateToProperty(property);
        }

        private void NavigateToProperty(ProspectPropertyResponse property)
        {
            _navigationManager.NavigateTo($"/admin/underwriting/property/{property.Id}");
        }
    }
}
