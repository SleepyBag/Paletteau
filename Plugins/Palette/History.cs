using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Palette
{
    using HistoryTable = List<HistoryEntry>;

    internal class HistoryEntry
    {
        [JsonProperty("count")]
        public int count { get; set; }
        [JsonProperty("commandItem")]
        public CommandItem commandItem { get; set; }
        [JsonProperty("processIdentifier")]
        public ProcessIdentifier processIdentifier { get; set; }

        public HistoryEntry(ProcessIdentifier _processIdentifier, CommandItem item)
        {
            count = 0;
            processIdentifier = _processIdentifier;
            commandItem = item;
        }
    }

    internal class History
    {
        [JsonProperty("history")]
        HistoryTable history = new HistoryTable();
        string filename;
        
        public HistoryEntry GetEntry(ProcessIdentifier processIdentifier, CommandItem commandItem)
        {
            // TODO: could be faster by using hash table
            return history.Find(e => processIdentifier == e.processIdentifier && commandItem == e.commandItem);
        }

        public void Hit(Process process, CommandItem commandItem)
        {
            var processIdentifier = new ProcessIdentifier(process);
            HistoryEntry entry = GetEntry(processIdentifier, commandItem);

            if (entry == null)
            {
                entry = new HistoryEntry(processIdentifier, commandItem);
                history.Add(entry);
            }
            entry.count += 1;

            // asynchronously write to history file
            // TODO: what if the history file is large?
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

        public int GetCount(Process process, CommandItem commandItem)
        {
            var processIdentifier = new ProcessIdentifier(process);
            HistoryEntry entry = GetEntry(processIdentifier, commandItem);
            if (entry == null)
                return 0;
            return entry.count;
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
