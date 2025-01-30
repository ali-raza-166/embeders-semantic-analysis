using SemanticSimilarityAnalysis.Proj.Interfaces;

namespace SemanticSimilarityAnalysis.Proj.Models
{
    public class Embedding(int index, string text, List<float> vector) : IVectorData
    {
        private int Index { get; set; } = index;
        private string Text { get; set; } = text;
        public List<float> Vector { get; set; } = vector;

        public string Id => Index.ToString();
        public Dictionary<string, object> Metadata => new Dictionary<string, object>
        {
            { "Text", Text }
        };
    }
}