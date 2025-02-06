using CsvHelper;
using CsvHelper.Configuration;
using SemanticSimilarityAnalysis.Proj.Models;
using System.Globalization;

public class CsvExtractor
{
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

        while (csv.Read())
        {
            var title = csv.GetField("Title");
            var genre = csv.GetField("Genre");
            var overview = csv.GetField("Overview");

            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(genre) && !string.IsNullOrWhiteSpace(overview))
            {
                record.Add(new MultiEmbeddingRecord { Title = title, Genre = genre, Overview = overview });
            }
        }

        return record;
    }
}
