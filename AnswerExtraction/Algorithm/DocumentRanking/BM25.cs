﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AnswerExtraction.Algorithm.DocumentRanking
{
    public class BM25 : IBM25
    {
        private double _k1 = 1.2;
        public double K1 {
            get => _k1;
            set 
            {
                if (value is < 1.2 or > 2.0)
                    throw new ArgumentException("k1 must be between 1.2 and 2.0");

                _k1 = value;
            } 
        }
        public double B { get; set; } = 0.75;

        public double Compute(string doc, int docLength, IEnumerable<FlaggedKeyword> flaggedKeywords, int numberOfDocs, double avgDocLength,
            Dictionary<string, int> keywordsContainedInMap)
        {
            return flaggedKeywords.Sum(kw =>
            {
                return KeywordScore(doc, docLength, kw, numberOfDocs, avgDocLength, keywordsContainedInMap);
            }); 
        }

        private double KeywordScore(string doc, int docLength, FlaggedKeyword keyword, int numberOfDocs, double avgDocLength,
            Dictionary<string, int> keywordsContainedInMap)
        {
            var idf = IDF(numberOfDocs, keywordsContainedInMap[keyword.Keyword]);
            int termFrequency = keyword.Matched ? keyword.Times.Value : TermFrequencyCount(doc, keyword.Keyword);
            keyword.Times = termFrequency;

            var result = idf * (termFrequency * (K1 + 1) / (termFrequency + K1 * (1 - B + (B * docLength / avgDocLength))));
            return result;
        }

        // I think it's safe to assume that we can use the raw term frequency as BM25 balances TF by using doc length and avg doc length
        // todo: Needs confirmation
        private int TermFrequencyCount(string doc, string keyword)
        {
            var regex = new Regex(keyword, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            int matches = regex.Matches(doc).Count;
            return matches;
        }

        private double IDF(int numberOfDocs, int numberOfDocsContainingKeyword)
        {
            var intermediateResult = (double)((numberOfDocs - numberOfDocsContainingKeyword + 0.5) / (numberOfDocsContainingKeyword + 0.5)) + 1;
            return Math.Log(intermediateResult);
        }

        public static Dictionary<string, int> GetKeywordContainedInDocsMap(IEnumerable<string> keywords, IEnumerable<string> docs)
        {
            return keywords.ToDictionary(
                    kw => kw,
                    kw => NumberOfDocsContainingAKeyword(docs, kw)
                );
        }

        public static int NumberOfDocsContainingAKeyword(IEnumerable<string> docs, string keyword)
        {
            return docs.Count(d => d.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }
    }
}
