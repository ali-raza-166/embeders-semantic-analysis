using Pinecone;

namespace SemanticSimilarityAnalysis.Proj.Interfaces;

public interface IVectorData
{
    string Id { get; }
    List<float> Values { get; }
    IEnumerable<KeyValuePair<string, MetadataValue?>> Metadata { get; }
}

