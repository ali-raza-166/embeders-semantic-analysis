using Pinecone;
using SemanticSimilarityAnalysis.Proj.Interfaces;

namespace SemanticSimilarityAnalysis.Proj.Models
{
    public class MultiEmbeddingRecord
    {
        public Dictionary<string, string> Attributes { get; private set; }
        public Dictionary<string, List<IVectorData>> Vectors { get; private set; }

        public MultiEmbeddingRecord(Dictionary<string, string> attributes, Dictionary<string, List<IVectorData>>? vectors)
        {
            Attributes = attributes;
            Vectors = vectors ?? new Dictionary<string, List<IVectorData>>();
        }

        public void AddEmbedding(string field, IVectorData embedding)
        {
            if (!Vectors.ContainsKey(field))
            {
                Vectors[field] = new List<IVectorData>();
            }
            Vectors[field].Add(embedding);
        }
    }

    public class VectorData : IVectorData
    {
        public string Id { get; private set; }
        public Dictionary<string, MetadataValue?> Metadata { get; private set; }
        public List<float> Values { get; private set; }

        public VectorData(List<float> values, string? id = null, Dictionary<string, MetadataValue?>? metadata = null)
        {
            Values = values;
            Id = id ?? Guid.NewGuid().ToString();
            Metadata = metadata ?? new Dictionary<string, MetadataValue?>();
        }

        IEnumerable<KeyValuePair<string, MetadataValue?>> IVectorData.Metadata => Metadata;
    }

}
