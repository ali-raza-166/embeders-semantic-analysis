using Pinecone;
using SemanticSimilarityAnalysis.Proj.Models;

namespace SemanticSimilarityAnalysis.Proj.Services
{
    public class PineconeService
    {
        private readonly PineconeClient _pineconeClient;
        private readonly string _indexName;
        public PineconeService(string indexName)
        {
            var apiKey = Environment.GetEnvironmentVariable("PINECONE_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Pinecone API key is not set in environment variables.");
            }
            _indexName = "us-east-1";
            _pineconeClient = new PineconeClient(apiKey);
        }
        // public async Task UpsertEmbeddingAsync(List<PineconeModel> vectors, string namespaceName)
        // {
        //     var index = _pineconeClient.Index(host: _indexName);
        //
        //     var upsertResponse = await index.UpsertAsync(new UpsertRequest
        //     {
        //         Vectors = vectors.Select(v => new Vector
        //         {
        //             Id = v.Id,
        //             Values = v.Values.ToArray(),
        //             Metadata = v.Metadata
        //         }).ToArray(),
        //         Namespace = namespaceName
        //     });
        //     Console.WriteLine($"Upsert completed. Status: {upsertResponse.Status}");
        // }
    }
}
