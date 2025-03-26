
using Pinecone;
using SemanticSimilarityAnalysis.Proj.Interfaces;

namespace SemanticSimilarityAnalysis.Proj.Models
{
    public class Embedding(int index, string text, List<float> vector) : IVectorData
    {
        private int Index { get; set; } = index;
        private string Text { get; set; } = text;
        public List<float> Values { get; set; } = vector;

        public string Id => Index.ToString();
        public IEnumerable<KeyValuePair<string, MetadataValue?>> Metadata =>
            new Dictionary<string, MetadataValue?>
            {
                { "Text", new MetadataValue(Text) }
            };
    }
}