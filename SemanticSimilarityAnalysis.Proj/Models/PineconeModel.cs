using Pinecone;
using SemanticSimilarityAnalysis.Proj.Interfaces;

namespace SemanticSimilarityAnalysis.Proj.Models
{
    public class PineconeModel(string id,  List<float> values, Dictionary<string, object?> metadata, float score=0f)
        : IVectorData
    {
        public string Id { get; set; } = id;
        public List<float> Values { get; set; } = values;
        private Dictionary<string, object?> RawMetadata { get; set; } = metadata;
        public float Score { get; set; } = score;
        
        // Implement the Metadata property from IVectorData interface
        public IEnumerable<KeyValuePair<string, MetadataValue?>> Metadata =>
            RawMetadata.Select(kvp => new KeyValuePair<string, MetadataValue?>(
                kvp.Key,
                kvp.Value switch
                {
                    string s => new MetadataValue(s),
                    double d => new MetadataValue(d),
                    bool b => new MetadataValue(b),
                    _ => null // Handle unsupported types
                }
            ));
    }
}