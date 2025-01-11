using System.ClientModel;
using OpenAI.Embeddings;
using SemanticSimilarityAnalysis.Proj.Models;

namespace SemanticSimilarityAnalysis.Proj.Services
{
    public class OpenAiEmbeddingService
    {
        private readonly string? _apiKey;
        public OpenAiEmbeddingService()
        {
            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("API key is missing. Please set the OPENAI_API_KEY environment variable.");
            }
        }
        public async Task<List<float>> CreateEmbeddingsAsync(List<string> inputs)
        {
            EmbeddingClient embeddingClient = new("text-embedding-3-small", _apiKey);
            try
            {
                OpenAIEmbeddingCollection collection = await embeddingClient.GenerateEmbeddingsAsync(inputs);
                var embeddingValues = new List<float>();
                foreach (var embedding in collection)
                {
                    var vector = embedding.ToFloats();
                    embeddingValues.AddRange(vector.ToArray());
                    var embeddingObject = new Embedding(inputs[collection.IndexOf(embedding)], vector.ToList());
                    embeddings.Add(embeddingObject);
                    var vectorAsString = $"[{string.Join(", ", vector)}]";
                    Console.WriteLine($"Dimension: {vector.Length}");
                    Console.WriteLine($"Generated Embedding: {vectorAsString}");
                }
                return embeddingValues;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the embedding process
                throw new InvalidOperationException("Failed to generate embedding", ex);
            }
        }
    }
}