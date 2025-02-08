using CsvHelper; // Library for reading and writing CSV files
using CsvHelper.Configuration; // Provides configuration settings for CsvHelper
using SemanticSimilarityAnalysis.Proj.Models; // Importing the MovieModel class
using System.Globalization; // Provides culture-specific formatting for reading CSV files

public class CsvExtractor
{
    // Method to extract movie details from a CSV file
    public List<MultiEmbeddingRecord> ExtractRecordsFromCsv(string csvFilePath, List<string> fields)
    {
        var record = new List<MultiEmbeddingRecord>();

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
            var title = csv.GetField("Title");
            var genre = csv.GetField("Genre");
            var overview = csv.GetField("Overview");
            // Ensure all required fields are non-empty before adding to the list
            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(genre) && !string.IsNullOrWhiteSpace(overview))
            {
                record.Add(new MultiEmbeddingRecord { Title = title, Genre = genre, Overview = overview });
            }
        }

        return record;
    }
}
