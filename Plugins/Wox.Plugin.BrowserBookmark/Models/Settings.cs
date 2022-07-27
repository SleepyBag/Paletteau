using Paletteau.Infrastructure;

namespace Paletteau.Plugin.BrowserBookmark.Models
{
    public class Settings : BaseModel
    {
        public bool OpenInNewBrowserWindow { get; set; } = true;

        public string BrowserPath { get; set; }
    }
}