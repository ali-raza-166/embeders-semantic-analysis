using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SemanticSimilarityAnalysis.Proj;
using SemanticSimilarityAnalysis.Proj.Extensions;
using SemanticSimilarityAnalysis.Proj.Utils;

var configurations = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var serviceProvider = new ServiceCollection()
    .RegisterServices(configurations) 
    .BuildServiceProvider();

var processor = serviceProvider.GetRequiredService<ProcessorAli>();
await processor.RunAsync();



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