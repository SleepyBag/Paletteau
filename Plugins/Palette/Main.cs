using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Wox.Plugin;
using System.Threading;
using static Wox.Infrastructure.StringMatcher;
using Wox.Infrastructure;

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

        public Process GetActiveProcess()
        {
            IntPtr handle = GetForegroundWindow();
            uint pID;
           
            GetWindowThreadProcessId(handle, out pID);

            return Process.GetProcessById((Int32)pID);
        }

        public List<Result> Query(Query query)
        {
            var process = query.BackgroundProcess;
            if (process == null)
            {
                return new List<Result>();
            }
            var commandTable = setting.commandTables[process.ProcessName];
            var results = new List<Result>();
            foreach (var actionItem in commandTable) {
                var matchResult = StringMatcher.FuzzySearch(query.Search, actionItem.description);
                if (query.Search.Length == 0 || matchResult.Success)
                {
                    results.Add(new Result
                    {
                        Title = actionItem.description,
                        SubTitle = actionItem.action,
                        // IcoPath = Path.Combine("Images", "app.png"),
                        Score = matchResult.Score,
                        TitleHighlightData = matchResult.MatchData,
                        Action = _ =>
                        {
                            Task.Run(() =>
                            {
                                while (GetActiveProcess().Id != process.Id)
                                {
                                    Task.Delay(100);
                                };
                                SendKeys.SendWait(actionItem.action);
                                SendKeys.Flush();
                            });
                            return true;
                        }
                    });
                }
            }
            return results;
        }

        public void Init(PluginInitContext context)
        {
            setting = Setting.ReadSetting(
                Path.Combine(context.CurrentPluginMetadata.PluginDirectory, "settings.json")
            );
        }
    }
}
