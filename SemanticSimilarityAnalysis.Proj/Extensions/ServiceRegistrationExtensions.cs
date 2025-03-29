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

/// <summary>
/// Provides an extension method to register services in the dependency injection container.
/// </summary>
public static class ServiceRegistrationExtensions
{
    /// <summary>
    /// Registers all required services for the semantic similarity analysis project.
    /// </summary>
    /// <param name="services">The service collection to which services will be added.</param>
    /// <param name="configuration">The application configuration settings.</param>
    /// <returns>The updated service collection with registered dependencies.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the OpenAI API key is missing.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the Pinecone API key is not set.</exception>
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"]
                     ?? throw new ArgumentNullException("OPENAI_API_KEY", "API key is not found.");
        var pineconeApiKey = configuration["Pinecone:ApiKey"]
                             ?? throw new InvalidOperationException("Pinecone API key is not set.");
        var chatModel = configuration["OpenAI:ChatModel"] ?? "gpt-4o"; 
        var embeddingModel = configuration["OpenAI:EmbeddingModel"] ?? "text-embedding-3-small";

        return services

            // Register OpenAI-based Services
            .AddSingleton(new EmbeddingClient(embeddingModel, apiKey))
            .AddSingleton(new ChatClient(chatModel, apiKey))
            
            // Register OpenAI-based Services
            .AddSingleton<OpenAiEmbeddingService>()
            .AddSingleton<EmbeddingAnalysisService>()
            .AddSingleton<OpenAiTextGenerationService>()
            .AddSingleton<ChatbotService>()

            // Register Pinecone-based services
            .AddSingleton<PineconeSetup>()
            .AddSingleton(new PineconeClient(pineconeApiKey))
            .AddSingleton<PineconeService>()

            // Register RAG (Retrieval-Augmented Generation) Components
            .AddSingleton<RagPipeline>()
            .AddSingleton<RagAccuracyCalculator>()

            // Register Embedding Utilities and Similarity Metrics
            .AddSingleton<DimensionalityReductionService>(provider => new DimensionalityReductionService(2))
            .AddSingleton<CosineSimilarity>()
            .AddSingleton<EmbeddingUtils>()

            // Register File Processing Helpers
            .AddSingleton<PdfHelper>()
            .AddSingleton<CSVHelper>()
            .AddSingleton<TextHelper>()
            .AddSingleton<JsonHelper>()
            .AddSingleton<CommandLineHelper>()

            // Register NLP and Processing Tools
            .AddSingleton<LanguageDetector>(provider =>
            {
                var detector = new LanguageDetector();
                detector.AddAllLanguages();
                return detector;
            })

            // Register Python Interoperability and Visualization Services
            .AddSingleton<ProcessorAli>()
            .AddSingleton<OpenAiEmbeddingsDimReductionAndPlotting>()
            .AddSingleton<Word2VecEmbeddingsDimReductionAndPlotting>()
            .AddSingleton<CSharpPythonConnector>();
    }
}
