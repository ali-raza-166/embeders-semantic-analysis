# Working with Command Line to analyze Semantic Similarity

## Setting Up

To enable command-line execution in your application, ensure that you include the following lines of code in your Program.cs or equivalent entry point file:

```csharp
    var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory()) 
    .AddCommandLine(args)
    .Build();

    var commandLineHelper = serviceProvider.GetRequiredService<CommandLineHelper>();
    await commandLineHelper.ExecuteCommandAsync(configuration);
```

## Correct Directory

Before running the application, it is crucial to ensure that you are in the correct directory `SemanticSimilarityAnalysis.Proj`. This can be done by navigating to the project directory using the following command:

```bash
cd ./SemanticSimilarityAnalysis.Proj/
```

## Usage
### General Syntax

```
dotnet run --command <command> [options]
```

Replace <command> with one of the supported commands (ww, wp, wd) and provide the necessary options.

---

### Commands and Examples
**1. Words vs. Words (`ww`)**

Compare two lists of words.

- Syntax:

```
dotnet run --command ww --list1 <words> --list2 <words> [--output <path>] [--outputDir <path>]
```

- Options:

Refer to the [Options](#options) table for details on --list1, --list2, --output, and --outputDir.

- Example:

```
dotnet run --command ww --list1 apple,banana,orange,dog,cat --list2 fruit,animal --output results.csv
```

<br/>

**2. Words vs. PDFs (`wp`)**

Compare a list of words with text extracted from PDF documents.

- Syntax:

```
dotnet run --command wp --words <words> [--pdf-folder <path>] [--output <path>] [--outputDir <path>]
```

- Options:

Refer to the [Options](#options) table for details on --words, --pdf-folder, --output, and --outputDir.

- Example:

```
dotnet run --command wp --words "apple,banana,orange,dog,cat" --pdf-folder "C:/Documents/PDFs" --output results.csv
```

<br/>

**3. Words vs. Dataset (`wd`)**

Compare a list of words with a dataset (e.g., a CSV file).

- Syntax:

```
dotnet run --command wd --words <words> [--dataset <path>] [--output <path>] [--rows <number>] [--inputDir <path>] [--outputDir <path>]
```

- Options:

Refer to the [Options](#options) table for details on --words, --dataset, --output, --rows, --inputDir, and --outputDir.

- Steps After Entering the Command

    1. **Provide the List of Words**:
       - If you didn’t provide the `--words` option, the tool will prompt you to enter a list of words or a text file path.
       - Example:
         ```
         Please provide the list of words or a text file path: apple,banana,orange
         ```
 
    2. **Select Fields for Embeddings**:
       - The tool will display the available fields (columns) from the dataset CSV file.
       - You will be prompted to enter the fields you want to use for generating embeddings. Separate multiple fields with commas and the input.
      
       - Example:
         ```
         Available fields (title, description, genre):
         Enter the fields to be extracted from the dataset (comma-separated, RIGHT CASE as in the available fields): title,description
         ```
    
    3. **Choose a Label Field**:
       - You will be prompted to select a field to use as the label for saving and plotting the results.
       - Example:
         ```
         Enter 1 field to use as the label: title
         ```
    
    4. **Choose a Field for Comparison**:
       - You will be prompted to select a field whose embeddings will be compared with the words' embeddings.
       - Example:
         ```
         Enter the field you want to compare with the words: description
         ```
            
    ***Note**: Field names are case-sensitive. Ensure you enter them exactly as they appear in the dataset
  
    5. **Processing**:
       - The tool will generate embeddings for the selected fields and compare them with the words' embeddings.
       - The results will be saved to the specified output CSV file.

- Example:

```
dotnet run --command wd --words "apple,banana,orange" --dataset imdb_1000.csv --output results.csv --rows 100
```

### Options

Below is a list of all available options for the commands. These options can be used with any of the commands (`ww`, `wp`, `wd`), unless otherwise specified.

| **Option**         | **Description**                                                                 | **Default Value**       | **Applicable Commands** |
|---------------------|---------------------------------------------------------------------------------|-------------------------|--------------------------|
| `--list1`           | Comma-separated list of words for the first list.                               | None (required)         | `ww`                    |
| `--list2`           | Comma-separated list of words for the second list.                              | None (required)         | `ww`                    |
| `--words`           | Comma-separated list of words.                                                  | None (required)         | `wp`, `wd`              |
| `--pdf-folder`      | Path to the folder containing PDF files.                                         | `"Datasets/PDFs"`       | `wp`                    |
| `--dataset`         | Name of the dataset CSV file.                                                   | `"imdb_1000.csv"`       | `wd`                    |
| `--output`          | Name of the output CSV file.                                                    | Command-specific default| All commands            |
| `--rows`            | Number of rows to process from the dataset.                                     | Process all rows        | `wd`                    |
| `--inputDir`        | Directory containing the dataset CSV file.                                      | `"Datasets/CSVs"`       | `wd`                    |
| `--outputDir`       | Directory to save the output CSV file.                                          | `"Outputs/CSVs"`        | All commands            |

### Default Values

If you don't provide certain options, the tool will use the following default values:

- PDF Folder: "Datasets/PDFs"

- Input Directory: "Datasets/CSVs"

- Output Directory: "Outputs/CSVs"

- Dataset: "imdb_1000.csv"


## Help
To see the full list of commands and options, you can either:

**1. Run the Tool**

- Open your terminal or command prompt, navigate to the project directory, and run the following command:
```
dotnet run
```
- This will display the help menu with a list of available commands and options.

**2. Start the Program in Visual Studio**

- Open the project in Visual Studio.
 
- Set the project as the startup project (if it isn't already).

- Press F5 or click the Start button to run the program.

- The help menu will be displayed in the console window, showing the list of available commands and options.

## Important Notes

For lists of words, you can either:

- Provide a comma-separated list (e.g., "apple,banana,orange").

- Provide a text file path containing the words (e.g., "words.txt").

- Enclose lists in quotation marks if they contain spaces or special characters.
    Example: "Business and Finance, Information Technology, Legal and Environmental".

- If a required argument is missing, the program will prompt you to enter it.

- The program will automatically use default values for optional arguments if they are not provided.

- Ensure that the paths provided for files and directories are correct and accessible.

- The output directory (default: "Outputs/CSVs") will be created automatically if it does not exist. Results will be saved in this directory as CSV files.
