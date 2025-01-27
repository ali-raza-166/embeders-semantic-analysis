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
            _indexName = indexName;
            _pineconeClient = new PineconeClient(apiKey);
        }
    }
}
