using AnswerExtraction.Algorithm.BERT;
using AnswerExtraction.Algorithm.DocumentParsing;
using AnswerExtraction.Algorithm.DocumentRanking;
using AnswerExtraction.Algorithm.NLP;
using AnswerExtraction.API;
using AnswerExtraction.Extensions.Enumerable;
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
        public Answerer(Client apiClient, IBM25 bM25, IQueryParser queryParser, IParagraphSplitter paragraphSplitter, IBertWrapper bertWrapper)
        {
            _apiClient = apiClient;
            _bM25 = bM25;
            _queryParser = queryParser;
            _paragraphSplitter = paragraphSplitter;
            _bertWrapper = bertWrapper;
        }

        private readonly Client _apiClient;
        private readonly IBM25 _bM25;
        private readonly IQueryParser _queryParser;
        private readonly IParagraphSplitter _paragraphSplitter;
        private readonly IBertWrapper _bertWrapper;

        public async Task<string> AnswerAsync(string question, string subject)
        {
            question = AddQuestionMarkIfNotExists(question);
            var queryParseResult = await _queryParser.ParseQueryAsync(question);
            var fullMetadata = await _apiClient.FileAsync();
            var bestMatchedDocFromTag = Tags.BestMatch(queryParseResult.BM25Tokens,
                fullMetadata.Metadata
                    .GetMetadataBasedOnSubject(subject));

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
            var bestPassage = GetBestPassage(passages, queryParseResult.BM25Tokens);
            var answer = await _bertWrapper.GetAnswerAsync(question, bestPassage);

            return answer;
        }

        private async Task<string> GetBestDocBasedOnBm25Score(string[] keywords, FileMetadataFullResponseDTO fileMetadata, string subject)
        {
            // Load all documents into memory and associate them with the number of words they have.
            var docPairs = await Task.WhenAll(fileMetadata.Metadata
                                        .GetMetadataBasedOnSubject(subject)
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

        private string GetBestPassage(string[] passages, string[] keywords)
        {
            var passageWordCountMap = passages.Select(p => new { passage = p, wordCount = p.Split(null).Length });
            int numberOfPassages = passages.Length;
            double avgWordCount = passageWordCountMap.Average(p => p.wordCount);
            var keywordsAsFlagged = keywords.Select(t => new FlaggedKeyword(default, t, false));
            var keywordsContainedInDocMap = BM25.GetKeywordContainedInDocsMap(keywords, passages);
            var passagesOrdered = passageWordCountMap
                                    .Select(p => new
                                    {
                                        passage = p.passage,
                                        score = _bM25.Compute(p.passage, p.wordCount, keywordsAsFlagged, numberOfPassages,
                                        avgWordCount, keywordsContainedInDocMap)
                                    })
                                    .OrderByDescending(pair => pair.score)
                                    .ToList();

            return passagesOrdered.First().passage;
        }
    }
}
