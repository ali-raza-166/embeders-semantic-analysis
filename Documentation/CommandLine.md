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

<br/>

## Correct Directory

Before running the application, it is crucial to ensure that you are in the correct directory `SemanticSimilarityAnalysis.Proj`. This can be done by navigating to the project directory using the following command:

```bash
cd ./SemanticSimilarityAnalysis.Proj/
```

<br/>

## Usage
### General Syntax

```
dotnet run --command <command> [options]
```

Replace <command> with one of the supported commands (ww, wp, wd) and provide the necessary options.

### Commands and Examples
**1. Words vs. Words (`ww`)**

Compare two lists of words.

- Syntax:

```
dotnet run --command ww --list1 <words> --list2 <words> [--output <path>] [--outputDir <path>]
```

<br/>

- Options:

Refer to the [Options](#options) table for details on --list1, --list2, --output, and --outputDir.

<br/>

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

<br/>

- Options:

Refer to the [Options](#options) table for details on --words, --pdf-folder, --output, and --outputDir.

<br/>

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

<br/>

- Options:

Refer to the [Options](#options) table for details on --words, --dataset, --output, --rows, --inputDir, and --outputDir.

<br/>

- Example:

```
dotnet run --command wd --words "apple,banana,orange" --dataset imdb_1000.csv --output results.csv --rows 100
```

<br/>

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
