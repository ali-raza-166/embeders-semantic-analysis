namespace SemanticSimilarityAnalysis.Proj.Model
{
    public class Embedding(int index, string text, List<float> embeddingVector)
    {
        public int Index { get; set; } = index;
        public string Text { get; set; } = text; 
        public List<float> EmbeddingVector { get; set; } = embeddingVector;
    }
}