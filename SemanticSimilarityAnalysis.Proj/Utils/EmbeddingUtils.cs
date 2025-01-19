using SemanticSimilarityAnalysis.Proj.Model;

namespace SemanticSimilarityAnalysis.Proj.Utils
{
    internal class EmbeddingUtils
    {
        // Calculates the average embedding vector from a list of Embedding objects
        // Returns a single vector (List<float>) that represents the entire document's content
        public static List<float> GetAverageEmbedding(List<Embedding> embeddings)
        {
            if (embeddings == null || embeddings.Count == 0)
            {
                throw new ArgumentException("The embeddings list cannot be null or empty.");
            }

            int numOfEmbeddings = embeddings.Count;

            // Initialize the average vector with zeros
            int vectorLength = embeddings[0].EmbeddingVector.Count;
            var avgEmbedding = new List<float>(new float[vectorLength]);

            // Sum up all embedding vectors
            foreach (var embedding in embeddings)
            {
                for (int i = 0; i < vectorLength; i++)
                {
                    avgEmbedding[i] += embedding.EmbeddingVector[i];
                }
            }

            // Divide each element by the number of embeddings to compute the average
            for (int i = 0; i < vectorLength; i++)
            {
                avgEmbedding[i] /= numOfEmbeddings;
            }

            return avgEmbedding;
        }
    }
}
