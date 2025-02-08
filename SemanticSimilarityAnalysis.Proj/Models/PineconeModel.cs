using Pinecone;
using SemanticSimilarityAnalysis.Proj.Interfaces;

namespace SemanticSimilarityAnalysis.Proj.Models
{
    public class PineconeModel(string id, List<float> values, Dictionary<string, object> metadata)
        : IVectorData
    {
        public string Id { get; set; } = id;
        public List<float> Values { get; set; } = values;
        public Dictionary<string, object> Metadata { get; set; } = metadata;
        public List<float> Vector => Values;

        // NOTE: Avoid implementation error
        IEnumerable<KeyValuePair<string, MetadataValue?>> IVectorData.Metadata => throw new NotImplementedException();
    }
}