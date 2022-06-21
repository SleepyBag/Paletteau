using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Paletteau.Plugin;
using Paletteau.Infrastructure;

namespace Palette
{
    class Main : IPlugin
    {
        #region user32.dll import
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError=true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError=true)]
        static extern Process GetProcessById(Int32 pID);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion

        Setting setting;
        History history;
        Process curProcess;                                         // for increamntal filtering
        Dictionary<String, List<Int32>> matchingCache;              // for increamntal filtering
        List<ActionItem> commandTable;                              // for incremental filtering

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
            return Enumerable.Range(0, commandTable.Count()).ToList();
        }

        private Process GetActiveProcess()
        {
            IntPtr handle = GetForegroundWindow();
            uint pID;
           
            GetWindowThreadProcessId(handle, out pID);

            return Process.GetProcessById((Int32)pID);
        }

        static Dictionary<String, String> keyNameMapping = new Dictionary<string, string>
        {
            { "~", "↩" },
            { " ", "␣" },
            { "{ENTER}", "↩" },
            { "{TAB}", "⭾" },
            { "{DEL}", "␡" },
            { "{DELETE}", "␡" },
            { "{LEFT}", "←" },
            { "{RIGHT}", "→" },
            { "{UP}", "↑" },
            { "{DOWN}", "↓" },
            { "{BS}", "⇐" },
            { "{BKSP}", "⇐" },
            { "{BACKSPACE}", "⇐" },
            { "{ADD}", "numpad+" },
            { "{SUBTRACT}", "numpad-" },
            { "{MULTIPLY}", "numpad*" },
            { "{DIVIDE}", "numpad/" },
        };

        private static string TranslateKey(string key)
        {
            string translatedKey = "";
            bool modifier = true;
            while (modifier)
            {
                modifier = false;
                if (key.StartsWith("^"))
                {
                    translatedKey += "Ctrl-";
                    modifier = true;
                } 
                else if (key.StartsWith("+"))
                {
                    translatedKey += "Shift-";
                    modifier = true;
                }
                else if (key.StartsWith("%"))
                {
                    translatedKey += "Alt-";
                    modifier = true;
                }
                if (modifier)
                    key = key.Substring(1);
            }
            if (keyNameMapping.ContainsKey(key))
                translatedKey += keyNameMapping[key];
            else if (!key.StartsWith("{"))
                translatedKey += key.ToUpper();        // normal character
            else if (key.EndsWith("}"))                // just in case. Don't crash on exceptional cases
                translatedKey = key.Substring(1, key.Length - 2);
            return translatedKey;
        }

        public List<Result> Query(Query query)
        {
            var process = query.BackgroundProcess;
            if (process == null || !setting.commandTables.ContainsKey(process.ProcessName))
            {
                return new List<Result>();
            }
            // cached information of current process
            if (process.ProcessName != curProcess?.ProcessName)
            {
                curProcess = process;
                commandTable = setting.commandTables[process.ProcessName];
                matchingCache = new Dictionary<string, List<int>>();
            }
            
            var matchingRange = GetMatchingRange(query.Search);
            var matchedIndices = new List<Int32>();
            var results = new List<Result>();
            // only search for results in current matching range
            foreach (int index in matchingRange) {
                var actionItem = commandTable[index];
                var matchResult = StringMatcher.FuzzySearch(query.Search, actionItem.description);
                if (query.Search.Length == 0 || matchResult.Success)
                {
                    // put matched indices into cache
                    matchedIndices.Add(index);
                    // put item into result list
                    results.Add(new Result
                    {
                        Title = actionItem.description,
                        SubTitle = TranslateKey(actionItem.action),
                        // IcoPath = Path.Combine("Images", "app.png"),
                        Score = matchResult.Score + history.GetCount(process, actionItem) * 1000000,      // history always first
                        TitleHighlightData = matchResult.MatchData,
                        Action = _ =>
                        {
                            Task.Run(() =>
                            {
                                history.Hit(process, actionItem);
                                // waiting for 3 seconds
                                for (int i = 0; i < 30; i++)
                                {
                                    // if current process matched the target, send keys
                                    if (GetActiveProcess().Id == process.Id)
                                    {
                                        SendKeys.SendWait(actionItem.action);
                                        SendKeys.Flush();
                                        return;
                                    }
                                    Task.Delay(100);
                                };
                            });
                            return true;
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
    }
}
