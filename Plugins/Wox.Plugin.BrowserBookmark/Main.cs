using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Paletteau.Infrastructure.Storage;
using Paletteau.Plugin.BrowserBookmark.Commands;
using Paletteau.Plugin.BrowserBookmark.Models;
using Paletteau.Plugin.BrowserBookmark.Views;
using Paletteau.Infrastructure;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Paletteau.Plugin.BrowserBookmark
{
    public class Main : ISettingProvider, IPlugin, IReloadable, IPluginI18n, ISavable
    {
        private PluginInitContext context;

        private Dictionary<string, List<Bookmark>> cachedBookmarks = new Dictionary<string, List<Bookmark>>();
        private object _updateLock = new object();

        private readonly Settings _settings;
        private readonly PluginJsonStorage<Settings> _storage;


        public Main()
        {
            _storage = new PluginJsonStorage<Settings>();
            _settings = _storage.Load();

            var chromeBookmarks = new ChromeBookmarks().GetBookmarks().Distinct().ToList();
            var edgeBookmarks = new EdgeBookmarks().GetBookmarks().Distinct().ToList();
            lock (_updateLock)
            {
                cachedBookmarks["chrome"] = chromeBookmarks;
                cachedBookmarks["chromium"] = chromeBookmarks;
                cachedBookmarks["msedge"] = edgeBookmarks;
            }
            Task.Run(() =>
            {
                // firefox bookmarks is slow, since it nened open sqlite connection.
                // use lazy load
                var mozBookmarks = new FirefoxBookmarks().GetBookmarks().Distinct().ToList();
                lock (_updateLock)
                {
                    cachedBookmarks["firefox"] = mozBookmarks;
                }
            });

        }

        public void Init(PluginInitContext context)
        {
            this.context = context;
        }

        public static string getBaseUrl(string url)
        {
            if (url.Contains("://"))
            {
                url = url.Substring(url.IndexOf("://") + 3);
            }
            if (url.Contains('/'))
            {
                url = url.Substring(0, url.IndexOf('/'));
            }
            return url;
        }

        public List<Result> Query(Query query)
        {
            Process backgroundProcess = query.BackgroundProcess;
            if (!cachedBookmarks.TryGetValue(backgroundProcess.ProcessName, out var currentBookmarks))
            {
                return new List<Result>();
            }
            string param = query.Search.TrimStart();

            // Should top results be returned? (true if no search parameters have been passed)
            var topResults = string.IsNullOrEmpty(param);

            lock (_updateLock)
            {

                var returnList = currentBookmarks;

                if (!topResults)
                {
                    // Since we mixed chrome and firefox bookmarks, we should order them again                
                    returnList = currentBookmarks.Where(o => Bookmarks.MatchProgram(o, param)).ToList();
                    returnList = returnList.OrderByDescending(o => o.Score).ToList();
                }

                var results = returnList.Select(c => new Result()
                {
                    Title = c.Name,
                    SubTitle = getBaseUrl(c.Url),
                    PluginDirectory = context.CurrentPluginMetadata.PluginDirectory,
                    IcoPath = @"Images\bookmark.png",
                    Score = c.Score,
                    Action = (e) =>
                    {
                        if (_settings.OpenInNewBrowserWindow)
                        {
                            c.Url.NewBrowserWindow(backgroundProcess.MainModule.FileName, backgroundProcess.ProcessName);
                        }
                        else
                        {
                            c.Url.NewTabInBrowser(backgroundProcess.MainModule.FileName, backgroundProcess.ProcessName);
                        }

                        return true;
                    }
                }).ToList();
                return results;
            }
        }

        public void ReloadData()
        {
            //TODO: Let the user select which browser's bookmarks are displayed
            var chromeBookmarks = new ChromeBookmarks().GetBookmarks().Distinct().ToList();
            var mozBookmarks = new FirefoxBookmarks().GetBookmarks().Distinct().ToList();
            var edgeBookmarks = new EdgeBookmarks().GetBookmarks().Distinct().ToList();
            lock (_updateLock)
            {
                cachedBookmarks["chrome"] = chromeBookmarks;
                cachedBookmarks["chromium"] = chromeBookmarks;
                cachedBookmarks["firefox"] = mozBookmarks;
                cachedBookmarks["msedge"] = edgeBookmarks;
            }
        }

        public string GetTranslatedPluginTitle()
        {
            return context.API.GetTranslation("wox_plugin_browserbookmark_plugin_name");
        }

        public string GetTranslatedPluginDescription()
        {
            return context.API.GetTranslation("wox_plugin_browserbookmark_plugin_description");
        }

        public Control CreateSettingPanel()
        {
            return new SettingsControl(_settings);
        }

        public void Save()
        {
            _storage.Save();
        }
    }
}
