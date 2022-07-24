using System;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Paletteau.Plugin;
using Paletteau.Infrastructure;
using Paletteau.Infrastructure.UserSettings;

namespace Palette
{
    class Main : IPlugin, ISettingProvider
    {
        DateTime lastQueryTime;
        Setting setting;
        History history;
        ProcessIdentifier curProcessIdentifier;                     // for increamntal filtering
        Dictionary<String, List<Int32>> matchingCache;              // for increamntal filtering
        List<CommandItem> commandListCache;                         // for incremental filtering

        public List<Result> Query(Query query)
        {
            var process = query.BackgroundProcess;
            // if no palettes for current process
            if (process == null)
            {
                return new List<Result>();
            }
            if (!setting.commandTable.Contains(process))
            {
                return new List<Result>();
            }

            var processIdentifier = new ProcessIdentifier(process);
            var commandList = setting.commandTable[process];
            var commandListUpdated = commandList.IsUpdated();
            // cached information of current process
            if (processIdentifier != curProcessIdentifier || commandListUpdated)
            {
                curProcessIdentifier = processIdentifier;
                commandListCache = commandList.ToList();
                matchingCache = new Dictionary<string, List<int>>();
            }

            lastQueryTime = DateTime.Now;
            
            var matchingRange = GetMatchingRange(query.Search);
            var matchedIndices = new List<Int32>();
            var results = new List<Result>();
            // only search for results in current matching range
            foreach (int index in matchingRange) {
                var commandItem = commandListCache[index];
                var matchResult = StringMatcher.FuzzySearch(query.Search, commandItem.Title);
                if (query.Search.Length == 0 || matchResult.Success)
                {
                    // put matched indices into cache
                    matchedIndices.Add(index);
                    // put item into result list
                    results.Add(new Result
                    {
                        Title = commandItem.Title,
                        SubTitle = commandItem.SubTitle,
                        // IcoPath = Path.Combine("Images", "app.png"),
                        Score = matchResult.Score + history.GetCount(process, commandItem) * 1000000,      // history always first
                        TitleHighlightData = matchResult.MatchData,
                        Action = actionContext =>
                        {
                            bool result = commandItem.Action(actionContext, process);
                            history.Hit(process, commandItem);
                            return result;
                        }
                    });
                }
            }
            // update cache
            matchingCache[query.Search] = matchedIndices;
            return results;
        }

        public void Init(PluginInitContext context)
        {
            setting = Setting.ReadSetting(
                Path.Combine(context.CurrentPluginMetadata.PluginDirectory, "settings.json")
            );
            history = History.ReadHistory(
                Path.Combine(context.CurrentPluginMetadata.PluginDirectory, "history.json")
            );
        }

        public System.Windows.Controls.Control CreateSettingPanel()
        {
            return new SettingWindow(setting);
        }

        // check if needle is a subsequence of hay
        private static bool IsSubsequence(string hay, string needle)
        {
            // empty string is always the subsequence of other string
            if (needle.Length == 0)
            {
                return true;
            }
            // check if subsequence
            for(int i = 0, j = 0; i < hay.Length; ++i)
            {
                if (hay[i] == needle[j])
                {
                    ++j;
                }
                if (j == needle.Length)
                    return true;
            }
            return false;
        }

        // get the list of matching items' indices
        private List<Int32> GetMatchingRange(string query)
        {
            if (matchingCache.ContainsKey(query))
                return matchingCache[query];
            // sort keys from longer to shorter
            var cachedKeys = matchingCache.Keys.OrderBy(key => -key.Length).ToList();
            // check if there is a subsequence matched before
            foreach(string cachedKey in matchingCache.Keys)
            {
                if (IsSubsequence(query, cachedKey))
                {
                    return matchingCache[cachedKey];
                }
            }
            // if no subsequene matched before, all the indices could be in the range
            return Enumerable.Range(0, commandListCache.Count()).ToList();
        }

        private bool IsSettingUpdated()
        {
            return setting.lastLoadTime > lastQueryTime;
        }
    }
}
