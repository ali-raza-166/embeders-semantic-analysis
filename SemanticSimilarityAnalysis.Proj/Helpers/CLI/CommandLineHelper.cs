#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SemanticSimilarityAnalysis.Proj.Helpers.Csv;
using SemanticSimilarityAnalysis.Proj.Services;

namespace SemanticSimilarityAnalysis.Proj
{
    public class CommandLineHelper
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandLineHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Default directories
        private readonly string defaultInputCsvDir = "Datasets/CSVs";
        private readonly string defaultOutputCsvDir = "Outputs/CSV";
        private readonly string defaultPdfDir = "Datasets/PDFs";
        private readonly string defaultJsonDir = "Outputs";

        public async Task ExecuteCommandAsync(IConfiguration configuration)
        {
            // Get the command from the configuration (e.g., command-line arguments)
            var command = configuration["command"];

            // If no command is provided, show help and exit
            if (string.IsNullOrEmpty(command))
            {
                ShowHelp();
                return;
            }

            // Execute the appropriate command based on the user input
            switch (command)
            {
                case "ww":
                    await ExecuteWordsVsWordsAsync(configuration);
                    break;

                case "wp":
                    await ExecuteWordsVsPdfsAsync(configuration);
                    break;

                case "pp":
                    await ExecutePdfsVsPdfsAsync(configuration);
                    break;

                case "wd":
                    await ExecuteWordsVsDatasetAsync(configuration);
                    break;

                default:
                    Console.WriteLine($"Invalid command: {command}");
                    ShowHelp();
                    break;
            }
        }

        // Method to execute the "words-vs-words" command
        private async Task ExecuteWordsVsWordsAsync(IConfiguration configuration)
        {
            // Command line arguments
            var list1 = configuration["list1"]?
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(word => word.Trim())
                .ToList();

            var list2 = configuration["list2"]?
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(word => word.Trim())
                .ToList();

            var outputFileName = configuration["output"] ?? "words_vs_words.csv";
            var outputDirectory = configuration["outputDir"] ?? defaultOutputCsvDir;

            var analysis = _serviceProvider.GetRequiredService<EmbeddingAnalysisService>();
            var csvHelper = _serviceProvider.GetRequiredService<CSVHelper>();
            var outputPath = Path.GetFullPath(Path.Combine(outputDirectory, outputFileName));

            // Prompt for list1 and list2 if they are not provided
            if (list1 == null || list2 == null)
            {
                Console.WriteLine("Required arguments are missing.");
                list1 = PromptForList("Enter the first list of words (comma-separated):");
                list2 = PromptForList("Enter the second list of words (comma-separated):");
            }

            var result = await analysis.CompareWordsVsWords(list1, list2);

            // Save the result to a CSV file
            csvHelper.ExportToCsv(result, outputFileName, outputDirectory);
            Console.WriteLine($"Results saved to {outputPath}");
        }

        // Method to execute the "words-vs-pdfs" command
        private async Task ExecuteWordsVsPdfsAsync(IConfiguration configuration)
        {
            // Command line arguments
            var words = configuration["words"]?.Split(',').ToList();
            var pdfFolder = configuration["pdf-folder"] ?? defaultPdfDir;
            var outputFileName = configuration["output"] ?? "words_vs_pdfs.csv";
            var outputDirectory = configuration["outputDir"] ?? defaultOutputCsvDir;

            var analysis = _serviceProvider.GetRequiredService<EmbeddingAnalysisService>();
            var csvHelper = _serviceProvider.GetRequiredService<CSVHelper>();
            var outputPath = Path.GetFullPath(Path.Combine(outputDirectory, outputFileName));

            // Prompt for words and pdfFolder if they are not provided
            if (words == null)
            {
                Console.WriteLine("Required arguments are missing.");
                words = PromptForList("Enter the list of words (comma-separated):");
            }
            if (!Directory.Exists(pdfFolder))
            {
                pdfFolder = PromptForDirectory("Enter the PDF folder path:", pdfFolder);
            }

            var result = await analysis.ComparePdfsvsWords(words, pdfFolder);

            // Save the result to a CSV file
            csvHelper.ExportToCsv(result, outputFileName, outputDirectory);
            Console.WriteLine($"Results saved to {outputPath}");
        }

        // Method to execute the "pdfs-vs-pdfs" command
        private async Task ExecutePdfsVsPdfsAsync(IConfiguration configuration)
        {
            // Command line arguments
            var pdfFolder = configuration["pdf-folder"] ?? defaultPdfDir;
            var outputFileName = configuration["output"] ?? "pdfs_vs_pdfs.csv";
            var outputDirectory = configuration["outputDir"] ?? defaultOutputCsvDir;

            var analysis = _serviceProvider.GetRequiredService<EmbeddingAnalysisService>();
            var csvHelper = _serviceProvider.GetRequiredService<CSVHelper>();
            var outputPath = Path.GetFullPath(Path.Combine(outputDirectory, outputFileName));

            if (!Directory.Exists(pdfFolder))
            {
                pdfFolder = PromptForDirectory("Enter the PDF folder path:", pdfFolder);
            }

            var result = await analysis.CompareAllPdfDocuments(pdfFolder);

            // Save the result to a CSV file
            csvHelper.ExportToCsv(result, outputFileName, outputDirectory);
            Console.WriteLine($"Results saved to {outputPath}");
        }

        // Method to execute the "words-vs-dataset" command
        /// <note>
        /// To start generating the dataset embeddings and then compare them with the words, we need to enter:
        /// 1. Wanted fields of the dataset to generate embeddings
        /// 2. Choose a field which will be the label of the record for SAVING into csv and for PLOTTING
        /// 3. Choose a field whose embeddings will be used to compare with the words' embeddings
        /// </note>
        private async Task ExecuteWordsVsDatasetAsync(IConfiguration configuration)
        {
            // Command line arguments
            var words = configuration["words"]?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word.Trim())
            .ToList(); ;
            var csvFileName = configuration["dataset"] ?? "imdb_1000.csv"; // Dataset CSV file
            var outputFileName = configuration["output"] ?? "dataset_vs_words.csv"; // Output file name
            var inputDirectory = configuration["inputDir"] ?? defaultInputCsvDir; // Directory containing the dataset
            var outputDirectory = configuration["outputDir"] ?? defaultOutputCsvDir; // Directory containing the output
            /// 
            /// Number of rows to process, default is -1 which means defaultProcessRows = 20 rows. (Check out CSVHelper.cs for more details)
            /// 
            var processRows = int.TryParse(configuration["rows"], out int rows) ? rows : -1;

            var analysis = _serviceProvider.GetRequiredService<EmbeddingAnalysisService>();
            var csvHelper = _serviceProvider.GetRequiredService<CSVHelper>();

            var csvFilePath = Path.GetFullPath(Path.Combine(inputDirectory, csvFileName));
            var outputPath = Path.GetFullPath(Path.Combine(outputDirectory, outputFileName));

            // Prompt for csvFileName and inputPath if they are not provided
            if (!File.Exists(csvFilePath))
            {
                csvFileName = PromptForFile("Enter the CSV dataset file name:", csvFileName);
                csvFilePath = Path.GetFullPath(Path.Combine(inputDirectory, csvFileName));
            }

            // Read fields
            var fields = csvHelper.ReadCsvFields(csvFilePath);

            // Prompt for words if they are not provided
            if (words == null || words.Count == 0)
            {
                Console.WriteLine("Required arguments are missing.");
                words = PromptForList("Enter the list of words (comma-separated):");
            }

            Console.WriteLine("Words: " + string.Join(", ", words));

            // 1. Prompt for embedding attributes/fields of the dataset to generate embeddings
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nAvailable fields ({string.Join(", ", fields)}):");
            Console.ResetColor();
            Console.Write("\nEnter the fields to use for embeddings");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("(comma-separated, RIGHT CASE as in the available fields)");
            Console.ResetColor();
            var embeddingAttributes = PromptForList(":");

            // Check if at least one attribute is provided
            if (embeddingAttributes.Count == 0)
            {
                Console.WriteLine("At least one attribute is required for embeddings.");
                return;
            }

            // 2. Prompt for label attribute 
            var labelAttribute = PromptForInput("Enter 1 field to use as the label:");
            if (labelAttribute == null)
            {
                Console.WriteLine("A label attribute is required.");
                return;
            }

            // 3. Prompt for attribute to get its embeddings and compare with words' embeddings
            var attributeToCompare = PromptForInput($"Enter the field you want to compare with the words:");

            if (embeddingAttributes.Count == 0)
            {
                Console.WriteLine("An attribute is required for comparison.");
                return;
            }

            // Define the JSON embeddings file path
            string jsonFileName = $"{Path.GetFileNameWithoutExtension(csvFileName)}_Embeddings.json";

            // Create embeddings
            await analysis.CreateDataSetEmbeddingsAsync(embeddingAttributes, csvFileName, processRows, inputDirectory, defaultJsonDir);

            // Compare
            var result = await analysis.compareDataSetVsWords(jsonFileName, labelAttribute, attributeToCompare, words, defaultJsonDir);

            // Save the result to a CSV file
            csvHelper.ExportToCsv(result, outputFileName, outputDirectory);

            Console.WriteLine($"Results saved to {outputPath}");
        }


        // Method to display help information
        private void ShowHelp()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Usage: dotnet run --command <command> [options]");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nCommands:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(@"
              ww --list1 <words> --list2 <words> [--output <path>] (Words vs. Words)
              wp --words <words> [--pdf-folder <path>] [--output <path>] (Words vs. PDFs)
              pp [--pdf-folder <path>] [--output <path>] (PDFs vs. PDFs)
              wd --words <words> [--dataset <path>] [--output <path>] [--rows <number>] (Words vs. Dataset)
            ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nArguments:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(@"
              --list1 <words>         Comma-separated list of words (for ww command) e.g., ""word1,word2,word3"".
              --list2 <words>         Comma-separated list of words (for ww command) e.g., ""word1,word2,word3"".
              --words <words>         Comma-separated list of words (for wp and wd commands) e.g., ""word1,word2,word3"".
                                      **Note**: Enclose the list in quotation marks (e.g., ""Business and Finance, Information Technology, Legal and Environmental"").
              --pdf-folder <path>     Path to the folder containing PDFs (for wp and pp commands).
              --dataset <path>        Path to the dataset CSV file (for wd command).
              --output <path>         Path to the output CSV file.
              --rows <number>         Number of rows to process from dataset (for wd command). Default is 20.
            ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nDefault values:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($@"
              --outputDir             {defaultOutputCsvDir}
              --inputDir              {defaultInputCsvDir}
              --pdf-folder            {defaultPdfDir}
              --dataset               imdb_1000.csv
            ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nExamples WITHOUT options:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(@"
              dotnet run --command ww --list1 word1,word2,word3 --list2 word1,word2,word3
              dotnet run --command wp --words word1,word2,word3
              dotnet run --command pp
              dotnet run --command wd --words word1,word2,word3
            ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nExamples WITH options:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(@"
              dotnet run --command ww --list1 ""word1,word2,word3"" --list2 ""word1,word2,word3"" --output output.csv
              dotnet run --command wp --words ""word1,word2,word3"" --pdf-folder pdfs --output output.csv
              dotnet run --command pp --pdf-folder pdfs --output output.csv
              dotnet run --command wd --words ""word1,word2,word3"" --dataset imdb_1000.csv --output output.csv --rows 100
            ");

            // Reset color to default
            Console.ResetColor();
        }


        // Method to prompt the user for a file
        private string PromptForFile(string prompt, string defaultValue)
        {
            string path;
            while (true)
            {
                Console.Write($"{prompt} (default: {defaultValue}): ");

                path = Console.ReadLine();

                if (string.IsNullOrEmpty(path))
                {
                    path = defaultValue;
                }
                if (File.Exists(path))
                {
                    return path;
                }
                Console.WriteLine("File does not exist. Please enter a valid path.");
            }
        }

        // Method to prompt the user for a directory
        private string PromptForDirectory(string prompt, string defaultValue)
        {
            string path;
            while (true)
            {
                Console.Write($"{prompt} (default: {defaultValue}): ");

                path = Console.ReadLine();

                if (string.IsNullOrEmpty(path))
                {
                    path = defaultValue;
                }
                if (Directory.Exists(path))
                {
                    return path;
                }
                Console.WriteLine("Directory does not exist. Please enter a valid path.");
            }
        }

        // Method to prompt the user for a single input
        private string PromptForInput(string prompt)
        {
            Console.Write($"{prompt} ");

            string input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input cannot be empty.");
            }

            return input;
        }

        // Method to prompt the user for a comma-separated list
        private List<string> PromptForList(string prompt)
        {
            Console.Write($"{prompt} ");

            string input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                return new List<string>();
            }

            Console.WriteLine("The input is: " + input);
            return input
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(word => word.Trim())
                .ToList();
        }
    }
}
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.