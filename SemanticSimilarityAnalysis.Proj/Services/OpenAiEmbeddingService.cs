using OpenAI.Embeddings;
using SemanticSimilarityAnalysis.Proj.Model;

namespace SemanticSimilarityAnalysis.Proj.Services
{
    public class OpenAiEmbeddingService
    {
        private readonly string _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                                          ?? throw new ArgumentNullException(nameof(_apiKey), "API key not found in environment variables.");
        public async Task<List<Embedding>> CreateEmbeddingsAsync(List<string> inputs)
        {
            var embeddingClient = new EmbeddingClient("text-embedding-ada-002", _apiKey);
            try
            {
                OpenAIEmbeddingCollection collection = await embeddingClient.GenerateEmbeddingsAsync(inputs);
                var embeddingsList = new List<Embedding>();
                foreach (OpenAIEmbedding embedding in collection)
                {
                    Console.WriteLine($"Found embedding: {embedding}");
                    Console.WriteLine(embedding.Index);
                    //Console.WriteLine(embedding.ToString()); 

                    ReadOnlyMemory<float> vector = embedding.ToFloats();
                    var vectorList = vector.Span.ToArray().ToList();

                    Console.WriteLine($"Embedding vector (first 10 values): {string.Join(", ", vectorList.Take(10))}");
                    Console.WriteLine($"Embedding vector: {vectorList}\n");

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