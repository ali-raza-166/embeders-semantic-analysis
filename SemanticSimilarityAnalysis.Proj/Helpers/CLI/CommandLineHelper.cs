#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SemanticSimilarityAnalysis.Proj.Helpers.Csv;
using SemanticSimilarityAnalysis.Proj.Helpers.Text;
using SemanticSimilarityAnalysis.Proj.Services;


namespace SemanticSimilarityAnalysis.Proj
{
    /// <summary>
    /// Helper class for executing commands related to semantic similarity analysis from the command line.
    /// </summary>
    /// <remarks>
    /// This class handles the execution of various commands such as comparing words vs. words, words vs. PDFs, and words vs. datasets.
    /// It utilizes dependency injection to access required services and supports default directories for input and output files.
    /// The class processes user input, performs semantic similarity analysis, and exports results to CSV files.
    /// </remarks>
    public class CommandLineHelper
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandLineHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Default values
        private readonly string defaultPdfDir = "Datasets/PDFs";
        private readonly string defaultTxtDir = "Datasets/TXTs";
        private readonly string defaultInputCsvDir = "Datasets/CSVs";
        private readonly string defaultOutputCsvDir = "Outputs/CSVs";
        private readonly string defaultJsonDir = "Outputs";
        private readonly string defaultDataset = "imdb_1000.csv";

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
                // Words vs. Words
                case "ww":
                    await ExecuteWordsVsWordsAsync(configuration);
                    break;

                // Words vs. PDFs
                case "wp":
                    await ExecuteWordsVsPdfsAsync(configuration);
                    break;

                // Words vs. Dataset
                case "wd":
                    await ExecuteWordsVsDatasetAsync(configuration);
                    break;

                //case "pp":
                //    await ExecutePdfsVsPdfsAsync(configuration);
                //    break;

                // Invalid command
                default:
                    Console.WriteLine($"Invalid command: {command}");
                    ShowHelp();
                    break;
            }
        }

        // Method to execute the "words-vs-words" command
        private async Task ExecuteWordsVsWordsAsync(IConfiguration configuration)
        {
            // Get output file name and directory from configuration, or use defaults
            var outputFileName = configuration["output"] ?? "words_vs_words.csv";
            var outputDirectory = configuration["outputDir"] ?? defaultOutputCsvDir;
            var outputPath = Path.Combine(outputDirectory, outputFileName);

            // Process list1 and list2 from command-line arguments
            var list1 = ProcessInput(configuration["list1"] ?? "", "Please provide the first list of words or a text file path:");
            var list2 = ProcessInput(configuration["list2"] ?? "", "Please provide the second list of words or a text file path:");

            // Ensure both lists contain at least one word before proceeding
            if (list1.Count() == 0 || list2.Count() == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Both lists must contain at least one word. Exiting...");
                Console.ResetColor();
                return;
            }

            // Retrieve required services for analysis and CSV export
            var analysis = _serviceProvider.GetRequiredService<EmbeddingAnalysisService>();
            var csvHelper = _serviceProvider.GetRequiredService<CSVHelper>();


            // Perform the word similarity analysis
            var results = await analysis.CompareWordsVsWords(list1, list2);

            // Save the results to a CSV file
            csvHelper.ExportToCsv(results, outputFileName, outputDirectory);

            // Display success message with output file location
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Results saved to {outputPath}");
            Console.ResetColor();
        }


        // Method to execute the "words-vs-pdfs" command
        private async Task ExecuteWordsVsPdfsAsync(IConfiguration configuration)
        {
            // Command line arguments
            var pdfFolder = configuration["pdf-folder"] ?? defaultPdfDir;
            var outputFileName = configuration["output"] ?? "words_vs_pdfs.csv";
            var outputDirectory = configuration["outputDir"] ?? defaultOutputCsvDir;

            var words = ProcessInput(configuration["words"] ?? "", "Please provide a list of words or a text file path: ");

            if (words.Count() == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Both lists must contain at least one word. Exiting...");
                Console.ResetColor();
                return;
            }

            var analysis = _serviceProvider.GetRequiredService<EmbeddingAnalysisService>();
            var csvHelper = _serviceProvider.GetRequiredService<CSVHelper>();
            var outputPath = Path.GetFullPath(Path.Combine(outputDirectory, outputFileName));

            // Prompt for words if they are not provided


            // Prompt for pdfFolder if it does not exist
            if (!Directory.Exists(pdfFolder))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning: The specified PDF folder does not exist.");
                Console.ResetColor();
                pdfFolder = PromptForDirectory("Enter the PDF folder path:", pdfFolder);
            }

            // Perform the analysis
            var results = await analysis.ComparePdfsvsWords(words, pdfFolder);

            // Save the result to a CSV file
            csvHelper.ExportToCsv(results, outputFileName, outputDirectory);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Results saved to {outputPath}");
            Console.ResetColor();
        }


        // Method to execute the "pdfs-vs-pdfs" command
        //private async Task ExecutePdfsVsPdfsAsync(IConfiguration configuration)
        //{
        //    // Command line arguments
        //    var pdfFolder = configuration["pdf-folder"] ?? defaultPdfDir;
        //    var outputFileName = configuration["output"] ?? "pdfs_vs_pdfs.csv";
        //    var outputDirectory = configuration["outputDir"] ?? defaultOutputCsvDir;

        //    var analysis = _serviceProvider.GetRequiredService<EmbeddingAnalysisService>();
        //    var csvHelper = _serviceProvider.GetRequiredService<CSVHelper>();
        //    var outputPath = Path.GetFullPath(Path.Combine(outputDirectory, outputFileName));

        //    if (!Directory.Exists(pdfFolder))
        //    {
        //        pdfFolder = PromptForDirectory("Enter the PDF folder path:", pdfFolder);
        //    }

        //    var result = await analysis.CompareAllPdfDocuments(pdfFolder);

        //    // Save the result to a CSV file
        //    csvHelper.ExportToCsv(result, outputFileName, outputDirectory);
        //    Console.WriteLine($"Results saved to {outputPath}");
        //}


        // Method to execute the "words-vs-dataset" command
        /// <note>
        /// To start generating the dataset embeddings and then compare them with the words, we need to enter:
        /// 1. Choose fields from the dataset for labeling and generating embeddings
        /// 2. Choose a field which will be the label of the record for SAVING into csv and for PLOTTING
        /// 3. Choose a field whose embeddings will be used to compare with the words' embeddings
        /// </note>
        private async Task ExecuteWordsVsDatasetAsync(IConfiguration configuration)
        {
            // Command line arguments
            var inputDirectory = configuration["inputDir"] ?? defaultInputCsvDir; // Directory containing the dataset
            var csvFileName = configuration["dataset"] ?? defaultDataset; // Dataset CSV file
            var outputDirectory = configuration["outputDir"] ?? defaultOutputCsvDir; // Directory containing the output
            var outputFileName = configuration["output"] ?? "dataset_vs_words.csv"; // Output file name
            var processRows = int.TryParse(configuration["rows"], out int rows) ? rows : -1;

            var analysis = _serviceProvider.GetRequiredService<EmbeddingAnalysisService>();
            var csvHelper = _serviceProvider.GetRequiredService<CSVHelper>();

            var csvFilePath = Path.GetFullPath(Path.Combine(inputDirectory, csvFileName));
            var outputPath = Path.GetFullPath(Path.Combine(outputDirectory, outputFileName));

            // Prompt for csvFileName and inputPath if they are not provided　
            if (!File.Exists(csvFilePath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning: The specified dataset file does not exist.");
                Console.ResetColor();
                csvFileName = PromptForFile("Enter the CSV dataset file name:", defaultDataset);
                csvFilePath = Path.GetFullPath(Path.Combine(inputDirectory, csvFileName));
            }

            // Read fields
            var fields = csvHelper.ReadCsvFields(csvFilePath);

            // Prompt for words if they are not provided
            var words = ProcessInput(configuration["words"] ?? "", "Please provide the list of words or a text file path: ");
            Console.WriteLine("Words: " + string.Join(", ", words));

            // 1. Prompt for embedding attributes/fields of the dataset to generate embeddings
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nAvailable fields ({string.Join(", ", fields)}):");
            Console.ResetColor();
            Console.Write("\nEnter the fields to be extracted from the dataset ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("(comma-separated, RIGHT CASE as in the available fields)");
            Console.ResetColor();
            var embeddingAttributes = PromptForList(":");

            // Check if at least one attribute is provided
            if (embeddingAttributes.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: At least one attribute is required for embeddings.");
                Console.ResetColor();
                return;
            }

            // 2. Prompt for label attribute 
            var labelAttribute = PromptForInput("Enter 1 field to use as the label:");
            if (string.IsNullOrEmpty(labelAttribute))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: A label attribute is required.");
                Console.ResetColor();
                return;
            }

            // 3. Prompt for attribute to get its embeddings and compare with words' embeddings
            var attributeToCompare = PromptForInput($"Enter the field you want to compare with the words:");
            if (string.IsNullOrEmpty(attributeToCompare))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: An attribute is required for comparison.");
                Console.ResetColor();
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Results saved to {outputPath}");
            Console.ResetColor();
        }


        /// <summary>
        /// Displays the help message.
        /// </summary>
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
                wd --words <words> [--dataset <path>] [--output <path>] [--rows <number>] (Words vs. Dataset)
            ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nArguments:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(@"
                --command <command>     The command to execute. Must be one of the following:
                                        - ww: Compare two lists of words.
                                        - wp: Compare a list of words with PDF documents.
                                        - wd: Compare a list of words with a dataset.

                --list1 <words>         Comma-separated list of words for the first list (for ww command).
                                        Example: ""apple,banana,orange"" or provide a text file path like ""list1.txt"".

                --list2 <words>         Comma-separated list of words for the second list (for ww command).
                                        Example: ""apple,banana,orange"" or provide a text file path like ""list2.txt"".

                --words <words>         Comma-separated list of words (for wp and wd commands).
                                        Example: ""apple,banana,orange"" or provide a text file path like ""words.txt"".
                                        **Note**: Enclose the list in quotation marks if it contains spaces or special characters.

                --pdf-folder <path>     Path to the folder containing PDF files (for wp command).
                                        Example: ""C:/Documents/PDFs"" or use the default folder: ""Datasets/PDFs"".

                --dataset <path>        Path to the dataset CSV file (for wd command).
                                        Example: ""imdb_1000.csv"" or provide a custom path.

                --output <path>         Path to save the output CSV file.
                                        Example: ""results.csv"".

                --rows <number>         Number of rows to process from the dataset (for wd command).
                                        Example: 100 (default is 20 rows).

                --inputDir <path>       Directory containing the dataset CSV file (for wd command).
                                        Example: ""C:/Datasets/CSVs"" or use the default folder: ""Datasets/CSVs"".

                --outputDir <path>      Directory to save the output CSV file.
                                        Example: ""C:/Outputs/CSVs"" or use the default folder: ""Outputs/CSVs"".
            ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nDefault values:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($@"
                --pdf-folder            {defaultPdfDir}
                --inputDir              {defaultInputCsvDir}
                --outputDir             {defaultOutputCsvDir}
                --dataset               {defaultDataset}
            ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nExamples WITHOUT options:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(@"
                dotnet run --command ww --list1 apple,banana,orange --list2 grape,mango,pineapple
                dotnet run --command wp --words apple,banana,orange
                dotnet run --command wd --words apple,banana,orange
            ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nExamples WITH options:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(@"
                dotnet run --command ww --list1 ""apple,banana,orange"" --list2 ""grape,mango,pineapple"" --output results.csv
                dotnet run --command wp --words ""apple,banana,orange"" --pdf-folder ""C:/Documents/PDFs"" --output results.csv
                dotnet run --command wd --words ""apple,banana,orange"" --dataset imdb_1000.csv --output results.csv --rows 100
            ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nImportant Notes:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(@"
                1. For lists of words, you can either:
                    - Provide a comma-separated list (e.g., ""apple,banana,orange"").
                    - Provide a text file path containing the words (e.g., ""words.txt"").

                2. Enclose lists in quotation marks if they contain spaces or special characters.
                    Example: ""Business and Finance, Information Technology, Legal and Environmental"".

                3. If a required argument is missing, the program will prompt you to enter it.

                4. The program will automatically use default values for optional arguments if they are not provided.

                5. Ensure that the paths provided for files and directories are correct and accessible.
            ");

            // Reset color to default
            Console.ResetColor();
        }

        /// <summary>
        /// Method to prompt the user for a csv file to compare words with a dataset
        /// </summary>
        /// <param name="prompt">Prompt text</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>String of path</returns>
        private string PromptForFile(string prompt, string defaultValue)
        {
            string fileName;
            while (true)
            {
                Console.Write($"{prompt} (default: {defaultValue}): ");

                fileName = Console.ReadLine();

                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = defaultValue;
                }
                if (File.Exists(Path.Combine(defaultInputCsvDir, fileName)))
                {
                    return fileName;
                }
                Console.WriteLine("File does not exist. Please enter a valid path.");
            }
        }

        /// <summary>
        /// Method to prompt the user for a directory
        /// </summary>
        /// <param name="prompt">Prompt text</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>String of path</returns>
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

        /// <summary>
        /// Method to prompt the user for a single input 
        /// </summary>
        /// <param name="prompt">Input prompt e.g. "Enter a number: "</param>
        /// <returns>String of input</returns>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Method to prompt the user for a comma-separated list
        /// </summary>
        /// <param name="prompt">Input prompt e.g. "Enter a comma-separated list: "</param>
        /// <returns>List of words from the user input</returns>
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

        /// <summary>
        /// When USER INTERACTION IS REQUIRED TO ENTER WORDS
        /// Prompts the user for either a comma-separated list of words or a file path containing words.
        /// </summary>
        /// <param name="prompt">Prompt text e.g "Enter the CSV dataset file name:"</param>
        /// <returns>A list of words extracted from the input or file.</returns>
        private List<string> PromptForWordsOrFile(string prompt)
        {
            var textHelper = new TextHelper();

            Console.Write($"{prompt} ");

            string input = Console.ReadLine()!;

            // If input is a file path, read the words from the file
            if (textHelper.IsTextFilePath(input))
            {
                Console.WriteLine($"Reading words from file: {input}");
                try
                {
                    return textHelper.ExtractWordsFromTextFile(input, defaultTxtDir);
                }
                catch (FileNotFoundException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                    return PromptForWordsOrFile("Enter the list of words (comma-separated) or provide a valid text file path:");
                }
            }
            else
            {

                // Treat input as a comma-separated string
                return input.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(word => word.Trim())
                            .ToList();
            }
        }

        /// <summary>
        /// When PROCESSING COMMAND LINE INPUT
        /// Check if the input is not empty and process it as either a file path containing words or a comma-separated list of words.
        /// </summary>
        /// <param name="input">The input string from configuration.</param>
        /// <returns>A list of words extracted from the input.</returns>
        private List<string> ProcessInput(string input, string prompt)
        {
            var textHelper = new TextHelper();

            // If input is missing, or empty, prompt for it
            if (string.IsNullOrWhiteSpace(input))
                return PromptForWordsOrFile(prompt);

            // If input is a file path, extract words from the file
            if (textHelper.IsTextFilePath(input))
            {
                Console.WriteLine($"Reading words from file: {input}");
                try
                {
                    return textHelper.ExtractWordsFromTextFile(input, defaultTxtDir);
                }
                catch (FileNotFoundException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                    return new List<string>();
                }
            }

            // If input is not a file, treat it as a comma-separated list of words
            return input.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(word => word.Trim())
                        .ToList();
        }
    }
}
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.