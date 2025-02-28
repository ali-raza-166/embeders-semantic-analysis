using Microsoft.Extensions.DependencyInjection;
using OpenAI.Embeddings;
using SemanticSimilarityAnalysis.Proj.Helpers.Csv;
using SemanticSimilarityAnalysis.Proj.Helpers.Json;
using SemanticSimilarityAnalysis.Proj.Helpers.Pdf;
using SemanticSimilarityAnalysis.Proj.Services;
using SemanticSimilarityAnalysis.Proj.Utils;

namespace SemanticSimilarityAnalysis.Proj
{
    internal abstract class Program
    {
        private static async Task Main(string[] args)
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                         ?? throw new ArgumentNullException("api_key", "API key is not found.");
            var serviceProvider = new ServiceCollection()
                .AddSingleton<EmbeddingClient>(provider => new EmbeddingClient("text-embedding-ada-002", apiKey))
                .AddSingleton<OpenAiEmbeddingService>()
                //.AddSingleton<PineconeService>()
                .AddSingleton<EmbeddingAnalysisService>()
                .AddSingleton<CosineSimilarity>()
                .AddSingleton<EuclideanDistance>()
                .AddSingleton<EmbeddingUtils>()
                .AddSingleton<PdfHelper>()
                .AddSingleton<CSVHelper>()
                .AddSingleton<JsonHelper>()
                .AddSingleton<OpenAiTextGenerationService>()
                .AddSingleton<ProcessorAli>()  // Register ProcessorAli to be used in Main()
                .BuildServiceProvider();

            //var processor = serviceProvider.GetRequiredService<ProcessorAli>();
            //await processor.RunAsync();
            var analysis = serviceProvider.GetRequiredService<EmbeddingAnalysisService>();
            await analysis.ProcessDataSetEmbeddingsAsync(["Title", "Overview", "Genre"], "imdb_1000.csv", "");
            await analysis.AnalyzeEmbeddingsAsync("imdb_1000_Embeddings.json", "Title", "Overview", ["romantic comedy", "investigate"]);
        }
    }
}