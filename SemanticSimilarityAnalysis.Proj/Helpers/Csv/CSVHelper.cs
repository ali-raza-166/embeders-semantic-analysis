using CsvHelper;
using CsvHelper.Configuration;
using SemanticSimilarityAnalysis.Proj.Models;
using System.Globalization;

namespace SemanticSimilarityAnalysis.Proj.Helpers.Csv
{
    public class CSVHelper
    {
        /// <summary>
        /// Default number of rows to process when generating embeddings from the dataset
        /// </summary>
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
        /// <param name="fields">Fields/columns to extract</param>
        /// <param name="csvFilePath">Path to the CSV file</param>
        /// <returns>Return a List of MultiEmbeddingRecord objects containing the extracted data as attributes and empty embeddings at first.</returns>
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
    }
}
