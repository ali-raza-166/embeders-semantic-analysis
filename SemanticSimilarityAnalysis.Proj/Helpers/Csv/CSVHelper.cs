using CsvHelper;
using CsvHelper.Configuration;
using SemanticSimilarityAnalysis.Proj.Interfaces;
using SemanticSimilarityAnalysis.Proj.Models;
using SemanticSimilarityAnalysis.Proj.Services;
using System.Globalization;

namespace SemanticSimilarityAnalysis.Proj.Helpers.Csv
{
    public class CSVHelper
    {
        private readonly OpenAiEmbeddingService _embeddingService;

        public CSVHelper(OpenAiEmbeddingService embeddingService)
        {
            _embeddingService = embeddingService ?? throw new ArgumentNullException(nameof(embeddingService));
        }

        // Extract records from a CSV file
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

        // Generate embeddings for text attributes from a list of records
        public async Task<List<MultiEmbeddingRecord>> GenerateTextEmbeddingsAsync(List<MultiEmbeddingRecord> records)
        {
            if (records == null || records.Count == 0)
                throw new ArgumentException("Record list cannot be null or empty.", nameof(records));

            foreach (var record in records)
            {
                foreach (var attribute in record.Attributes)
                {
                    var attributeName = attribute.Key;
                    var attributeValue = attribute.Value;

                    // Ignore empty attibutes
                    if (string.IsNullOrWhiteSpace(attributeValue)) continue;

                    // Generate embedding for the attribute value
                    var attributeEmbedding = await _embeddingService.CreateEmbeddingsAsync(new List<string> { attributeValue });

                    // Add the embedding to the record for the respective attribute
                    record.AddEmbedding(attributeName, new VectorData(attributeEmbedding[0].Values));
                }
            }
            return records;
        }
    }
}
