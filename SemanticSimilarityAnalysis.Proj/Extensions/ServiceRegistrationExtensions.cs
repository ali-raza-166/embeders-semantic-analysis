using LanguageDetection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;
using OpenAI.Embeddings;
using Pinecone;
using SemanticSimilarityAnalysis.Proj.Helpers;
using SemanticSimilarityAnalysis.Proj.Helpers.Csv;
using SemanticSimilarityAnalysis.Proj.Helpers.Json;
using SemanticSimilarityAnalysis.Proj.Helpers.Pdf;
using SemanticSimilarityAnalysis.Proj.Helpers.Text;
using SemanticSimilarityAnalysis.Proj.Pipelines;
using SemanticSimilarityAnalysis.Proj.Services;
using SemanticSimilarityAnalysis.Proj.Utils;

namespace SemanticSimilarityAnalysis.Proj.Extensions;
public static class ServiceRegistrationExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiKey = configuration["OPENAI_API_KEY"]
                     ?? throw new ArgumentNullException("OPENAI_API_KEY", "API key is not found.");
        var pineconeApiKey = configuration["PINECONE_API_KEY"]
                             ?? throw new InvalidOperationException("Pinecone API key is not set.");
        var chatModel = configuration["OpenAI:ChatModel"] ?? "gpt-4o"; 
        var embeddingModel = configuration["OpenAI:EmbeddingModel"] ?? "text-embedding-3-small";

        return services
            .AddSingleton(new EmbeddingClient(embeddingModel, apiKey))
            .AddSingleton(new ChatClient(chatModel, apiKey))
            .AddSingleton(new PineconeClient(pineconeApiKey))
            .AddSingleton<OpenAiEmbeddingService>()
            .AddSingleton<EmbeddingAnalysisService>()
            .AddSingleton<OpenAiTextGenerationService>()
            .AddSingleton<DimensionalityReductionService>(provider => new DimensionalityReductionService(2))
            .AddSingleton<CosineSimilarity>()
            .AddSingleton<EuclideanDistance>()
            .AddSingleton<EmbeddingUtils>()
            .AddSingleton<PdfHelper>()
            .AddSingleton<CSVHelper>()
            .AddSingleton<TextHelper>()
            .AddSingleton<JsonHelper>()
            .AddSingleton<CommandLineHelper>()
            .AddSingleton<PineconeService>()
            .AddSingleton<PineconeSetup>()
            .AddSingleton<ChatbotService>()
            .AddSingleton<RagAccuracyCalculator>()
            .AddSingleton<RagPipeline>()
            .AddSingleton<LanguageDetector>(provider =>
            {
                var detector = new LanguageDetector();
                detector.AddAllLanguages();
                return detector;
            })
            .AddSingleton<ProcessorAli>()
            .AddSingleton<OpenAiEmbeddingsDimReductionAndPlotting>()
            .AddSingleton<CSharpPythonConnector>()
            .AddSingleton<Word2VecEmbeddingsDimReductionAndPlotting>();
    }
}
