using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Windows.Controls;
using System.Xml;
using Paletteau.Infrastructure.Logger;
using static Paletteau.Infrastructure.StringMatcher;

namespace Paletteau.Infrastructure
{
    public class StringMatcher
    {

        // public SearchPrecisionScore UserSettingSearchPrecision { get; set; }

        private readonly Alphabet _alphabet;
        private MemoryCache _cache;

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public StringMatcher()
        {
            _alphabet = new Alphabet();
            _alphabet.Initialize();

            NameValueCollection config = new NameValueCollection();
            config.Add("pollingInterval", "00:05:00");
            config.Add("physicalMemoryLimitPercentage", "1");
            config.Add("cacheMemoryLimitMegabytes", "30");
            _cache = new MemoryCache("StringMatcherCache", config);
        }

        public static StringMatcher Instance { get; internal set; }

        public static MatchResult FuzzySearch(string query, string stringToCompare)
        {
            return Instance.FuzzyMatch(query, stringToCompare);
        }

        public MatchResult FuzzyMatch(string query, string stringToCompare)
        {
            query = query.Trim();
            if (string.IsNullOrEmpty(stringToCompare) || string.IsNullOrEmpty(query)) return new MatchResult(false);
            var queryWithoutCase = query.ToLower();
            string translated = _alphabet.Translate(stringToCompare);

            string key = $"{queryWithoutCase}|{translated}";
            MatchResult match = _cache[key] as MatchResult;
            if (match == null)
            {
                //match = FuzzyMatchRecurrsive(
                //    queryWithoutCase, translated, 0, 0, new List<int>()
                //);

                // calculate matching score using dynamic programming
                // last dimension indicates whether it matches using the last character. It is used to find consecutive matches
                // 0 - do not use the last character; 1 - use the last character
                var maxConsecutive = Math.Min(queryWithoutCase.Length, 10);                                     // avoid explosion of memory usage
                var maxScoreMatrix = new int[queryWithoutCase.Length + 1, translated.Length + 1, maxConsecutive + 1];
                var matchedMatrix = new bool[queryWithoutCase.Length + 1, translated.Length + 1, maxConsecutive + 1];
                var previousIndexMatrix = new (int, int, int)[queryWithoutCase.Length + 1, translated.Length + 1, maxConsecutive + 1];
                // empty pattern matched all strings
                for (int i = 0; i <= translated.Length; i++)
                {
                    matchedMatrix[0, i, 0] = true;
                    for (int j = 1; j <= maxConsecutive; j++)
                        maxScoreMatrix[0, i, j] = -100000000;
                }
                // dynamic programming match
                for (int i = 1; i <= queryWithoutCase.Length; ++i)
                {
                    char queryChar = queryWithoutCase[i - 1];
                    for (int j = 1; j <= translated.Length; ++j)
                    {
                        char currentChar = translated[j - 1];
                        // the case of not to use current character
                        maxScoreMatrix[i, j, 0] = -100000000;                  // don't overwrite negative bonus
                        for (int k = 0; k <= maxConsecutive; k++)
                            if (matchedMatrix[i, j - 1, k])
                            {
                                matchedMatrix[i, j, 0] = matchedMatrix[i, j, 0] || matchedMatrix[i, j - 1, k];
                                if (maxScoreMatrix[i, j - 1, k] > maxScoreMatrix[i, j, 0]) {
                                    maxScoreMatrix[i, j, 0] = maxScoreMatrix[i, j - 1, k];
                                    if (k == 0)
                                        previousIndexMatrix[i, j, 0] = previousIndexMatrix[i, j - 1, k];   // jump to the last match
                                    else
                                        previousIndexMatrix[i, j, 0] = (i, j - 1, k);
                                }
                            }
                        // the case of to use current character
                        for (int k = 1; k <= maxConsecutive; k++)
                        {
                            maxScoreMatrix[i, j, k] = -100000000;                      // don't overwrite negative bonus
                        }
                        if (Char.ToLower(queryChar) == Char.ToLower(currentChar))
                        {
                            for (int k = 1; k <= maxConsecutive + 1; k++)
                            {
                                if (matchedMatrix[i - 1, j - 1, k - 1])
                                {
                                    int bonus = 0;
                                    int currentConsecutive = Math.Min(k, maxConsecutive);  // for the max consecutive, also continue from the same consecutive index
                                    matchedMatrix[i, j, currentConsecutive] = true;
                                    // penalty of first matching index - the later, the worse
                                    if (i == 1)
                                        bonus -= (j - 1) * 3;
                                    // 30 bonus for matching beginning of camel case
                                    if (Char.IsUpper(currentChar) && j != 1 && Char.IsLower(translated[j - 2]))
                                        bonus += 30;
                                    // 50 bonus for matching begining of a word
                                    if (j == 1 || (Char.IsLetter(currentChar) && !Char.IsLetterOrDigit(translated[j - 2])))
                                    {
                                        bonus += 50;
                                        if (Char.IsUpper(currentChar))
                                            bonus += 50;                               // 50 bonus for upper case word
                                    }
                                    bonus += (k - 1) * 10;                             // 10 bonus for each consecutive match
                                    maxScoreMatrix[i, j, currentConsecutive] = maxScoreMatrix[i - 1, j - 1, k - 1] + bonus;
                                    if (k != 1)
                                        previousIndexMatrix[i, j, currentConsecutive] = (i - 1, j - 1, k - 1);
                                    else
                                        previousIndexMatrix[i, j, currentConsecutive] = previousIndexMatrix[i - 1, j - 1, k - 1];   // jump to the last match
                                }
                            }
                        }
                    }
                }
                bool isMatch = false;
                for (int k = 0; k <= maxConsecutive; k++)
                    isMatch = isMatch || matchedMatrix[queryWithoutCase.Length, translated.Length, k];
                if (isMatch)
                {
                    int maxStringIndex = -1;
                    int maxConsecutiveIndex = -1;
                    int maxScore = -100000000;
                    // get the max score among all possibilities
                    for (int index2 = 1; index2 <= translated.Length; index2++)
                        for (int index3 = 0; index3 <= maxConsecutive; index3++)
                            if (matchedMatrix[queryWithoutCase.Length, index2, index3] && maxScoreMatrix[queryWithoutCase.Length, index2, index3] > maxScore)
                            {
                                maxScore = maxScoreMatrix[queryWithoutCase.Length, index2, index3];
                                maxStringIndex = index2;
                                maxConsecutiveIndex = index3;
                            }
                    int unmatched = translated.Length - queryWithoutCase.Length;   // penalty of string length, 5 for each unmatched character
                    maxScore -= (5 * unmatched);
                    maxScore += 100;                                               // avoid negative scores
                    List<int> matchList = new List<int>();
                    (int, int, int) index = (queryWithoutCase.Length, maxStringIndex, maxConsecutiveIndex);
                    while (index != (0, 0, 0))
                    {
                        matchList.Add(index.Item2 - 1);
                        index = previousIndexMatrix[index.Item1, index.Item2, index.Item3];
                    }
                    matchList.Reverse();
                    match = new MatchResult(isMatch, matchList, maxScore);
                }
                else
                    match = new MatchResult(isMatch);

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.SlidingExpiration = new TimeSpan(12, 0, 0);
                _cache.Set(key, match, policy);
            }

            return match;
        }

        public enum SearchPrecisionScore
        {
            Regular = 50,
            Low = 20,
            None = 0
        }
    }

    public class MatchResult
    {
        public MatchResult(bool success)
        {
            Success = success;
            // SearchPrecision = searchPrecision;
        }

        public MatchResult(bool success, List<int> matchData, int rawScore)
        {
            Success = success;
            // SearchPrecision = searchPrecision;
            MatchData = matchData;
            RawScore = rawScore;
        }

        public bool Success { get; set; }

        /// <summary>
        /// The final score of the match result with search precision filters applied.
        /// </summary>
        public int Score { get; private set; }

        /// <summary>
        /// The raw calculated search score without any search precision filtering applied.
        /// </summary>
        private int _rawScore;

        public int RawScore
        {
            get { return _rawScore; }
            set
            {
                _rawScore = value;
                Score = ScoreAfterSearchPrecisionFilter(_rawScore);
            }
        }

        /// <summary>
        /// Matched data to highlight.
        /// </summary>
        public List<int> MatchData { get; set; }

        public SearchPrecisionScore SearchPrecision { get; set; }

        public bool IsSearchPrecisionScoreMet()
        {
            return IsSearchPrecisionScoreMet(_rawScore);
        }

        private bool IsSearchPrecisionScoreMet(int rawScore)
        {
            return rawScore >= (int)SearchPrecision;
        }

        private int ScoreAfterSearchPrecisionFilter(int rawScore)
        {
            return IsSearchPrecisionScoreMet(rawScore) ? rawScore : 0;
        }
    }

}
