using System;
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
        public List<Result> Query(Query query)
        {
            var process = query.BackgroundProcess;
            var results = new List<Result>
            {
                new Result
                {
                    Title = process.ProcessName,
                    SubTitle = "process name"
                },
                new Result
                {
                    Title = process.MainWindowTitle,
                    SubTitle = "window title"
                },
                new Result
                {
                    Title = process.MainModule.FileName,
                    SubTitle = "executable path",
                    Action = _ =>
                    {
                        Process.Start(Path.GetDirectoryName(process.MainModule.FileName));
                        return true;
                    }
                },
                new Result {
                    Title = process.Id.ToString(),
                    SubTitle = "process id"
                },
                new Result {
                    Title = process.TotalProcessorTime.ToString(),
                    SubTitle = "total processor time"
                },
                new Result
                {
                    Title = process.UserProcessorTime.ToString(),
                    SubTitle = "user processor time"
                },
                new Result
                {
                    Title = process.VirtualMemorySize64.ToString(),
                    SubTitle = "virtual memory size"
                },
                new Result
                {
                    Title = process.BasePriority.ToString(),
                    SubTitle = "priority"
                },
            };
            // filter by query
            results = StringMatcher.FilterListResultBySubtitle(results, query.Search);
            return results;
        }

        public void Init(PluginInitContext context)
        {
        }

    }
}
