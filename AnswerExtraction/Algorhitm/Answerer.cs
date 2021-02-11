using AnswerExtraction.Algorhitm.DocumentRanking;
using AnswerExtraction.Algorhitm.NLP;
using AnswerExtraction.API;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Algorhitm
{
    // Couldn't come up with a better name.
    public class Answerer : IAnswerer
    {
        public Answerer(Client apiClient, IBM25 bM25, IQueryParser queryParser)
        {
            _apiClient = apiClient;
            _bM25 = bM25;
            _queryParser = queryParser;
        }

        private readonly Client _apiClient;
        private readonly IBM25 _bM25;
        private readonly IQueryParser _queryParser;

        public async Task<string> AnswerAsync(string question)
        {
            var keywords = _queryParser.ParseQuery(question);
            var fullMetadata = await _apiClient.FileAsync();
            var bestMatchedDocFromTag = Tags.BestMatch(keywords, fullMetadata.Metadata);

            string bestDoc;
            if (bestMatchedDocFromTag != null)
            {
                var bestDocUrl = bestMatchedDocFromTag.FilePath;
                bestDoc = await _apiClient.LoadDocIntoMemoryAsync(bestDocUrl);
            }
            else
            {
                bestDoc = await GetBestDocBasedOnBm25Score(keywords, fullMetadata);
            }
            return "abc";
        }

        private async Task<string> GetBestDocBasedOnBm25Score(string[] keywords, FileMetadataFullResponseDTO fileMetadata)
        {
            // Load all documents into memory and associate them with the number of words they have.
            var docPairs = await Task.WhenAll(fileMetadata.Metadata
                                        .Select(async fm =>
                                        {
                                            var doc = await _apiClient.LoadDocIntoMemoryAsync(fm.FilePath);
                                            return (fm.Id, fm.OriginalName, doc, fm.WordCount);
                                        }));
                        

            var termFrequencyMap = BM25.GetKeywordContainedInDocsMap(keywords, docPairs.Select(dp => dp.doc));

            // DEBUG ONLY
            var scores = docPairs.Select(pair =>
                (Id: pair.Id, Doc: pair.doc,
                    Score: _bM25.Compute(pair.doc, pair.WordCount, keywords, fileMetadata.Metadata.Length, fileMetadata.AvgWordCount, termFrequencyMap)))
                .OrderByDescending(sc => sc.Score);

            return scores.First().Doc;
        }
    }
}
