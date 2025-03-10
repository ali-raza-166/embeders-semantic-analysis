using OpenAI.Embeddings;
using SemanticSimilarityAnalysis.Proj.Models;

namespace SemanticSimilarityAnalysis.Proj.Services
{
    public class OpenAiEmbeddingService
    {
        private readonly EmbeddingClient _embeddingClient;

        public OpenAiEmbeddingService(EmbeddingClient embeddingClient) 
        {
            _embeddingClient = embeddingClient ?? throw new ArgumentNullException(nameof(embeddingClient));
        }

        public async Task<List<Embedding>> CreateEmbeddingsAsync(List<string> inputs)
        {
            try
            {
                OpenAIEmbeddingCollection collection = await _embeddingClient.GenerateEmbeddingsAsync(inputs);
                var embeddingsList = new List<Embedding>();
                foreach (OpenAIEmbedding embedding in collection)
                {
                    ReadOnlyMemory<float> vector = embedding.ToFloats();
                    var vectorList = vector.Span.ToArray().ToList();

                    Console.WriteLine($"Embedding vector index: {embedding.Index}");
                    Console.WriteLine($"Embedding vector (first 10 values): {string.Join(", ", vectorList.Take(10))}");
                    var text = inputs[embedding.Index];

                    var newEmbedding = new Embedding(embedding.Index, text, vectorList);
                    embeddingsList.Add(newEmbedding);
                }

                return embeddingsList;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to generate embeddings", ex);
            }
        }
    }

}