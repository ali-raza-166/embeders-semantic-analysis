using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SemanticSimilarityAnalysis.Proj.Extensions;
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