using SemanticSimilarityAnalysis.Proj.Models;
using System.Text.Json;

namespace SemanticSimilarityAnalysis.Proj.Helpers.Json
{
    public class JsonHelper
    {
        // Method to read and deserialize JSON from a file
        public MultiEmbeddingRecord GetRecordFromJson(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);

            // Deserialize the JSON string into a MultiEmbeddingRecord object
            var record = Newtonsoft.Json.JsonConvert.DeserializeObject<MultiEmbeddingRecord>(jsonString);

            if (record == null)
            {
                throw new InvalidOperationException("Failed to deserialize JSON into MultiEmbeddingRecord.");
            }

            return record;
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
