using SemanticSimilarityAnalysis.Proj.Model;

namespace SemanticSimilarityAnalysis.Proj.Utils
{
    internal class EmbeddingUtils
    {
        public static List<float> GetAverageEmbedding(List<Embedding> embeddings)
        {
            if (embeddings == null || embeddings.Count == 0)
            {
                throw new ArgumentException("The embeddings list cannot be null or empty.");
            }

            int numOfEmbeddings = embeddings.Count;
            int vectorLength = embeddings[0].EmbeddingVector.Count;
            var avgEmbedding = new List<float>(new float[vectorLength]);
            
            foreach (var embedding in embeddings)
            {
                for (int i = 0; i < vectorLength; i++)
                {
                    avgEmbedding[i] += embedding.EmbeddingVector[i];
                }
            }

            for (int i = 0; i < vectorLength; i++)
            {
                avgEmbedding[i] /= numOfEmbeddings;
            }

            return avgEmbedding;
        }
    }
}
