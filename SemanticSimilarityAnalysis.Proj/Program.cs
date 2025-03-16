using LanguageDetection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;
using OpenAI.Embeddings;
using SemanticSimilarityAnalysis.Proj;
using SemanticSimilarityAnalysis.Proj.Helpers;
using SemanticSimilarityAnalysis.Proj.Helpers.Csv;
using SemanticSimilarityAnalysis.Proj.Helpers.Json;
using SemanticSimilarityAnalysis.Proj.Helpers.Pdf;
using SemanticSimilarityAnalysis.Proj.Pipelines;
using SemanticSimilarityAnalysis.Proj.Services;
using SemanticSimilarityAnalysis.Proj.Utils;

var model = "text-embedding-3-small";
//var model = "text-embedding-ada-002";
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                         ?? throw new ArgumentNullException("api_key", "API key is not found.");

// Setup dependency injection
var serviceProvider = new ServiceCollection()
    .AddSingleton<EmbeddingClient>(provider => new EmbeddingClient(model, apiKey))

    .AddSingleton<ChatClient>(provider => new ChatClient("gpt-4o", apiKey))
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
    .AddSingleton<Word2VecEmbeddingsDimReductionAndPlotting>()
    .BuildServiceProvider();


// var processor = serviceProvider.GetRequiredService<ProcessorAli>();
// await processor.RunAsync();

///
/// For command line
/// 
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path so MAKE SURE you are in [SemanticSimilarityAnalysis.Proj] directory
    .AddCommandLine(args)
    .Build();

var commandLineHelper = serviceProvider.GetRequiredService<CommandLineHelper>();
await commandLineHelper.ExecuteCommandAsync(configuration);

///
/// For testing
/// Create output csv from set of phrases
///
//var analysis = serviceProvider.GetRequiredService<EmbeddingAnalysisService>();
//var csvHelper = serviceProvider.GetRequiredService<CSVHelper>();

////// Call the function to compare all phrases in the folder
//var results = await analysis.CompareAllPhrasesAsync();

//// Print the results
//foreach (var fileResult in results)
//{
//    Console.WriteLine($"File: {fileResult.Key}");
//    foreach (var plotPoint in fileResult.Value)
//    {
//        Console.WriteLine($"  Phrase: {plotPoint.Label}");
//        foreach (var pair in plotPoint.Similarities)
//        {
//            Console.WriteLine($"    Similarity with '{pair.Key}': {pair.Value}");
//        }
//    }
//}

//csvHelper.ExportAllPhrasesToCsv(results);