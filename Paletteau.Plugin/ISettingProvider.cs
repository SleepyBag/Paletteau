using System.Windows.Controls;

namespace Paletteau.Plugin
{
    public interface ISettingProvider
    {
        Control CreateSettingPanel();
    }
}
