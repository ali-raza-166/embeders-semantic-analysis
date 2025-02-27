using CsvHelper;
using CsvHelper.Configuration;
using SemanticSimilarityAnalysis.Proj.Interfaces;
using SemanticSimilarityAnalysis.Proj.Models;
using System.Globalization;

namespace SemanticSimilarityAnalysis.Proj.Helpers.Csv
{
    public class CSVHelper
    {
        // Extract records from a CSV file
        public List<MultiEmbeddingRecord> ExtractRecordsFromCsv(
            List<string> fields,
            string csvFilePath
            )
        {
            var records = new List<MultiEmbeddingRecord>();

            try
            {
                if (!File.Exists(csvFilePath))
                    throw new FileNotFoundException($"CSV file not found: {csvFilePath}");

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
                        records.Add(new MultiEmbeddingRecord(attributes, new Dictionary<string, List<IVectorData>>()));
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (CsvHelperException ex)
            {
                Console.WriteLine($"CSV parsing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            return records;
        }

    }
}
