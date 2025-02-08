using SemanticSimilarityAnalysis.Proj.Interfaces;

namespace SemanticSimilarityAnalysis.Proj.Models
{
    public class MultiEmbeddingRecord : IVectorData
    {
        public string Id { get; private set; }

        // Not implemented for MultiEmbeddingRecord
        public List<float> Vector => throw new NotImplementedException();

        public Dictionary<string, object> Metadata { get; private set; }
        public Dictionary<string, List<float>> Vectors { get; private set; }
        public Dictionary<string, string> Attributes { get; private set; }

        public MultiEmbeddingRecord(
            string id,
            Dictionary<string, string>? attributes = null,
            Dictionary<string, object>? metadata = null)
        {
            Id = id;
            Attributes = attributes ?? new Dictionary<string, string>();
            Metadata = metadata ?? new Dictionary<string, object>();
            Vectors = new Dictionary<string, List<float>>(); // Always initialized
        }
    }
}
