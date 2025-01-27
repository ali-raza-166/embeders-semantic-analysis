namespace SemanticSimilarityAnalysis.Proj.Utils;

public class CosineSimilarity
{
    public double ComputeCosineSimilarity(List<float> vectorA, List<float> vectorB)
    {
        if (vectorA.Count != vectorB.Count)
        {
            return 0;
        }
        var dotProduct = 0.0;
        var magnitudeA = 0.0;
        var magnitudeB = 0.0;

        for (var i = 0; i < vectorA.Count; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += Math.Pow(vectorA[i], 2);
            magnitudeB += Math.Pow(vectorB[i], 2);
        }

        if (magnitudeA == 0.0 || magnitudeB == 0.0)
            throw new ArgumentException("Embeddings must not have zero magnitude.");

        return dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
    }
}