using Microsoft.Extensions.DependencyInjection;
using OpenAI.Embeddings;
using OpenAI.Chat;
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
                .AddSingleton<EmbeddingClient>(provider => new EmbeddingClient("text-embedding-3-small", apiKey))
                .AddSingleton<ChatClient>(provider => new ChatClient("gpt-4o", apiKey)) 
                .AddSingleton<OpenAiEmbeddingService>()
                .AddSingleton<EmbeddingAnalysisService>()
                .AddSingleton<OpenAiTextGenerationService>()
                .AddSingleton<CosineSimilarity>()
                .AddSingleton<EuclideanDistance>()
                .AddSingleton<EmbeddingUtils>()
                .AddSingleton<PdfHelper>()
                .AddSingleton<CSVHelper>()
                .AddSingleton<JsonHelper>()
                .AddSingleton<PineconeService>()
                .AddSingleton<ProcessorAli>()
                .AddSingleton<Word2VecService>(provider => new Word2VecService("./Datasets/glove.6B.300d.txt"))  // Register Word2VecService
                .BuildServiceProvider();

            // var processor = serviceProvider.GetRequiredService<ProcessorAli>();
            // await processor.RunAsync();
            var analysis = serviceProvider.GetRequiredService<EmbeddingAnalysisService>();
            var csvHelper = serviceProvider.GetRequiredService<CSVHelper>();

            var list1 = new List<string>
            {
                "racket",
                "footstep",
                "happy",
                "software",
                "playground",
                "AI",
                "robotics",
                "cinema",
                "physics",
                "grip",
                "assination"
            };

            var list2 = new List<string>
            {
                "technology",
                "badminton",
                "violence",
                "mecha",
                "children"
            };

            // Words vs Words
            var wordsResult = await analysis.CompareWordsVsWordsAsync(list1, list2);
            csvHelper.ExportToCsv(wordsResult, "words.csv");

            // Words vs PDFs
            var pdfResult = await analysis.ComparePdfsvsWordsAsync(list2);
            csvHelper.ExportToCsv(pdfResult, "pdfs.csv");

            // Words vs Dataset
            await analysis.CreateDataSetEmbeddingsAsync(["Title", "Overview", "Genre"], "imdb_1000.csv", 25);
            var datasetResults = await analysis.compareDataSetVsWords("imdb_1000_Embeddings.json", "Title", "Overview", list2);
            csvHelper.ExportToCsv(datasetResults, "imdb_1000_Similarity.csv");
        }
    }
}