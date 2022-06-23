using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var item = (ActionItem)obj;
            return item.type == type && item.action == action && item.description == description;
        }

        public static bool operator !=(ActionItem a, ActionItem b) => !(a == b);
        public static bool operator ==(ActionItem a, ActionItem b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null)) return false;
            if (ReferenceEquals(b, null)) return false;
            return a.Equals(b);
        }
    }

    internal class Setting
    {
        [JsonProperty("palettes")]
        public Dictionary<string, CommandTable> commandTables = new Dictionary<string, CommandTable>();

        public static Setting ReadSetting(string filename)
        {
            Setting setting;
            try
            {
                using (StreamReader r = new StreamReader(filename))
                {
                    string s = r.ReadToEnd();
                    setting = JsonConvert.DeserializeObject<Setting>(s);
                }
            }
            catch
            {
                setting = new Setting();
            }
            return setting;
        }

    }
}
