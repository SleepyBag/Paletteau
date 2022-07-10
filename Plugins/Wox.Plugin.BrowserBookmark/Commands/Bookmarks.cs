using System;
using System.Collections.Generic;
using System.Linq;
using Paletteau.Infrastructure;

namespace Paletteau.Plugin.BrowserBookmark.Commands
{
    internal static class Bookmarks
    {
        internal static bool MatchProgram(Bookmark bookmark, string queryString)
        {
            var nameMatchResult = StringMatcher.FuzzySearch(queryString, bookmark.Name);
            var urlMatchResult = StringMatcher.FuzzySearch(queryString, bookmark.Url);
            if (nameMatchResult.Success || urlMatchResult.Success)
            {
                bookmark.Score = Math.Max(nameMatchResult.Score, urlMatchResult.Score);
                return true;
            }

            return false;
        }

    }
}
