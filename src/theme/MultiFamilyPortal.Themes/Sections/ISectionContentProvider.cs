using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.Themes.Sections
{
    internal interface ISectionContentProvider
    {
        RenderFragment? Content { get; }
    }
}
