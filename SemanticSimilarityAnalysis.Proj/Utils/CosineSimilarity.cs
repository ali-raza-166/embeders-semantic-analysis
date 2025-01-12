namespace SemanticSimilarityAnalysis.Proj.Utils;

public class CosineSimilarity
{
    public double ComputeCosineSimilarity(List<float> vectorA, List<float> vectorB)
    {
        if (vectorA == null || vectorB == null || vectorA.Count != vectorB.Count)
            throw new ArgumentException("Vectors must be non-null and of the same length.");

        var dotProduct = 0f;
        var magnitudeA = 0f;
        var magnitudeB = 0f;

        for (var i = 0; i < vectorA.Count; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += vectorA[i] * vectorA[i];
            magnitudeB += vectorB[i] * vectorB[i];
        }

        if (magnitudeA == 0f || magnitudeB == 0f)
            return 0f; //if any of the vectory is zero

        return dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
    }
}