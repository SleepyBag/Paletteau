using System;
using System.Collections.ObjectModel;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using Wox.Infrastructure.Logger;
using Wox.Infrastructure.Storage;
using Wox.Plugin;

namespace Wox.Infrastructure.UserSettings
{
    public class Settings : BaseModel
    {
        #region Save/Load

        public static void Save()
        {
            _storage.Save();
        }

        private static WoxJsonStorage<Settings> _storage = new WoxJsonStorage<Settings>();
        public static Settings Instance;

        public static void Initialize()
        {
            Instance = _storage.Load();
        }

        #endregion

        public string Hotkey { get; set; } = "Alt + Space";
        public string Language { get; set; } = "en";
        public string Theme { get; set; } = "Dark";
        public string QueryBoxFont { get; set; } = FontFamily.GenericSansSerif.Name;
        public string QueryBoxFontStyle { get; set; }
        public string QueryBoxFontWeight { get; set; }
        public string QueryBoxFontStretch { get; set; }
        public string ResultFont { get; set; } = FontFamily.GenericSansSerif.Name;
        public string ResultFontStyle { get; set; }
        public string ResultFontWeight { get; set; }
        public string ResultFontStretch { get; set; }
        public string ResultHighlightFontStyle { get; set; }
        public string ResultHighlightFontWeight { get; set; }
        public string ResultHighlightFontStretch { get; set; }


        /// <summary>
        /// when false Alphabet static service will always return empty results
        /// </summary>
        public bool ShouldUsePinyin { get; set; } = false;

        internal StringMatcher.SearchPrecisionScore QuerySearchPrecision { get; private set; } = StringMatcher.SearchPrecisionScore.Regular;
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [JsonIgnore]
        public string QuerySearchPrecisionString
        {
            get { return QuerySearchPrecision.ToString(); }
            set
            {
                try
                {
                    var precisionScore = (StringMatcher.SearchPrecisionScore)Enum
                                            .Parse(typeof(StringMatcher.SearchPrecisionScore), value);

                    QuerySearchPrecision = precisionScore;
                    StringMatcher.Instance.UserSettingSearchPrecision = precisionScore;
                }
                catch (ArgumentException e)
                {
                    Logger.WoxError("Failed to load QuerySearchPrecisionString value from Settings file", e);

                    QuerySearchPrecision = StringMatcher.SearchPrecisionScore.Regular;
                    StringMatcher.Instance.UserSettingSearchPrecision = StringMatcher.SearchPrecisionScore.Regular;

                    throw;
                }
            }
        }

        public bool AutoUpdates { get; set; } = true;
        public bool UpdateToPrereleases { get; set; } = false;

        public double WindowLeft { get; set; }
        public double WindowTop { get; set; }
        public int MaxResultsToShow { get; set; } = 6;
        public int ActivateTimes { get; set; }

        // Order defaults to 0 or -1, so 1 will let this property appear last
        [JsonProperty(Order = 1)]
        public PluginsSettings PluginSettings { get; set; } = new PluginsSettings();
        public ObservableCollection<CustomPluginHotkey> CustomPluginHotkeys { get; set; } = new ObservableCollection<CustomPluginHotkey>();

        [Obsolete]
        public double Opacity { get; set; } = 1;

        [Obsolete]
        public OpacityMode OpacityMode { get; set; } = OpacityMode.Normal;

        public bool DontPromptUpdateMsg { get; set; }
        public bool EnableUpdateLog { get; set; }

        public bool StartWoxOnSystemStartup { get; set; } = true;
        public bool HideOnStartup { get; set; }
        bool _hideNotifyIcon { get; set; }
        public bool HideNotifyIcon
        {
            get { return _hideNotifyIcon; }
            set
            {
                _hideNotifyIcon = value;
                OnPropertyChanged();
            }
        }
        public bool LeaveCmdOpen { get; set; }
        public bool HideWhenDeactive { get; set; } = true;
        public bool RememberLastLaunchLocation { get; set; }
        public bool IgnoreHotkeysOnFullscreen { get; set; }

        public HttpProxy Proxy { get; set; } = new HttpProxy();
    }

    [Obsolete]
    public enum OpacityMode
    {
        Normal = 0,
        LayeredWindow = 1,
        DWM = 2
    }
}