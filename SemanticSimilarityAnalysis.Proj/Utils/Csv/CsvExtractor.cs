using CsvHelper;
using CsvHelper.Configuration;
using SemanticSimilarityAnalysis.Proj.Interfaces;
using SemanticSimilarityAnalysis.Proj.Models;
using System.Globalization;

public class CsvExtractor
{
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
