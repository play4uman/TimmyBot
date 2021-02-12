using AnswerExtraction.Algorhitm;
using AnswerExtraction.Algorhitm.DocumentRanking;
using AnswerExtraction.Algorhitm.NLP;
using AnswerExtraction.API;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Extensions.DI
{
    public static class ServiceCollectionAddAnswerExtractionExtensions
    {
        public static IServiceCollection AddAnswerExtraction(this IServiceCollection services, string apiUrl)
        {
            services
                .AddTransient<IBM25, BM25>()
                .AddTransient<IQueryParser, QueryParser>()
                .AddTransient<IAnswerer, Answerer>()
                .AddTransient<Client>(sc => new Client(apiUrl, sc.GetRequiredService<HttpClient>()));
            return services;
        }
    }
}
