using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palette
{
    using CommandTable = List<ActionItem>;               // command table type for a specific program

    public class ActionItem                              // type for a specific action for a program
    {
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("action")]
        public string action { get; set; }
    }

    internal class Setting
    {
        [JsonProperty("palettes")]
        public Dictionary<string, CommandTable> commandTables { get; set; }

        public static Setting ReadSetting(string filename)
        {
            using (StreamReader r = new StreamReader(filename))
            {
                string s = r.ReadToEnd();
                Setting setting = JsonConvert.DeserializeObject<Setting>(s);
                return setting;
            }
        }

    }
}
