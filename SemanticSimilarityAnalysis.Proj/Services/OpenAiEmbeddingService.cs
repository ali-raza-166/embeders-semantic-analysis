using OpenAI.Embeddings;
using SemanticSimilarityAnalysis.Proj.Models;

namespace SemanticSimilarityAnalysis.Proj.Services
{
    /// <summary>
    /// Service for generating text embeddings using OpenAI's embedding API.
    /// </summary>
    /// <remarks>
    /// This service interacts with OpenAI's embedding model to transform text inputs into vector representations.
    /// These embeddings are useful for semantic similarity analysis, and retrieval-augmented generation (RAG).
    /// </remarks>
    public class OpenAiEmbeddingService
    {
        private readonly EmbeddingClient _embeddingClient;
        
        /// <summary>
        /// Initializes the OpenAI embedding service with a provided embedding client.
        /// </summary>
        /// <param name="embeddingClient">An instance of OpenAI's embedding client.</param>
        /// <exception cref="ArgumentNullException">Thrown if the embedding client is null.</exception>
        public OpenAiEmbeddingService(EmbeddingClient embeddingClient)
        {
            _embeddingClient = embeddingClient ?? throw new ArgumentNullException(nameof(embeddingClient));
        }

        /// <summary>
        /// Generates embeddings for a list of text inputs using OpenAI's embedding API.
        /// </summary>
        /// <param name="inputs">A list of text strings to convert into embeddings.</param>
        /// <returns>A list of <see cref="Embedding"/> objects containing the vector representations of the input texts.</returns>
        /// <exception cref="InvalidOperationException">Thrown if embedding generation fails.</exception>
        /// <remarks>
        /// This method sends text inputs to OpenAI's embedding model and returns a list of embeddings, 
        /// each containing index, and a vector representation.
        /// </remarks>
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