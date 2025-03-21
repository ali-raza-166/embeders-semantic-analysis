using Pinecone;

namespace SemanticSimilarityAnalysis.Proj.Interfaces;

/// <summary>
/// Interface for vector data
/// </summary>
public interface IVectorData
{
    string Id { get; }
    List<float> Values { get; }
    IEnumerable<KeyValuePair<string, MetadataValue?>> Metadata { get; }
}

