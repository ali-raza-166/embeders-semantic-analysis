namespace SemanticSimilarityAnalysis.Proj.Services
{
    /// <summary>
    /// PineconeSetup is responsible for setting up the Pinecone index and inserting embeddings.
    /// It acts as a high-level orchestrator for initializing an index, generating embeddings, 
    /// and upserting them into Pinecone.
    /// </summary>
    public class PineconeSetup
    {
        private readonly PineconeService _pineconeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PineconeSetup"/> class.
        /// </summary>
        /// <param name="pineconeService">The Pinecone service used for managing embeddings.</param>
        public PineconeSetup(PineconeService pineconeService)
        {
            _pineconeService = pineconeService;
        }

        /// <summary>
        /// Runs the setup process by initializing the Pinecone index, generating embeddings 
        /// from the provided dataset, and upserting them into the specified index and namespace.
        /// </summary>
        /// <param name="dataset">A list of text samples to be embedded and stored.</param>
        /// <param name="indexName">The name of the Pinecone index.</param>
        /// <param name="namespaceName">The namespace within the Pinecone index.</param>
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