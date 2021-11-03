using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace MultiFamilyPortal.InspiniaTheme.Components
{
    public partial class InspiniaNavBarItem
    {
        [Parameter]
        public string Href { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string IconClass { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private string _active;
        private bool _expanded;
        protected override void OnInitialized()
        {
            _navigationManager.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            var relativePath = "/" + Regex.Replace(e.Location, _navigationManager.BaseUri, string.Empty);
            _active = Href == relativePath ? "active" : string.Empty;
        }
    }
}
