using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SemanticSimilarityAnalysis.Proj.Extensions;
using SemanticSimilarityAnalysis.Proj.Helpers.Text;
using SemanticSimilarityAnalysis.Proj.Services;
using SemanticSimilarityAnalysis.Proj.Utils;

var configurations = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();
Console.WriteLine($"DEBUG: OpenAI API Key = {configurations["OpenAI:ApiKey"]}");
Console.WriteLine($"DEBUG: Pinecone API Key = {configurations["Pinecone:ApiKey"]}");

var serviceProvider = new ServiceCollection()
    .RegisterServices(configurations)
    .BuildServiceProvider();

try
{
    var processor = serviceProvider.GetRequiredService<ProcessorAli>();
    await processor.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Application error: {ex.Message}");
}



///
/// For command line
/// 
//var configuration = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path so MAKE SURE you are in [SemanticSimilarityAnalysis.Proj] directory
//    .AddCommandLine(args)
//    .Build();

//var commandLineHelper = serviceProvider.GetRequiredService<CommandLineHelper>();
//await commandLineHelper.ExecuteCommandAsync(configuration);

var analysis = serviceProvider.GetRequiredService<EmbeddingAnalysisService>();

///
/// For Word2Vec comparisons
///

var textHelper = serviceProvider.GetRequiredService<TextHelper>();

// Example 1: Compare two lists of words
//var words1 = textHelper.ExtractWordsFromTextFile("pestle.txt");
//var words2 = textHelper.ExtractWordsFromTextFile("w_pestle.txt");
//var words3 = textHelper.ExtractWordsFromTextFile("ML.txt");
//var words4 = textHelper.ExtractWordsFromTextFile("ML2.txt");

//analysis.w2VecCompareWordsVsWords(
//    words1: words1,
//    words2: words2,
//    outputFileName: "word2vec_wordsVsWords_pestle.csv",
//    filePath: @"../../../Datasets/glove.6B.300d.txt",
//    outputDir: "../../../Outputs/CSVs"
//);

//Console.WriteLine("Word vs Word comparison completed. Results saved to CSV.");

// Example 2: Compare dataset with a list of words/phrases
var books = textHelper.ExtractWordsFromTextFile("books.txt");
Console.WriteLine($"books: {books}");
analysis.w2VecCompareDatasetVsWords(
    labelField: "title",
    embeddingField: "description",
    inputWordsOrPhrases: books,
    gloVeFilePath: @"../../../Datasets/glove.6B.300d.txt",
    inputDir: @"../../../Datasets/CSVs",
    inputFileName: "books.csv",
    outputDir: @"../../../Outputs/CSVs/",
    outputFileName: "word2vec_datasetVsWords.csv"
);

Console.WriteLine("Dataset vs Words comparison completed. Results saved to CSV.");

var movies = textHelper.ExtractWordsFromTextFile("movies.txt");
analysis.w2VecCompareDatasetVsWords(
    labelField: "Title",
    embeddingField: "Overview",
    inputWordsOrPhrases: movies,
    gloVeFilePath: @"../../../Datasets/glove.6B.300d.txt",
    inputDir: @"../../../Datasets/CSVs",
    inputFileName: "imdb_1000.csv",
    outputDir: @"../../../Outputs/CSVs/",
    outputFileName: "word2vec_datasetVsWords(MOVIES).csv"
);

//Console.WriteLine("Dataset vs Words comparison completed. Results saved to CSV.");

//var categories = textHelper.ExtractWordsFromTextFile("spotify.txt");
//for (int i = 0; i < categories.Count; i++)
//{
//    Console.WriteLine(categories[i]);
//}
//analysis.w2VecCompareDatasetVsWords(
//    labelField: "title",
//    embeddingField: "lyrics",
//    inputWordsOrPhrases: categories,
//    gloVeFilePath: @"../../../Datasets/glove.6B.300d.txt",
//    inputDir: @"../../../Datasets/CSVs",
//    inputFileName: "Lyrics.csv",
//    outputDir: @"../../../Outputs/CSVs/",
//    outputFileName: "word2vec_datasetVsWords(LYRICS).csv"
//);

//Console.WriteLine("Dataset vs Words comparison completed. Results saved to CSV.");

