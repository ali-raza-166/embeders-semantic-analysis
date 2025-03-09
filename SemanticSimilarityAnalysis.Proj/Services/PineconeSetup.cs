namespace SemanticSimilarityAnalysis.Proj.Services
{
    public class PineconeSetup
    {
        private readonly PineconeService _pineconeService;

        public PineconeSetup(PineconeService pineconeService)
        {
            _pineconeService = pineconeService;
        }

        public async Task RunAsync(List<string> dataset, string indexName, string namespaceName)
        {
            if (dataset == null || dataset.Count == 0)
            {
                throw new ArgumentException("Dataset cannot be empty.");
            }

            await _pineconeService.InitializeIndexAsync(indexName);
            var pineconeModels = await _pineconeService.GenerateEmbeddingsAsync(dataset);
            await _pineconeService.UpsertEmbeddingAsync(pineconeModels, indexName, namespaceName);

            Console.WriteLine("Pinecone setup completed!");
            
        }
    }
}