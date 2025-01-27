namespace SemanticSimilarityAnalysis.Proj.Interfaces;

public interface IVectorData
{
    string Id { get; }
    List<float> Vector { get; }
    Dictionary<string, object> Metadata { get; }
}
