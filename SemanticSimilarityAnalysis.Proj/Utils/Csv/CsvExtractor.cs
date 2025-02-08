using CsvHelper; // Library for reading and writing CSV files
using CsvHelper.Configuration; // Provides configuration settings for CsvHelper
using SemanticSimilarityAnalysis.Proj.Models; // Importing the MovieModel class
using System.Globalization; // Provides culture-specific formatting for reading CSV files

public class CsvExtractor
{
    // Method to extract movie details from a CSV file
    public List<MultiEmbeddingRecord> ExtractRecordsFromCsv(string csvFilePath, List<string> fields)
    {
        var records = new List<MultiEmbeddingRecord>();

        using var reader = new StreamReader(csvFilePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        });

        csv.Read();
        csv.ReadHeader();
        // Iterate through each row in the CSV file
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
        return records;
    }
}
