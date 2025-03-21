using SemanticSimilarityAnalysis.Proj.Utils;
using SemanticSimilarityAnalysis.Proj.Services;

/// <summary>
/// Represents the evaluation results for a Retrieval-Augmented Generation (RAG) system, 
/// including cosine similarity and ROUGE scores.
/// </summary>
public class RagEvaluationResult
{
    public double CosineSimilarity { get; set; }
    public double Rouge1Score { get; set; }
    public double Rouge2Score { get; set; }
}

/// <summary>
/// Computes accuracy metrics for a RAG (Retrieval-Augmented Generation) system.
/// Uses cosine similarity and ROUGE scores for evaluation.
/// </summary>
public class RagAccuracyCalculator
{
    private readonly CosineSimilarity _cosineSimilarityCalculator;
    private readonly OpenAiEmbeddingService _embeddingService;

    /// <summary>
    /// Initializes the RAG accuracy calculator with cosine similarity and embedding services.
    /// </summary>
    public RagAccuracyCalculator(CosineSimilarity cosineSimilarityCalculator, OpenAiEmbeddingService embeddingService)
    {
        _cosineSimilarityCalculator = cosineSimilarityCalculator;
        _embeddingService = embeddingService;
    }

    /// <summary>
    /// Computes the cosine similarity between the generated and reference text using embeddings.
    /// </summary>
    /// <param name="predicted">The generated response.</param>
    /// <param name="reference">The ground truth/reference answer.</param>
    /// <returns>A task returning the cosine similarity score (0 to 1).</returns>
    public async Task<double> ComputeCosineSimilarityAsync(string predicted, string reference)
    {
        var predictedEmbedding = await _embeddingService.CreateEmbeddingsAsync(new List<string> { predicted });
        var referenceEmbedding = await _embeddingService.CreateEmbeddingsAsync(new List<string> { reference });

        return _cosineSimilarityCalculator.ComputeCosineSimilarity(predictedEmbedding[0].Values, referenceEmbedding[0].Values);
    }
    
    /// <summary>
    /// Calculates the ROUGE-N score, which measures n-gram overlap between reference and generated text.
    /// </summary>
    /// <param name="reference">The ground truth/reference answer.</param>
    /// <param name="generated">The generated response.</param>
    /// <param name="n">The n-gram size (e.g., 1 for ROUGE-1, 2 for ROUGE-2).</param>
    /// <returns>The ROUGE-N F1 score.</returns>
    public static double CalculateRougeN(string reference, string generated, int n)
    {

        var referenceNGrams = GetNGrams(reference, n);
        var generatedNGrams = GetNGrams(generated, n);
        
        if (referenceNGrams.Count == 0 || generatedNGrams.Count == 0)
        {
            return 0.0;
        }

        int overlapCount = referenceNGrams.Intersect(generatedNGrams).Count();
        if (overlapCount == 0)
        {
            return 0.0;
        }
        
        double recall = (double)overlapCount / referenceNGrams.Count;
        double precision = (double)overlapCount / generatedNGrams.Count;
        double f1Score = 2 * (precision * recall) / (precision + recall);
        
        return f1Score;
    }

    /// <summary>
    /// Extracts n-grams from a given text for ROUGE calculations.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <param name="n">The n-gram size.</param>
    /// <returns>A list of n-grams.</returns>
    private static List<string> GetNGrams(string text, int n)
    {
        var words = text.Split(new[] { ' ', '.', ',', ';', ':', '!', '?', '-', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var nGrams = new List<string>();

        for (int i = 0; i < words.Length - n + 1; i++)
        {
            // Create the n-gram from consecutive words
            var nGram = string.Join(" ", words.Skip(i).Take(n));
            nGrams.Add(nGram);
        }

        return nGrams;
    }

    /// <summary>
    /// Computes both ROUGE-1 (unigram) and ROUGE-2 (bigram) scores.
    /// </summary>
    /// <param name="reference">The ground truth/reference answer.</param>
    /// <param name="generated">The generated response.</param>
    /// <returns>A tuple with ROUGE-1 and ROUGE-2 scores.</returns>
    public static (double rouge1, double rouge2) CalculateRougeScores(string reference, string generated)
    {
        double rouge1Score = CalculateRougeN(reference, generated, 1);
        
        double rouge2Score = CalculateRougeN(reference, generated, 2);

        return (rouge1Score, rouge2Score);
    }
    
    /// <summary>
    /// Evaluates the generated response against the reference text using cosine similarity and ROUGE scores.
    /// </summary>
    /// <param name="generatedResponse">The generated response.</param>
    /// <param name="groundTruthAnswer">The ground truth/reference answer.</param>
    /// <returns>A task returning evaluation results with cosine similarity, ROUGE-1, and ROUGE-2 scores.</returns>
    public async Task<RagEvaluationResult> EvaluateAsync(string generatedResponse, string groundTruthAnswer)
    {
        double similarity = await ComputeCosineSimilarityAsync(generatedResponse, groundTruthAnswer);
        var (rouge1, rouge2) = CalculateRougeScores(generatedResponse, groundTruthAnswer);
        return new RagEvaluationResult
        {
            CosineSimilarity = similarity,
            Rouge1Score = rouge1,
            Rouge2Score = rouge2
        };
    }
}


