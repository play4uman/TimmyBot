﻿using AnswerExtraction.Algorithm.DocumentParsing;
using AnswerExtraction.Algorithm.DocumentRanking;
using AnswerExtraction.Algorithm.NLP;
using AnswerExtraction.API;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.Request;
using Shared.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Algorithm
{
    // Couldn't come up with a better name.
    public class Answerer : IAnswerer
    {
        public Answerer(Client apiClient, IBM25 bM25, IQueryParser queryParser, IParagraphSplitter paragraphSplitter)
        {
            _apiClient = apiClient;
            _bM25 = bM25;
            _queryParser = queryParser;
            _paragraphSplitter = paragraphSplitter;
        }

        private readonly Client _apiClient;
        private readonly IBM25 _bM25;
        private readonly IQueryParser _queryParser;
        private readonly IParagraphSplitter _paragraphSplitter;

        public async Task<string> AnswerAsync(string question, string subject)
        {
            question = AddQuestionMarkIfNotExists(question);
            var queryParseResult = await _queryParser.ParseQueryAsync(question);
            var fullMetadata = await _apiClient.FileAsync();
            var bestMatchedDocFromTag = Tags.BestMatch(queryParseResult.BM25Tokens, 
                fullMetadata.Metadata
                    .Where(m => m.Category.Equals(subject, StringComparison.OrdinalIgnoreCase)));

            string bestDoc;
            if (bestMatchedDocFromTag != null)
            {
                var bestDocUrl = bestMatchedDocFromTag.FilePath;
                bestDoc = await _apiClient.LoadDocIntoMemoryAsync(bestDocUrl);
            }
            else
            {
                bestDoc = await GetBestDocBasedOnBm25Score(queryParseResult.BM25Tokens, fullMetadata, subject);
            }

            var passages = _paragraphSplitter.SplitIntoParagraphs(bestDoc);
            return "abc";
        }

        private async Task<string> GetBestDocBasedOnBm25Score(string[] keywords, FileMetadataFullResponseDTO fileMetadata, string subject)
        {
            // Load all documents into memory and associate them with the number of words they have.
            var docPairs = await Task.WhenAll(fileMetadata.Metadata
                                        .Where(fm => fm.Category.Equals(subject, StringComparison.OrdinalIgnoreCase))
                                        .Select(async fm =>
                                        {
                                            var doc = await _apiClient.LoadDocIntoMemoryAsync(fm.FilePath);
                                            return (fm.Id, fm.OriginalName, doc, fm.WordCount, fm.KeywordsOccurances);
                                        }));
                        
            var termFrequencyMap = BM25.GetKeywordContainedInDocsMap(keywords, docPairs.Select(dp => dp.doc));
            var flaggedKeywords = GetFlaggedKeywods(keywords, fileMetadata);

            var scores = docPairs.Select(pair =>
                (Id: pair.Id, Doc: pair.doc,
                    Score: _bM25.Compute(pair.doc, pair.WordCount, flaggedKeywords.Where(fk => fk.FileId == pair.Id),
                        fileMetadata.Metadata.Length, fileMetadata.AvgWordCount, termFrequencyMap)))
                .OrderByDescending(sc => sc.Score)
                .ToList();

            await SendUnmatchedKeywords(flaggedKeywords.Where(fk => !fk.Matched));

            return scores.First().Doc;
        }

        private string AddQuestionMarkIfNotExists(string query)
        {
            return !query.EndsWith('?') ? query + "?" : query;
        }

        private List<FlaggedKeyword> GetFlaggedKeywods(string[] keywords, FileMetadataFullResponseDTO fileMetadata)
        {
            var result = new List<FlaggedKeyword>();
            foreach (var file in fileMetadata.Metadata)
            {
                foreach (var keyword in keywords)
                {
                    bool matched = file.KeywordsOccurances.TryGetValue(keyword, out int value);
                    result.Add(new FlaggedKeyword(file.Id, keyword, matched, matched ? value : null));
                }
            }
            return result;
        }

        private async Task SendUnmatchedKeywords(IEnumerable<FlaggedKeyword> flaggedUnsentKeywords)
        {
            foreach(var unsentKeyword in flaggedUnsentKeywords)
            {
                if (unsentKeyword.Times.HasValue) 
                {
                    await _apiClient.KeywordAsync(new FileKeywordOccurancesPostDTO
                    {
                        FileId = unsentKeyword.FileId,
                        Keyword = unsentKeyword.Keyword,
                        Times = unsentKeyword.Times.Value
                    });
                }
            }
        }


    }
}
