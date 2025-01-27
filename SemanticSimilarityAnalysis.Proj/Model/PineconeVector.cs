namespace SemanticSimilarityAnalysis.Proj.Model;

public class PineconeVector(string id, List<float> values, Dictionary<string, string> metadata)
{
    public string Id { get; set; } = id;
    public List<float> Values { get; set; } = values;
    public Dictionary<string, string> Metadata { get; set; } = metadata;
}