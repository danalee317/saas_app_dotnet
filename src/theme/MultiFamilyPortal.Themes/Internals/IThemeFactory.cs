using System.Reflection;

namespace MultiFamilyPortal.Themes.Internals{    public interface IThemeFactory    {        IPortalTheme GetCurrentTheme();
        IEnumerable<Assembly> GetAdditionalAssemblies(IPortalTheme theme);    }}