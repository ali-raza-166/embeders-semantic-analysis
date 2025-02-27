using SemanticSimilarityAnalysis.Proj.Services;

namespace SemanticSimilarityAnalysis.Proj
{
    internal abstract class Program
    {
        private static async Task Main(string[] args)
        {
            //var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
            //             ?? throw new ArgumentNullException("api_key", "API key is not found.");
            //var serviceProvider = new ServiceCollection()
            //    .AddSingleton<EmbeddingClient>(provider => new EmbeddingClient("text-embedding-ada-002", apiKey))
            //    .AddSingleton<OpenAiEmbeddingService>()
            //    .AddSingleton<PineconeService>()
            //    .AddSingleton<CosineSimilarity>()
            //    .AddSingleton<EuclideanDistance>()
            //    .AddSingleton<OpenAiTextGenerationService>()
            //    .AddSingleton<ProcessorAli>()  // Register ProcessorAli to be used in Main()
            //    .BuildServiceProvider();

            //var processor = serviceProvider.GetRequiredService<ProcessorAli>();
            //await processor.RunAsync();
            var analysis = new EmbeddingAnalysisService();
            await analysis.ProcessDataSetEmbeddingsAsync(["Title", "Overview", "Genre"], "imdb_1000.csv");
        }
    }

}