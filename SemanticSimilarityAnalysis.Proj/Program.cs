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
            { "Cricket", "Cristiano Ronaldo, widely regarded as one of the greatest footballers of all time, is a Portuguese professional player known for his exceptional skill, athleticism, and dedication to the sport. Born on February 5, 1985, in Madeira, Portugal, Ronaldo began his illustrious career at Sporting CP before rising to global stardom with Manchester United, where he won his first Ballon d'Or in 2008. His transfer to Real Madrid in 2009 marked a historic era, as he became the club's all-time top scorer, winning multiple Champions League titles and four additional Ballon d'Or awards."
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
