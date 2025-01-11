using System;

namespace SemanticSimilarityAnalysis.Proj.Services;

/// <summary>
/// Service for calculating similarity metrics between embeddings.
/// </summary>
public class SimilarityCalculator
{
    /// <summary>
    /// Calculates the cosine similarity between two embedding vectors.
    /// </summary>
    /// <param name="vector1">The first embedding vector.</param>
    /// <param name="vector2">The second embedding vector.</param>
    /// <returns>A double representing the similarity score.</returns>
    public double ComputeCosineSimilarity(List<double> vector1, List<double> vector2)
    {
        if (vector1.Count != vector2.Count)
            throw new ArgumentException("Vectors must have the same length.");

        double dotProduct = 0;
        double magnitude1 = 0;
        double magnitude2 = 0;

        for (var i = 0; i < vector1.Count; i++)
        {
            dotProduct += vector1[i] * vector2[i];
            magnitude1 += Math.Pow(vector1[i], 2);
            magnitude2 += Math.Pow(vector2[i], 2);
        }

        if (magnitude1 == 0 || magnitude2 == 0)
            throw new InvalidOperationException("One or both vectors have zero magnitude.");

        return dotProduct / (Math.Sqrt(magnitude1) * Math.Sqrt(magnitude2));
    }
}