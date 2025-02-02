using SemanticSimilarityAnalysis.Proj.Models;
using System.Text.Json;

namespace SemanticSimilarityAnalysis.Proj.Utils.Csv
{
    internal class CsvProcessor
    {
        public class CsvSaver
        {
            public static async Task SaveEmbeddingsToJsonAsync(List<MovieModel> movies, string jsonFilePath)
            {
                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                var jsonData = JsonSerializer.Serialize(movies, jsonOptions);
                await File.WriteAllTextAsync(jsonFilePath, jsonData);
            }
        }
    }
}
