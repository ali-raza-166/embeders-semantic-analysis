using CsvHelper;
using CsvHelper.Configuration;
using MathNet.Numerics.LinearAlgebra;
using SemanticSimilarityAnalysis.Proj.Models;
using System.Globalization;

namespace SemanticSimilarityAnalysis.Proj.Helpers.Csv
{
    /// <summary>
    /// Helper class for working with CSV files
    /// </summary>
    public class CSVHelper
    {
        /// Default number of rows to process when generating embeddings from the dataset
        private readonly int defaultProcessedRows = 20;


        /// <summary>
        /// Reads the header row of the CSV file using CsvHelper
        /// </summary>
        /// <param name="csvFilePath">Path to the CSV file</param>
        /// <returns>Returns a List<string> containing the header fields.</returns>
        public List<string> ReadCsvFields(string csvFilePath)
        {
            try
            {
                using (var reader = new StreamReader(csvFilePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    csv.Read();
                    csv.ReadHeader();
                    var fields = csv.HeaderRecord;
                    if (fields == null)
                    {
                        throw new Exception("CSV file does not have a header.");
                    }
                    return fields.ToList();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("An error occurred while reading the CSV file header.");
                return [];
            }
        }


        /// <summary>
        /// Extract records from a CSV file
        /// </summary>
        /// <param name="fields">Fields to extract for labelling and generating embeddings</param>
        /// <param name="csvFilePath">CSV file path</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public List<MultiEmbeddingRecord> ExtractRecordsFromCsv(
            List<string> fields,
            string csvFilePath
            )
        {
            var records = new List<MultiEmbeddingRecord>();

            try
            {
                using var reader = new StreamReader(csvFilePath);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true
                });

                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    var attributes = new Dictionary<string, string>();

                    // Extract text fields dynamically
                    foreach (var field in fields)
                    {
                        var value = csv.GetField(field);
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            attributes[field] = value;
                        }
                    }

                    if (attributes.Count > 0)
                    {
                        records.Add(new MultiEmbeddingRecord(attributes, new Dictionary<string, List<VectorData>>()));
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"CSV file not found: {csvFilePath}", ex);
            }
            catch (CsvHelper.MissingFieldException ex)
            {
                Console.WriteLine($"Missing field in CSV: {ex.Message}");
                throw; // Rethrow the exception
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }

            return records;
        }


        /// <summary>
        /// Export the similarity results to a CSV file
        /// </summary>
        /// <param name="similarityResults">List of similarity plot points</param>
        /// <param name="csvFileName">Name of the CSV file you want to create</param>
        /// <param name="outputDir">Path to the directory where you want to save the CSV file</param>
        public void ExportToCsv(List<SimilarityPlotPoint> similarityResults, string csvFileName, string outputDir = @"../../../Outputs/CSVs")
        {

            var csvFilePath = Path.Combine(outputDir, csvFileName);

            // Ensure the output directory exists
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Check if the file exists
            if (!File.Exists(csvFilePath))
            {
                Console.WriteLine($"File '{csvFilePath}' does not exist. Creating new file.");
                // The file will be created when you use StreamWriter below.
            }
            else
            {
                Console.WriteLine($"File '{csvFilePath}' already exists. Overwriting...");
            }

            using (var writer = new StreamWriter(csvFilePath))
            using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // Write header
                csv.WriteField(""); // Empty field for the label column
                foreach (var inputKey in similarityResults.First().Similarities.Keys)
                {
                    csv.WriteField(inputKey);
                }
                csv.NextRecord();

                // Write data rows
                foreach (var point in similarityResults)
                {
                    csv.WriteField(point.Label);
                    foreach (var similarity in point.Similarities.Values)
                    {
                        csv.WriteField(similarity);
                    }
                    csv.NextRecord();
                }
            }
        }


        // public void ExportReducedDimensionalityData(Matrix<double> reducedData, List<string> inputs, string filePath)
        // {
        //     // Convert Matrix<double> to double[,] to retain compatibility with the existing logic
        //     var numRows = reducedData.RowCount;
        //     var numCols = reducedData.ColumnCount;
        //     var reducedDataArray = new double[numRows, numCols];
        //
        //     // Copy data from the Matrix to the 2D array
        //     for (int i = 0; i < numRows; i++)
        //     {
        //         for (int j = 0; j < numCols; j++)
        //         {
        //             reducedDataArray[i, j] = reducedData[i, j];
        //         }
        //     }
        //
        //     // Ensure the output directory exists
        //     if (!Directory.Exists(filePath))
        //     {
        //         Directory.CreateDirectory(filePath);
        //     }
        //     
        //
        //     using var writer = new StreamWriter(filePath);
        //     using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        //
        //     // Write headers (Dim1, Dim2, Dim3, ..., DimN)
        //     csv.WriteField("String");
        //     for (var i = 0; i < numCols; i++)
        //     {
        //         csv.WriteField($"Dim{i + 1}");
        //     }
        //     csv.NextRecord();
        //
        //     // Write rows
        //     try
        //     {
        //         for (var i = 0; i < numRows; i++)
        //         {
        //             csv.WriteField(inputs[i]);  // Write the word first
        //
        //             for (var j = 0; j < numCols; j++)
        //             {
        //                 csv.WriteField(reducedDataArray[i, j]);
        //             }
        //             csv.NextRecord();
        //         }
        //         Console.WriteLine($"PCA Data exported here: {filePath}");
        //
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //         throw;
        //     }
        // }

        /// <summary>
        /// Export the reduced dimensionality data to a CSV file
        /// </summary>
        /// <param name="reducedData"></param>
        /// <param name="inputs"></param>
        /// <param name="csvFileName"></param>
        public void ExportReducedDimensionalityData(Matrix<double> reducedData, List<string> inputs, string csvFileName)
        {
            // Convert Matrix<double> to double[,] to retain compatibility with the existing logic
            var numRows = reducedData.RowCount;
            var numCols = reducedData.ColumnCount;
            var reducedDataArray = new double[numRows, numCols];

            // Copy data from the Matrix to the 2D array
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    reducedDataArray[i, j] = reducedData[i, j];
                }
            }

            var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"../../.."));
            string absoluteCsvDir = Path.Combine(projectRoot, "Outputs", "CSV");

            // Ensure the output directory exists
            if (!Directory.Exists(absoluteCsvDir))
            {
                Console.WriteLine($"Directory does not exist: {absoluteCsvDir}");
                Directory.CreateDirectory(absoluteCsvDir);
            }

            // Combine the directory and file name to get the full file path
            var csvFilePath = Path.Combine(absoluteCsvDir, csvFileName);
            try
            {
                using var writer = new StreamWriter(csvFilePath);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                // Write headers (Dim1, Dim2, Dim3, ..., DimN)
                csv.WriteField("String");
                for (var i = 0; i < numCols; i++)
                {
                    csv.WriteField($"Dim{i + 1}");
                }
                csv.NextRecord();

                // Write rows
                for (var i = 0; i < numRows; i++)
                {
                    csv.WriteField(inputs[i]);  // Write the word first

                    for (var j = 0; j < numCols; j++)
                    {
                        csv.WriteField(reducedDataArray[i, j]);
                    }
                    csv.NextRecord();
                }

                Console.WriteLine($"PCA Data exported here: {csvFilePath}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred while writing the file: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Determines the number of rows to process based on the user input and the number of rows in the CSV file.
        /// </summary>
        /// <param name="allRecords">All records extracted from the CSV file.</param>
        /// <param name="processedRows">Number of rows requested by the user (default is -1, which means use "defaultProcessedRows").</param>
        /// <returns>The number of rows to process.</returns>
        public int DetermineRowsToProcess(List<MultiEmbeddingRecord> allRecords, int processedRows)
        {
            // Determine if processedRows is the default value or user-provided
            bool isDefaultProcessedRows = (processedRows == -1); // Sentinel value check

            // Set the default number of rows to 20 if no user input is provided
            if (isDefaultProcessedRows)
            {
                processedRows = defaultProcessedRows; // Default value
                Console.WriteLine("Using default number of rows: 20.");
            }
            else
            {
                Console.WriteLine($"User requested number of rows: {processedRows}.\n");
            }

            // Check if the CSV file has fewer rows than the requested processedRows
            if (allRecords.Count < processedRows)
            {
                // If the user provided a value for processedRows (not the default), throw an error
                if (!isDefaultProcessedRows)
                {
                    throw new ArgumentOutOfRangeException(nameof(processedRows),
                        $"Requested {processedRows} rows, but the CSV file contains only {allRecords.Count} rows.");
                }
                else
                {
                    // If processedRows is the default value, proceed with all available rows
                    Console.WriteLine($"CSV file has fewer than the default ({processedRows}) rows ({allRecords.Count} rows). Proceeding with all available rows.");
                }
            }

            // Determine the number of rows to process
            return Math.Min(processedRows, allRecords.Count); // Use the smaller of the two values
        }


        /// <summary>
        /// Exports all phrase similarity data to a single CSV file, with each file's data separated by 3 empty rows.
        /// </summary>
        /// <param name="results">The dictionary containing similarity results for each file.</param>
        /// <param name="csvFileName">The name of the CSV file to export the results.</param>
        /// <param name="outputDir">The directory to save the CSV file. Default is "../../../Outputs/CSVs".</param>
        public void ExportAllPhrasesToCsv(
            Dictionary<string, List<SimilarityPlotPoint>> results,
            string csvFileName = "phrases.csv",
            string outputDir = @"../../../Outputs/CSVs")
        {
            // Ensure the output directory exists
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Construct the full file path
            var csvFilePath = Path.Combine(outputDir, csvFileName);

            // Write to CSV
            using (var writer = new StreamWriter(csvFilePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // Write header
                csv.WriteField("File Name");
                csv.WriteField("Phrase 1");
                csv.WriteField("Phrase 2");
                csv.WriteField("Similarity");
                csv.NextRecord();

                // Write data for each file
                foreach (var fileResult in results)
                {
                    var fileName = fileResult.Key;
                    var similarityResults = fileResult.Value;

                    // Write file name as a header
                    csv.WriteField($"File: {fileName}");
                    csv.WriteField("");
                    csv.WriteField("");
                    csv.WriteField("");
                    csv.NextRecord();

                    // Write similarity data for the file
                    foreach (var plotPoint in similarityResults)
                    {
                        string phrase1 = plotPoint.Label;

                        foreach (var pair in plotPoint.Similarities)
                        {
                            string phrase2 = pair.Key;
                            double similarity = pair.Value;

                            csv.WriteField("");
                            csv.WriteField(phrase1);
                            csv.WriteField(phrase2);
                            csv.WriteField(similarity);
                            csv.NextRecord();
                        }
                    }

                    // Add 3 empty rows between files
                    for (int i = 0; i < 3; i++)
                    {
                        csv.WriteField("");
                        csv.WriteField("");
                        csv.WriteField("");
                        csv.WriteField("");
                        csv.NextRecord();
                    }
                }
            }

            Console.WriteLine($"All phrase similarities exported to '{csvFilePath}' successfully.");
        }
    }
}