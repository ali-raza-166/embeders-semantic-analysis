using SemanticSimilarityAnalysis.Proj.Interfaces;
namespace SemanticSimilarityAnalysis.Proj.Utils
{
    /// <summary>
    /// Provides utility methods for working with embeddings (vector data).
    /// </summary>
    public class EmbeddingUtils
    {
        public List<float> GetAverageEmbedding(List<IVectorData> embeddings)
        {
            if (embeddings == null || embeddings.Count == 0)
            {
                throw new ArgumentException("The embeddings list cannot be null or empty.");
            }
            var numOfEmbeddings = embeddings.Count;
            var vectorLength = embeddings[0].Values.Count;
            var avgEmbedding = new List<float>(new float[vectorLength]);

            foreach (var embedding in embeddings)
            {
                for (var i = 0; i < vectorLength; i++)
                {
                    avgEmbedding[i] += embedding.Values[i];
                }
            }

            for (var i = 0; i < vectorLength; i++)
            {
                avgEmbedding[i] /= numOfEmbeddings;
            }

            return avgEmbedding;
        }
    }
}