using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Paletteau.Plugin;
using Paletteau.Infrastructure;
using Paletteau.Infrastructure.UserSettings;

namespace SourceInsightPalette
{
    class Main : IPlugin
    {
        SourceInsightSetting sourceInsightSetting;

        public List<Result> Query(Query query)
        {
            var process = query.BackgroundProcess;
            if (process == null || process.ProcessName != "sourceinsight4")
            {
                return null;
            }

            var sourceInsightExePath = process.MainModule.FileName;
            var results = new List<Result>();
            foreach (var commandEntry in sourceInsightSetting.commandTable)
            {
                var command = commandEntry.Key;
                var key = commandEntry.Value;
                var matchResult = StringMatcher.FuzzySearch(query.Search, command);
                if (query.Search.Length == 0 || matchResult.Success)
                {
                    // put item into result list
                    results.Add(new Result
                    {
                        Title = command,
                        SubTitle = key,
                        Score = matchResult.Score,      // history always first
                        TitleHighlightData = matchResult.MatchData,
                        Action = _ =>
                        {
                            Process.Start(sourceInsightExePath, "-i -c \"" + command + "\"");
                            return true;
                        }
                    });
                }
            }
            return results;
        }

        public void Init(PluginInitContext context)
        {
            sourceInsightSetting = SourceInsightSetting.LoadSourceInsightSetting();
        }
    }
}
