namespace SemanticSimilarityAnalysis.Proj.Models;

public class Embedding(string text, List<double> vector)
{
    /// <summary>
    /// Represents a text embedding, containing the original input text, its corresponding numerical vector, and a unique identifier.
    /// This class is used to encapsulate the data required for semantic similarity computations.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier for the embedding.
    public string Text { get; set; } = text; // The input text for which the embedding is generated.
    public List<double> Vector { get; set; } = vector; // The numerical embedding vector.

}