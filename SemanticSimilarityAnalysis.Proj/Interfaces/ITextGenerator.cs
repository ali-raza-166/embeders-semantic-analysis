namespace SemanticSimilarityAnalysis.Proj.Interfaces
{
    public interface ITextGenerator
    {
        Task<string> GenerateTextAsync(string prompt, List<string> contextParagraphs);
    }
}   