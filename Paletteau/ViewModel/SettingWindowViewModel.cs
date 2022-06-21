using Microsoft.WindowsAPICodePack.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Paletteau.Core;
using Paletteau.Core.Configuration;
using Paletteau.Core.Plugin;
using Paletteau.Core.Resource;
using Paletteau.Helper;
using Paletteau.Infrastructure;
using Paletteau.Infrastructure.Storage;
using Paletteau.Infrastructure.UserSettings;
using Paletteau.Plugin;

namespace Paletteau.ViewModel
{
    public class SettingWindowViewModel : BaseModel
    {
        private readonly Updater _updater;
        private readonly IPortable _portable;

        public SettingWindowViewModel(IPortable portable)
        {

            _updater = new Updater(Paletteau.Properties.Settings.Default.GithubRepo); ;
            _portable = portable;
            Settings = Settings.Instance;
            Settings.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Settings.ActivateTimes))
                {
                    OnPropertyChanged(nameof(ActivatedTimes));
                }
            };
            AutoUpdates();
        }

        public Settings Settings { get; set; }

        public async void UpdateApp()
        {
            if (PortableMode)
            {
                MessageBox.Show("Portable mode need check update manually in https://github.com/SleepyBag/Paletteau/releases");
            }
            else
            {
                await _updater.UpdateApp(false, Settings.UpdateToPrereleases);
            }
        }

        // This is only required to set at startup. When portable mode enabled/disabled a restart is always required
        private bool _portableMode = DataLocation.PortableDataLocationInUse();
        public bool PortableMode
        {
            get { return _portableMode; }
            set
            {
                if (!_portable.CanUpdatePortability())
                    return;

                if (DataLocation.PortableDataLocationInUse())
                {
                    _portable.DisablePortableMode();
                }
                else
                {
                    _portable.EnablePortableMode();
                }
            }
        }

        private void AutoUpdates()
        {
            Task.Run(async () =>
            {
                if (Settings.Instance.AutoUpdates && !PortableMode)
                {
                    // check udpate every 5 hours
                    var timer = new System.Timers.Timer(1000 * 60 * 60 * 5);
                    timer.Elapsed += async (s, e) =>
                    {
                        await _updater.UpdateApp(true, Settings.Instance.UpdateToPrereleases);
                    };
                    timer.Start();

                    // check updates on startup
                    await _updater.UpdateApp(true, Settings.Instance.UpdateToPrereleases);
                }
            }).ContinueWith(ErrorReporting.UnhandledExceptionHandleTask, TaskContinuationOptions.OnlyOnFaulted);
        }



        public void Save()
        {
            Settings.Save();
        }

        #region general

        public string Language
        {
            get
            {
                return Settings.Language;
            }
            set
            {
                InternationalizationManager.Instance.ChangeLanguage(value);

                if (InternationalizationManager.Instance.PromptShouldUsePinyin(value))
                    ShouldUsePinyin = true;
            }
        }

        public bool ShouldUsePinyin
        {
            get
            {
                return Settings.ShouldUsePinyin;
            }
            set
            {
                Settings.ShouldUsePinyin = value;
            }
        }

        public List<string> QuerySearchPrecisionStrings
        {
            get
            {
                var precisionStrings = new List<string>();

                var enumList = Enum.GetValues(typeof(StringMatcher.SearchPrecisionScore)).Cast<StringMatcher.SearchPrecisionScore>().ToList();

                enumList.ForEach(x => precisionStrings.Add(x.ToString()));

                return precisionStrings;
            }
        }

        private Internationalization _translater => InternationalizationManager.Instance;
        public List<Language> Languages => _translater.LoadAvailableLanguages();
        public IEnumerable<int> MaxResultsRange => Enumerable.Range(2, 16);

        public string TestProxy()
        {
            var proxyServer = Settings.Proxy.Server;
            var proxyUserName = Settings.Proxy.UserName;
            if (string.IsNullOrEmpty(proxyServer))
            {
                return InternationalizationManager.Instance.GetTranslation("serverCantBeEmpty");
            }
            if (Settings.Proxy.Port <= 0)
            {
                return InternationalizationManager.Instance.GetTranslation("portCantBeEmpty");
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_updater.GitHubRepository);

            if (string.IsNullOrEmpty(proxyUserName) || string.IsNullOrEmpty(Settings.Proxy.Password))
            {
                request.Proxy = new WebProxy(proxyServer, Settings.Proxy.Port);
            }
            else
            {
                request.Proxy = new WebProxy(proxyServer, Settings.Proxy.Port)
                {
                    Credentials = new NetworkCredential(proxyUserName, Settings.Proxy.Password)
                };
            }
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return InternationalizationManager.Instance.GetTranslation("proxyIsCorrect");
                }
                else
                {
                    return InternationalizationManager.Instance.GetTranslation("proxyConnectFailed");
                }
            }
            catch
            {
                return InternationalizationManager.Instance.GetTranslation("proxyConnectFailed");
            }
        }

        #endregion

        #region plugin

        public static string Plugin => "http://www.wox.one/plugin";
        public PluginViewModel SelectedPlugin { get; set; }

        public IList<PluginViewModel> PluginViewModels
        {
            get
            {
                var metadatas = PluginManager.AllPlugins
                    .OrderBy(x => x.Metadata.Disabled)
                    .ThenBy(y => y.Metadata.Name)
                    .Select(p => new PluginViewModel { PluginPair = p })
                    .ToList();
                return metadatas;
            }
        }

        public Control SettingProvider
        {
            get
            {
                var settingProvider = SelectedPlugin.PluginPair.Plugin as ISettingProvider;
                if (settingProvider != null)
                {
                    var control = settingProvider.CreateSettingPanel();
                    control.HorizontalAlignment = HorizontalAlignment.Stretch;
                    control.VerticalAlignment = VerticalAlignment.Stretch;
                    return control;
                }
                else
                {
                    return new Control();
                }
            }
        }



        #endregion

        #region theme

        public static string Theme => @"http://www.wox.one/theme/builder";

        public string SelectedTheme
        {
            get { return Settings.Theme; }
            set
            {
                Settings.Theme = value;
                ThemeManager.Instance.ChangeTheme(value);
            }
        }

        public List<string> Themes
            => ThemeManager.Instance.LoadAvailableThemes().Select(Path.GetFileNameWithoutExtension).ToList();

        public Brush PreviewBackground
        {
            get
            {
                var wallpaper = WallpaperPathRetrieval.GetWallpaperPath();
                if (wallpaper != null && File.Exists(wallpaper))
                {
                    var memStream = new MemoryStream(File.ReadAllBytes(wallpaper));
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = memStream;
                    bitmap.EndInit();
                    var brush = new ImageBrush(bitmap) { Stretch = Stretch.UniformToFill };
                    return brush;
                }
                else
                {
                    var wallpaperColor = WallpaperPathRetrieval.GetWallpaperColor();
                    var brush = new SolidColorBrush(wallpaperColor);
                    return brush;
                }
            }
        }

        public ResultsViewModel PreviewResults
        {
            get
            {
                var results = new List<Result>
                {
                    new Result
                    {
                        Title = "Paletteau is a global command palette.",
                        SubTitle = "Ctrl-A",
                        IcoPath = "Images/app.png"
                    },
                    new Result
                    {
                        Title = "Use pinyin to search for programs. (yyy / wangyiyun → 网易云音乐)",
                        SubTitle = "Ctrl-Shift-B",
                        IcoPath = "Images/search.png"
                    },
                    //new Result
                    //{
                    //    Title = "Keyword plugin search.",
                    //    SubTitle = "search google with g search_term.",
                    //    IcoPath = "Images/browser.png"
                    //},
                    //new Result
                    //{
                    //    Title = "Build custom themes at: ",
                    //    SubTitle = Theme,
                    //    IcoPath = "Images/color.png"
                    //},
                    //new Result
                    //{
                    //    Title = "Install plugins from: ",
                    //    SubTitle = Plugin,
                    //    IcoPath = "Images/plugin.png"
                    //},
                    new Result
                    {
                        Title = $"Open Source: {_updater.GitHubRepository}",
                        SubTitle = "Please star it!",
                        IcoPath = "Images/ok.png"
                    }
                };
                var vm = new ResultsViewModel(Settings);
                vm.AddResults(results, "PREVIEW");
                return vm;
            }
        }

        public FontFamily SelectedQueryBoxFont
        {
            get
            {
                if (Fonts.SystemFontFamilies.Count(o =>
                    o.FamilyNames.Values != null &&
                    o.FamilyNames.Values.Contains(Settings.QueryBoxFont)) > 0)
                {
                    var font = new FontFamily(Settings.QueryBoxFont);
                    return font;
                }
                else
                {
                    var font = new FontFamily("Segoe UI");
                    return font;
                }
            }
            set
            {
                Settings.QueryBoxFont = value.ToString();
                ThemeManager.Instance.ChangeTheme(Settings.Theme);
            }
        }

        public FamilyTypeface SelectedQueryBoxFontFaces
        {
            get
            {
                var typeface = SyntaxSugars.CallOrRescueDefault(
                    () => SelectedQueryBoxFont.ConvertFromInvariantStringsOrNormal(
                        Settings.QueryBoxFontStyle,
                        Settings.QueryBoxFontWeight,
                        Settings.QueryBoxFontStretch
                        ));
                return typeface;
            }
            set
            {
                Settings.QueryBoxFontStretch = value.Stretch.ToString();
                Settings.QueryBoxFontWeight = value.Weight.ToString();
                Settings.QueryBoxFontStyle = value.Style.ToString();
                ThemeManager.Instance.ChangeTheme(Settings.Theme);
            }
        }

        public FontFamily SelectedResultFont
        {
            get
            {
                if (Fonts.SystemFontFamilies.Count(o =>
                    o.FamilyNames.Values != null &&
                    o.FamilyNames.Values.Contains(Settings.ResultFont)) > 0)
                {
                    var font = new FontFamily(Settings.ResultFont);
                    return font;
                }
                else
                {
                    var font = new FontFamily("Segoe UI");
                    return font;
                }
            }
            set
            {
                Settings.ResultFont = value.ToString();
                ThemeManager.Instance.ChangeTheme(Settings.Theme);
            }
        }

        public FamilyTypeface SelectedResultFontFaces
        {
            get
            {
                var typeface = SyntaxSugars.CallOrRescueDefault(
                    () => SelectedResultFont.ConvertFromInvariantStringsOrNormal(
                        Settings.ResultFontStyle,
                        Settings.ResultFontWeight,
                        Settings.ResultFontStretch
                        ));
                return typeface;
            }
            set
            {
                Settings.ResultFontStretch = value.Stretch.ToString();
                Settings.ResultFontWeight = value.Weight.ToString();
                Settings.ResultFontStyle = value.Style.ToString();
                ThemeManager.Instance.ChangeTheme(Settings.Theme);
            }
        }

        public FamilyTypeface SelectedResultHighlightFontFaces
        {
            get
            {
                var typeface = SyntaxSugars.CallOrRescueDefault(
                    () => SelectedResultFont.ConvertFromInvariantStringsOrNormal(
                        Settings.ResultHighlightFontStyle,
                        Settings.ResultHighlightFontWeight,
                        Settings.ResultHighlightFontStretch
                        ));
                return typeface;
            }
            set
            {
                Settings.ResultHighlightFontStretch = value.Stretch.ToString();
                Settings.ResultHighlightFontWeight = value.Weight.ToString();
                Settings.ResultHighlightFontStyle = value.Style.ToString();
                ThemeManager.Instance.ChangeTheme(Settings.Theme);
            }
        }

        #endregion

        #region hotkey

        public CustomPluginHotkey SelectedCustomPluginHotkey { get; set; }

        #endregion

        #region about

        public string Github => _updater.GitHubRepository;
        public string ReleaseNotes => _updater.GitHubRepository + @"/releases/latest";
        public static string Version => Constant.Version;
        public string ActivatedTimes => string.Format(_translater.GetTranslation("about_activate_times"), Settings.ActivateTimes);
        #endregion
    }
}
