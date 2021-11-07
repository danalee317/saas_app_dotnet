using System.Collections;
using System.Net.Http.Json;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Settings
{
    public partial class EmailTemplates
    {
        [Inject]
        private HttpClient _client { get; set; }

        private ObservableRangeCollection<EmailTemplate> _emailTemplates { get; set; } = new();

        private EmailTemplate _selected;
        private PortalNotification notification;
        private IEnumerable<TemplateVariableDefinition> _definitions;

        protected override async Task OnInitializedAsync()
        {
            _emailTemplates.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<EmailTemplate>>("/api/admin/settings/email-templates"));
        }

        private void OnEditItem(GridCommandEventArgs args)
        {
            _definitions = Array.Empty<TemplateVariableDefinition>();
            _selected = args.Item as EmailTemplate;
            var type = Type.GetType(_selected.Model);
            if (type is null)
                return;

            _definitions = type.GetRuntimeProperties()
                .Select(x => new TemplateVariableDefinition
                {
                    Name = x.Name,
                    PartialTemplate = x.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(x.PropertyType)
                });
        }

        private record TemplateVariableDefinition
        {
            public string Name { get; init; }
            public bool PartialTemplate { get; init; }
        }

        private async Task OnUpdateTemplate()
        {
            if (_selected is null)
                return;

            using var response = await _client.PostAsJsonAsync("/api/admin/settings/email-templates/update", _selected);

            if (response.IsSuccessStatusCode)
                notification.ShowSuccess("Email template was successfully updated");
            else
                notification.ShowError("Unable to update the email template");

            _selected = null;
        }
    }
}
