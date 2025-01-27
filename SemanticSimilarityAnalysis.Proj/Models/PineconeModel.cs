namespace SemanticSimilarityAnalysis.Proj.Models;

public class PineconeModel(string id, List<float> values, Dictionary<string, string> metadata)
{
    public string Id { get; set; } = id;
    public List<float> Vector { get; set; } = values;
    public Dictionary<string, string> Metadata { get; set; } = metadata;
}