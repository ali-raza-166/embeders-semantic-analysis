using Moq;
using OpenAI.Embeddings;
using SemanticSimilarityAnalysis.Proj.Services;

namespace SemanticSimilarityAnalysis.Test
{
    [TestClass]
    public class OpenAiEmbeddingServiceTests
    {
        private Mock<EmbeddingClient> _mockEmbeddingClient;
        private OpenAiEmbeddingService _embeddingService;

        [TestInitialize]
        public void SetUp()
        {
            // Set up the mock for EmbeddingClient
            _mockEmbeddingClient = new Mock<EmbeddingClient>("text-embedding-ada-002", "test-api-key");

            // Set up the service with the mocked EmbeddingClient
            _embeddingService = new OpenAiEmbeddingService(_mockEmbeddingClient.Object);
        }

       
    }
}
