using Paletteau.Plugin;

namespace Paletteau.Infrastructure.UserSettings
{
    public class CustomPluginHotkey : BaseModel
    {
        public string Hotkey { get; set; }
        public string ActionKeyword { get; set; }
    }
}
