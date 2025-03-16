using SemanticSimilarityAnalysis.Proj.Models;
using System.Text.Json;

namespace SemanticSimilarityAnalysis.Proj.Helpers.Json
{
    [TestClass]
    public class JsonHelper
    {
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
    }

}