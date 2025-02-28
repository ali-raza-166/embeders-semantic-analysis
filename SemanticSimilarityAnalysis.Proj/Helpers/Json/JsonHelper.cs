using SemanticSimilarityAnalysis.Proj.Models;
using System.Text.Json;

namespace SemanticSimilarityAnalysis.Proj.Helpers.Json
{
    public class JsonHelper
    {
        // Method to read and deserialize JSON from a file 
        public List<MultiEmbeddingRecord> GetRecordFromJson(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                return JsonSerializer.Deserialize<List<MultiEmbeddingRecord>>(stream)
                    ?? throw new InvalidOperationException("Failed to load embeddings.");
            }
        }

        // Ensure the directory exists and save the JSON
        public async Task SaveRecordToJson(List<MultiEmbeddingRecord> records, string jsonFilePath)
        {
            var outputDirectory = Path.GetDirectoryName(jsonFilePath);

            if (!string.IsNullOrWhiteSpace(outputDirectory) && !Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var jsonData = JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(jsonFilePath, jsonData);

        }
    }

}
