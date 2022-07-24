using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Paletteau.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Palette.Providers
{
    using CustomCommandTable = List<CustomActionItem>;               // command table type for a specific program

    internal class CustomProvider : IPaletteProvider
    {
        CustomSetting customSetting;

        public List<CommandItem> GetItems(ProcessIdentifier processIdentifier)
        {
            var processName = processIdentifier.name;
            var commandTable = customSetting.commandTables[processName];
            return commandTable.Select(customActionItem => new CommandItem
            {
                Title = customActionItem.description,
                SubTitle = TranslateAction(customActionItem),
                Action = (_, process) =>
                {
                    Task.Run(async () =>
                    {
                        // waiting for 3 seconds
                        for (int i = 0; i < 30; i++)
                        {
                            // if current process matched the target, send keys
                            if (GetActiveProcess().Id == process.Id)
                            {
                                if (customActionItem.type == "key")
                                {
                                    foreach (string key in ParseKeySequence(customActionItem.action))
                                    {
                                        // System.Console.WriteLine(GetActiveProcess().Id);
                                        SendKeys.SendWait(key);
                                        SendKeys.Flush();
                                        await Task.Delay(50);
                                    }
                                }
                                return;
                            }
                            await Task.Delay(100);
                        };
                    });
                    return true;
                }
            }).ToList();
        }

        public void Init(ProviderContext providerContext)
        {
            customSetting = providerContext.setting.ToObject<CustomSetting>();
            var commandTable = providerContext.commandTable;
            foreach (var processName in customSetting.commandTables.Keys)
            {
                commandTable.RegisterProvider(new ProcessIdentifier(processName), this);
            }
        }

        public bool IsUpdated()
        {
            return false;
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

        private static string TranslateAction(CustomActionItem actionItem)
        {
            string result = "";
            if (actionItem.type == "key")
                result = string.Join(" | ", Array.ConvertAll(ParseKeySequence(actionItem.action), TranslateKey));
            return result;
        }

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
                translatedKey = key.Substring(1, key.Length - 2).ToUpper();
            return translatedKey;
        }

        private static string[] ParseKeySequence(string key)
        {
            return key.Split('|');
        }

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
    }

    internal class CustomSetting
    {
        [JsonProperty("palettes")]
        public Dictionary<string, CustomCommandTable> commandTables = new Dictionary<string, CustomCommandTable>();
    }

    internal class CustomActionItem                              // type for a specific action for a program
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
            var item = (CustomActionItem)obj;
            return item.type == type && item.action == action && item.description == description;
        }

        public static bool operator !=(CustomActionItem a, CustomActionItem b) => !(a == b);
        public static bool operator ==(CustomActionItem a, CustomActionItem b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null)) return false;
            if (ReferenceEquals(b, null)) return false;
            return a.Equals(b);
        }
    }
}
