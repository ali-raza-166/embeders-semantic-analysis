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
                    Console.WriteLine($"Found embedding: {embedding}"); //Found embedding: OpenAI.Embeddings.OpenAIEmbedding
                    Console.WriteLine(embedding.Index); //0..
                    Console.WriteLine(embedding.ToString()); //OpenAI.Embeddings.OpenAIEmbedding
                    
                    ReadOnlyMemory<float> vector = embedding.ToFloats(); 
                    var vectorList = vector.Span.ToArray().ToList();
                    
                    Console.WriteLine($"Embedding vector (first 10 values): {string.Join(", ", vectorList.Take(10))}"); //Embedding vector (first 10 values): -0,016087161, -0,00031383053, 0,012905086, -0,02740121, -0,011613217, 0,012653512, 0,0060649826, -0,025905363, -0,0032381704, -0,028094739
                    Console.WriteLine($"Embedding vector: {vectorList}"); //Embedding vector: System.Collections.Generic.List`1[System.Single]
                    
                    var text = inputs[embedding.Index]; // Get the text from the original inputs
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