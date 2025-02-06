using Pinecone;
using SemanticSimilarityAnalysis.Proj.Models;

namespace SemanticSimilarityAnalysis.Proj.Services
{
    public class PineconeService
    {
        private readonly PineconeClient _pineconeClient;
        private readonly int _dimension = 1536;
        private readonly string _indexName="example-index-ali";
        public PineconeService()
        {
            var apiKey = Environment.GetEnvironmentVariable("PINECONE_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Pinecone API key is not set in environment variables.");
            } 
            _pineconeClient = new PineconeClient(apiKey);
        }
        public async Task InitializeIndexAsync()
        {
            var indexes = await _pineconeClient.ListIndexesAsync();
            var indexList = indexes.Indexes;
            if (indexes?.Indexes == null || !indexes.Indexes.Any())
            {
                Console.WriteLine("No indexes found.");
                return;
            }
            if (indexes.Indexes.Any(index => index?.Name == _indexName)) 
            {
                Console.WriteLine($"Index '{_indexName}' already exists.");
                return;
            }
            Console.WriteLine($"Creating Pinecone index: {_indexName}...");

            var request = new CreateIndexRequest
            {
                Name = _indexName,
                Dimension = _dimension,
                Metric = CreateIndexRequestMetric.Cosine,
                Spec = new ServerlessIndexSpec
                {
                    Serverless = new ServerlessSpec
                    {
                        Cloud = ServerlessSpecCloud.Aws,
                        Region = "us-east-1",
                    }
                },
                DeletionProtection = DeletionProtection.Disabled
            };

            await _pineconeClient.CreateIndexAsync(request);
            Console.WriteLine($"Index '{_indexName}' created successfully.");
        }
    }
    
    // public async Task UpsertEmbeddingAsync(List<PineconeModel> vectors, string namespaceName)
    // {
    //     var index = _pineconeClient.Index(_indexName);  // Accessing the index directly
    //
    //     var upsertRequest = new UpsertRequest
    //     {
    //         Vectors = vectors.Select(v => new Vector
    //         {
    //             Id = v.Id,
    //             Values = v.Values.ToArray(),
    //             Metadata = v.Metadata
    //         }).ToArray(),
    //         Namespace = namespaceName
    //     };
    //
    //     var upsertResponse = await index.UpsertAsync(upsertRequest);
    //     
    // }
}
