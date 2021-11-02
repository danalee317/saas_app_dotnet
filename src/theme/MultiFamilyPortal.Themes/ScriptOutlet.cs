using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MultiFamilyPortal.Themes
{
    public sealed class ScriptOutlet : ComponentBase
    {
        /// <inheritdoc/>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            // Render the title content
            builder.OpenComponent<SectionOutlet>(0);
            builder.AddAttribute(1, nameof(SectionOutlet.Name), TitleSectionOutletName);
            builder.CloseComponent();

            // Render the default title if it exists
            if (!string.IsNullOrEmpty(_defaultTitle))
            {
                builder.OpenComponent<SectionContent>(2);
                builder.AddAttribute(3, nameof(SectionContent.Name), TitleSectionOutletName);
                builder.AddAttribute(4, nameof(SectionContent.IsDefaultContent), true);
                builder.AddAttribute(5, nameof(SectionContent.ChildContent), (RenderFragment)BuildDefaultTitleRenderTree);
                builder.CloseComponent();
            }

            // Render the rest of the head metadata
            builder.OpenComponent<SectionOutlet>(6);
            builder.AddAttribute(7, nameof(SectionOutlet.Name), HeadSectionOutletName);
            builder.CloseComponent();
        }

        private void BuildDefaultTitleRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "title");
            builder.AddContent(1, _defaultTitle);
            builder.CloseElement();
        }
    }
}
