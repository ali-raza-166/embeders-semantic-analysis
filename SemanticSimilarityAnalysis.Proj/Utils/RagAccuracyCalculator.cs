using SemanticSimilarityAnalysis.Proj.Utils;
using SemanticSimilarityAnalysis.Proj.Services;

public class RagEvaluationResult
{
    public double CosineSimilarity { get; set; }
    public double Rouge1Score { get; set; }
    public double Rouge2Score { get; set; }
}

public class RagAccuracyCalculator
{
    private readonly CosineSimilarity _cosineSimilarityCalculator;
    private readonly OpenAiEmbeddingService _embeddingService;

    public RagAccuracyCalculator(CosineSimilarity cosineSimilarityCalculator, OpenAiEmbeddingService embeddingService)
    {
        _cosineSimilarityCalculator = cosineSimilarityCalculator;
        _embeddingService = embeddingService;
    }

    public async Task<double> ComputeCosineSimilarityAsync(string predicted, string reference)
    {
        // Convert text to embeddings
        var predictedEmbedding = await _embeddingService.CreateEmbeddingsAsync(new List<string> { predicted });
        var referenceEmbedding = await _embeddingService.CreateEmbeddingsAsync(new List<string> { reference });

        // Compute cosine similarity between the embeddings
        return _cosineSimilarityCalculator.ComputeCosineSimilarity(predictedEmbedding[0].Values, referenceEmbedding[0].Values);
    }
    public static double CalculateRougeN(string reference, string generated, int n)
    {
        // Tokenize the reference and generated text into n-grams
        var referenceNGrams = GetNGrams(reference, n);
        var generatedNGrams = GetNGrams(generated, n);
        // Ensure there are no empty n-grams lists
        if (referenceNGrams.Count == 0 || generatedNGrams.Count == 0)
        {
            return 0.0; // Return 0 if either list is empty
        }

        // Calculate the number of overlapping n-grams (precision)
        int overlapCount = referenceNGrams.Intersect(generatedNGrams).Count();
        // If no overlap, return 0
        if (overlapCount == 0)
        {
            return 0.0;
        }

        // Recall: Overlap divided by the total n-grams in the reference
        double recall = (double)overlapCount / referenceNGrams.Count;

        // Precision: Overlap divided by the total n-grams in the generated text
        double precision = (double)overlapCount / generatedNGrams.Count;

        // F1 Score: Harmonic mean of precision and recall
        double f1Score = 2 * (precision * recall) / (precision + recall);

        // Return the F1 Score (you can return recall or precision separately if needed)
        return f1Score;
    }

    // Helper method to extract n-grams from a string
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

    // Method to calculate ROUGE-1 (unigrams) and ROUGE-2 (bigrams)
    public static (double rouge1, double rouge2) CalculateRougeScores(string reference, string generated)
    {
        // Calculate ROUGE-1 (unigrams)
        double rouge1Score = CalculateRougeN(reference, generated, 1);

        // Calculate ROUGE-2 (bigrams)
        double rouge2Score = CalculateRougeN(reference, generated, 2);

        return (rouge1Score, rouge2Score);
    }
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


