using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.InspiniaTheme
{
    public abstract class InspiniaTheme : IPortalTheme, IApplicationPart, IScriptProvider, IPortalMenuProvider
    {
        public InspiniaTheme()
        {
            Assemblies = new[] { GetType().Assembly };
        }

        public Type Layout { get; } = typeof(Layouts.MainLayout);
        public abstract string Name { get; }
        public Type _404 { get; } = typeof(Components._404);
        public string[] RequiredStyles { get; } = new[]
        {
            "https://avantipoint.blob.core.windows.net/theme/inspinia/2.9.4/css/animate.css",
            "https://avantipoint.blob.core.windows.net/theme/inspinia/2.9.4/css/style.css",
        };
        public string[] Scripts { get; } = new[]
        {
            "https://cdnjs.cloudflare.com/ajax/libs/popper.js/2.10.2/umd/popper.min.js",
            "https://avantipoint.blob.core.windows.net/theme/inspinia/2.9.4/js/plugins/metisMenu/jquery.metisMenu.js",
            //"https://cdn.avantipoint.com/themes/inspinia/2.9.4/js/plugins/metisMenu/jquery.metisMenu.js",
            "https://avantipoint.blob.core.windows.net/theme/inspinia/2.9.4/js/plugins/slimscroll/jquery.slimscroll.min.js",
            "https://avantipoint.blob.core.windows.net/theme/inspinia/2.9.4/js/inspinia.js",
            "https://avantipoint.blob.core.windows.net/theme/inspinia/2.9.4/js/plugins/pace/pace.min.js",
        };
        public IEnumerable<Assembly> Assemblies { get; protected set; }
        public abstract Type SideBar { get; }
    }
}
