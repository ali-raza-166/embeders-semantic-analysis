using SemanticSimilarityAnalysis.Proj.Models;
using System.Text.Json;

namespace SemanticSimilarityAnalysis.Proj.Helpers.Json
{
    /// <summary>
    /// Helper class for JSON operations
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// Method to read and deserialize JSON from a file for STARTING generating embeddings
        /// </summary>
        /// <param name="filePath">Path to the JSON file</param>
        /// <returns>List of MultiEmbeddingRecord for generating embeddings</returns>
        /// <exception cref="InvalidOperationException"></exception>
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

        /// <summary>
        /// Ensure the directory exists and save the JSON to PREPARE for generating embeddings
        /// </summary>
        /// <param name="records">List of MultiEmbeddingRecord saved to JSON and prepared for generating embeddings</param>
        /// <param name="jsonFilePath">Path to the JSON file</param>
        /// <returns></returns>
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
