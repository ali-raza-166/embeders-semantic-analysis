using Pinecone;
using SemanticSimilarityAnalysis.Proj.Interfaces;

namespace SemanticSimilarityAnalysis.Proj.Models
{
    /// <summary>
    /// Represents a record that stores multiple embeddings (vector data) along with associated attributes.
    /// This class is designed to manage embeddings for different fields, where each field can have multiple vectors.
    /// </summary>
    public class MultiEmbeddingRecord
    {
        public Dictionary<string, string> Attributes { get; private set; }
        public Dictionary<string, List<VectorData>> Vectors { get; private set; }

        public MultiEmbeddingRecord(Dictionary<string, string> attributes, Dictionary<string, List<VectorData>>? vectors)
        {
            Attributes = attributes;
            Vectors = vectors ?? new Dictionary<string, List<VectorData>>();
        }

        // Add an embedding for a specific field after extracting the dataset
        public void AddEmbedding(string field, VectorData embedding)
        {
            if (!Vectors.ContainsKey(field))
            {
                Vectors[field] = new List<VectorData>();
            }
            Vectors[field].Add(embedding);
        }
    }

    /// <summary>
    /// Represents a single piece of vector data, including its values, metadata, and a unique identifier.
    /// This class implements the <see cref="IVectorData"/> interface to provide standardized access to vector data.
    /// </summary>
    public class VectorData : IVectorData
    {
        public string Id { get; private set; }
        public List<float> Values { get; private set; }
        public Dictionary<string, MetadataValue?>? Metadata { get; private set; }

        IEnumerable<KeyValuePair<string, MetadataValue?>> IVectorData.Metadata => throw new NotImplementedException();

        public VectorData(List<float> values, string? id = null, Dictionary<string, MetadataValue?>? metadata = null)
        {
            Values = values;
            Id = id ?? Guid.NewGuid().ToString();
            Metadata = metadata ?? new Dictionary<string, MetadataValue?>();
        }
    }
}
