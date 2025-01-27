namespace SemanticSimilarityAnalysis.Proj.Models
{
    public class Embedding(int index, string text, List<float> vector)
    {
        public int Index { get; set; } = index;
        public string Text { get; set; } = text;
        public List<float> Vector { get; set; } = vector;
    }
}