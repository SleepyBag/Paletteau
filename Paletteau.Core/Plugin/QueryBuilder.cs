using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Paletteau.Plugin;

namespace Paletteau.Core.Plugin
{
    public static class QueryBuilder
    {
        public static Query Build(string text, Process backgroundProcess, Dictionary<string, PluginPair> nonGlobalPlugins)
        {
            // replace multiple white spaces with one white space
            var terms = text.Split(new[] { Query.TermSeperater }, StringSplitOptions.RemoveEmptyEntries);

            var rawQuery = string.Join(Query.TermSeperater, terms);
            string actionKeyword, search;
            List<string> actionParameters;
            if (terms.Length > 0 && nonGlobalPlugins.TryGetValue(terms[0], out var pluginPair) && !pluginPair.Metadata.Disabled)
            { // use non global plugin for query
                actionKeyword = terms[0];
                actionParameters = terms.Skip(1).ToList();
                search = actionParameters.Count > 0 ? rawQuery.Substring(actionKeyword.Length + 1) : string.Empty;
            }
            else
            { // non action keyword
                actionKeyword = string.Empty;
                actionParameters = terms.ToList();
                search = rawQuery;
            }

            var query = new Query
            {

                Terms = terms,
                RawQuery = rawQuery,
                ActionKeyword = actionKeyword,
                Search = search,
                BackgroundProcess = backgroundProcess,
                // Obsolete value initialisation
                ActionName = actionKeyword,
                ActionParameters = actionParameters
            };

            return query;
        }
    }
}