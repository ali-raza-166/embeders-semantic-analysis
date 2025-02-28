using SemanticSimilarityAnalysis.Proj.Interfaces;
using SemanticSimilarityAnalysis.Proj.Models;

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
    public List<KeyValuePair<string, double>> GetTopKCosineSimilarities(List<float> queryEmbedding, List<PineconeModel> models, int topK)
    {
        var results = new List<KeyValuePair<string, double>>();

        foreach (var model in models)
        {
            var similarity = ComputeCosineSimilarity(queryEmbedding, model.Values);
            results.Add(new KeyValuePair<string, double>(model.Id, similarity));
        }

        // Sort the results by similarity score in descending order and take the top K
        var topKResults = results
            .OrderByDescending(x => x.Value)
            .Take(topK)
            .ToList();

        return topKResults;
    }
}