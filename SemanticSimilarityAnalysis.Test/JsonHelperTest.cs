using SemanticSimilarityAnalysis.Proj.Models;
using System.Text.Json;

namespace SemanticSimilarityAnalysis.Proj.Helpers.Json
{
    [TestClass]
    public class JsonHelper
    {
        // Method to read and deserialize JSON from a file 
        [TestMethod]
        public List<MultiEmbeddingRecord> GetRecordFromJson(string filePath)
        {
            try
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return JsonSerializer.Deserialize<List<MultiEmbeddingRecord>>(stream)
                        ?? throw new InvalidOperationException("Failed to load embeddings.");
                }
            }
            catch (FileNotFoundException ex)
            {
                throw new InvalidOperationException("File not found.", ex);
            }

        }
        // Ensure the directory exists and save the JSON
        [TestMethod]
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