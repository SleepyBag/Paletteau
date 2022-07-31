using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Paletteau.Plugin;
using Paletteau.Infrastructure;
using Paletteau.Infrastructure.UserSettings;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Paletteau.Infrastructure.Windows;

namespace WindowManagement
{
    public class Main : IPlugin
    {
        List<WindowHandler> HiddenWindows = new List<WindowHandler>();

        public void Init(PluginInitContext context)
        {
        }

        public List<Result> Query(Query query)
        {
            Process process = query.BackgroundProcess;
            WindowHandler window = query.BackgroundWindow;
            // basic funcationalities
            var results = new List<Result> {
                new Result() { Title = "close main window", Action = _ => { process.CloseMainWindow(); return true; } },
                new Result() { Title = "force kill process", Action = _ => { process.Kill(); return true; } },
                new Result() { Title = "hide window", Action = _ => { window.Hide(); HiddenWindows.Add(window); return true; } },
            };
            if (window.IsOnTop())
            {
                results.Add(new Result()
                {
                    Title = "unpin window from top",
                    Action = _ => { window.UnpinOnTop(); return true; }
                });
            }
            else
            {
                results.Add(new Result()
                {
                    Title = "pin window on top",
                    Action = _ => { window.PinOnTop(); window.SetActive(); return true; }
                });
            }
            // remove all closed hidden windows
            HiddenWindows = HiddenWindows.Where(hiddenWindow => hiddenWindow.Exists()).ToList();
            // put hidden windows into results for recovery
            foreach (var hiddenWindow in HiddenWindows)
            {
                results.Add(
                    new Result() { 
                        Title = "show hidden window: " + hiddenWindow.GetWindowTitle(), 
                        Action = _ => { 
                            hiddenWindow.Show(); 
                            hiddenWindow.SetActive(); 
                            HiddenWindows.Remove(hiddenWindow); 
                            return true; 
                        }
                    }
                );
            }
            // filter by query
            results = StringMatcher.FilterListResultByTitle(results, query.Search);
            return results;
        }
    }
}
