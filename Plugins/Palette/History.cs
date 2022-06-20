using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palette
{
    using HistoryTable = Dictionary<string, HistoryEntry>;

    public class HistoryEntry
    {
        [JsonProperty("count")]
        public int count { get; set; }
        [JsonProperty("actionItem")]
        public ActionItem actionItem { get; set; }

        public HistoryEntry(ActionItem item)
        {
            count = 0;
            actionItem = item;
        }
    }

    public class History
    {
        [JsonProperty("history")]
        Dictionary<string, HistoryTable> history = new Dictionary<string, HistoryTable>();
        string filename;
        
        public void Hit(Process process, ActionItem actionItem)
        {
            HistoryEntry entry;
            string key = actionItem.action;
            if (!history.ContainsKey(process.ProcessName))
                history[process.ProcessName] = new HistoryTable();
            var historyTable = history[process.ProcessName];
            if (historyTable.ContainsKey(key) && historyTable[key].actionItem == actionItem)     // item content must match
                entry = historyTable[key];
            else
            {
                entry = new HistoryEntry(actionItem);
                historyTable[key] = entry;
            }
            entry.count += 1;

            // asynchronously write to history file
            Task.Run(() =>
            {
                using (StreamWriter w = new StreamWriter(File.OpenWrite(filename)))
                {
                    string json = JsonConvert.SerializeObject(this);
                    w.Write(json);
                    w.Flush();
                }
            });
        }

        public int GetCount(Process process, ActionItem actionItem)
        {
            if (!history.ContainsKey(process.ProcessName))
                return 0;
            var historyTable = history[process.ProcessName];
            if (!historyTable.ContainsKey(actionItem.action))
                return 0;
            return historyTable[actionItem.action].count;
        }

        public static History ReadHistory(string filename)
        {
            History history;
            try
            {
                using (StreamReader r = new StreamReader(filename))
                {
                    string s = r.ReadToEnd();
                    history = JsonConvert.DeserializeObject<History>(s);
                }
            }
            catch
            {
                history = new History();
            }
            history.filename = filename;
            return history;
        }
    }
}
