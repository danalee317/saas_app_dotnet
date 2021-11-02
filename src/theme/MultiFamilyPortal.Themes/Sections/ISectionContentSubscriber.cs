using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.Themes.Sections
{
    internal interface ISectionContentSubscriber
    {
        void ContentChanged(RenderFragment? content);
    }
}
