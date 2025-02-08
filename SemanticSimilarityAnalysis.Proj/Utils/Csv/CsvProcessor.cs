using SemanticSimilarityAnalysis.Proj.Models; // Importing the MovieModel class
using SemanticSimilarityAnalysis.Proj.Services; // Importing the OpenAiEmbeddingService class
using System.Text.Json; // Importing JSON serialization utilities

namespace SemanticSimilarityAnalysis.Proj.Utils
{
    // Class responsible for processing movie data and generating semantic embeddings.
    public class CsvProcessor
    {
        private readonly OpenAiEmbeddingService _embeddingService;
        private readonly string _jsonFilePath;

        public CsvProcessor(OpenAiEmbeddingService embeddingService, string jsonFilePath)
        {
            _embeddingService = embeddingService ?? throw new ArgumentNullException(nameof(embeddingService));
            _jsonFilePath = jsonFilePath ?? throw new ArgumentNullException(nameof(jsonFilePath));
        }
        // Method responsible for processing the movie list and generating embeddings for titles and overviews asynchronously.
        public async Task ProcessAndGenerateEmbeddingsAsync(List<MultiEmbeddingRecord> records)
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

            var jsonData = JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true });

            await SaveJsonToFile(jsonData);
        }

        // Ensure the directory exists and save the JSON
        private async Task SaveJsonToFile(string jsonData)
        {
            var outputDirectory = Path.GetDirectoryName(_jsonFilePath);
            // Check if the directory exists; if not, create it to ensure the file can be saved.
            if (!string.IsNullOrWhiteSpace(outputDirectory) && !Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            await File.WriteAllTextAsync(_jsonFilePath, jsonData);
        }
    }
}
