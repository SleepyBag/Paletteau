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
        List<WindowHandler> MinimizedWindows = new List<WindowHandler>();

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
                new Result() { Title = "minimize window", Action = _ => { window.Minimize(); MinimizedWindows.Add(window); return true; } },
            };
            // pin on top
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
            // maximize
            if (window.IsMaximized())
            {
                results.Add(new Result() {
                    Title = "restore maximized window", 
                    Action = _ => { window.Restore(); return true; } 
                });
            }
            else
            {
                results.Add(new Result() {
                    Title = "maximize window", 
                    Action = _ => { window.Maximize(); return true; } 
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

            // remove all closed or restored minimized windows
            MinimizedWindows = MinimizedWindows.Where(
                minimizedWindow => minimizedWindow.Exists() && minimizedWindow.IsMinimized()).ToList();
            // put minimized windows into results for recovery
            foreach (var minimizedWindow in MinimizedWindows)
            {
                results.Add(
                    new Result() { 
                        Title = "restore minimized window: " + minimizedWindow.GetWindowTitle(), 
                        Action = _ => { 
                            minimizedWindow.Restore(); 
                            minimizedWindow.SetActive(); 
                            MinimizedWindows.Remove(minimizedWindow); 
                            return true; 
                        }
                    }
                );
            }

            // filter by query
            results = StringMatcher.FilterListResultByTitle(results, query.Search);
            // keep original order - there aren't too many results
            foreach (var tuple in results.Select((result, index) => new {result, index}))
            {
                tuple.result.Score = results.Count() - tuple.index;
            }
            return results;
        }
    }
}
