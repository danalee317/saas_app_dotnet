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
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MultiFamilyPortal.Collections;
using System.Net.Http.Json;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingNotesTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal User { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private UnderwritingAnalysisNote NewNote;
        private UnderwritingAnalysisNote UpdateNote;
        private PortalNotification notification { get; set; }

        private readonly ObservableRangeCollection<UnderwritingAnalysisNote> Notes = new();

        protected override void OnParametersSet()
        {
            if (Property?.Notes?.Any() ?? false)
                Notes.ReplaceRange(Property.Notes);
        }

        private void AddNote()
        {
            NewNote = new UnderwritingAnalysisNote {
                Timestamp = DateTimeOffset.Now,
                UnderwriterEmail = User.FindFirstValue(ClaimTypes.Email)
            };
        }

        private void SaveNote()
        {
            Notes.Add(NewNote);
            Property.Notes.Add(NewNote);
            NewNote = null;
        }

        private void OnEditNote(GridCommandEventArgs args)
        {
            UpdateNote = args.Item as UnderwritingAnalysisNote;
        }
        private async Task SaveUpdateNote()
        {
            using var response = await _client.PostAsJsonAsync($"/api/admin/underwriting/update/note/{UpdateNote.Id}", UpdateNote);

            if (response.IsSuccessStatusCode)
                notification.ShowSuccess("Note successfully updated.");
            else
                notification.ShowWarning("Unable to update note.");
        }

        private async Task OnDeleteNote(GridCommandEventArgs args)
        {
            var note = args.Item as UnderwritingAnalysisNote;
            Property.Notes.Remove(note);
            Notes.Remove(note);

            using var response = await _client.DeleteAsync($"/api/admin/underwriting/delete/note/{note.Id}");

            if (response.IsSuccessStatusCode)
                notification.ShowSuccess("Note successfully deleted.");
            else
                notification.ShowWarning("Unable to delete note.");
        }
    }
}
