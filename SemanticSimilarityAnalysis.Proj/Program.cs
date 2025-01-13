using SemanticSimilarityAnalysis.Proj.Services;
using SemanticSimilarityAnalysis.Proj.Utils;
namespace SemanticSimilarityAnalysis.Proj
{
    internal abstract class Program
    {
        private static async Task Main(string[] args)
        {
            var embeddingService = new OpenAiEmbeddingService();
            var similarityCalculator = new CosineSimilarity();
            
            var inputs = new List<string>
            {
                "Cat",
                "Kitten"
            };
            
        try
        {
            var embeddings = await embeddingService.CreateEmbeddingsAsync(inputs);
            if (embeddings.Count >= 2)
            {
                var vectorA = embeddings[0].EmbeddingVector; 
                var vectorB = embeddings[1].EmbeddingVector; 
                
                var cosineSimilarity = similarityCalculator.ComputeCosineSimilarity(vectorA, vectorB);
                Console.WriteLine($"Cosine Similarity between '{inputs[0]}' and '{inputs[1]}': {cosineSimilarity}");
                
            }
            Console.WriteLine(embeddings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
