using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Palette.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms.VisualStyles;

namespace Palette
{
    internal class Setting
    {
        public Exception lastException { get; private set; }
        public string filename { get; private set; }
        public DateTime lastLoadTime { get; private set; }
        public CommandTable commandTable { get; private set;  }

        private Setting(string _filename)
        {
            filename = _filename;
            commandTable = new CommandTable();
        }

        public static Setting ReadSetting(string filename)
        {
            Setting setting = new Setting(filename);
            setting.ReloadSetting();
            return setting;
        }

        public void ReloadSetting()
        {
            commandTable = new CommandTable();
            try
            {
                using (StreamReader r = new StreamReader(filename))
                {
                    string s = r.ReadToEnd();
                    var globalSetting = JObject.Parse(s);
                    var providerSettings = globalSetting["providers"].ToObject<Dictionary<string, JObject>>();
                    // make provider objects according to type name
                    foreach (var entry in providerSettings)
                    {
                        string providerName = entry.Key;
                        JObject providerSetting = entry.Value;
                        Type providerType = Type.GetType("Palette.Providers." + providerName);
                        var provider = Activator.CreateInstance(providerType) as IPaletteProvider;
                        var providerContext = new ProviderContext
                        {
                            setting = providerSetting,
                            commandTable = commandTable
                        };
                        provider.Init(providerContext);
                    }
                }
                lastException = null;
            }
            catch (Exception ex)
            {
                lastException = ex;
            }
            lastLoadTime = DateTime.Now;
        }
    }
}
