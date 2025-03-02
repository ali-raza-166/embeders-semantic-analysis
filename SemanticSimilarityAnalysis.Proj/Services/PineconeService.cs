using Pinecone;
using SemanticSimilarityAnalysis.Proj.Models;

namespace SemanticSimilarityAnalysis.Proj.Services
{
    public class PineconeService
    {
        private readonly PineconeClient _pineconeClient;
        private const int Dimension = 1536;
        private const string IndexName = "example-index-ali";

        public PineconeService()
        {
            var apiKey = Environment.GetEnvironmentVariable("PINECONE_API_KEY");
            Console.WriteLine("PINECONE API KEY: " + apiKey);
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
            }

            if (indexes != null && indexes.Indexes != null && indexes.Indexes.Any(index => index?.Name == IndexName))
            {
                Console.WriteLine($"Index '{IndexName}' already exists.");
                return;
            }

            Console.WriteLine($"Creating Pinecone index: {IndexName}...");

            var request = new CreateIndexRequest
            {
                Name = IndexName,
                Dimension = Dimension,
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
            Console.WriteLine($"Index '{IndexName}' created successfully.");
        }

        public async Task UpsertEmbeddingAsync(List<PineconeModel> models, string namespaceName)
        {

            Console.WriteLine($"In pinecone service upsert method {namespaceName}...");
            Console.WriteLine($"Prinitng the recieved embeddings in upsert method call");
            try
            {
                Console.WriteLine($"Length of to be inserted embedding list: {models.Count()}...");
                foreach (var model in models)
                {
                    Console.WriteLine($"ID: {model.Id}");
                    foreach (var metadataItem in model.Metadata)
                    {
                        Console.WriteLine($"Key: {metadataItem.Key}, Value: {metadataItem.Value?.Value}");
                    }

                    Console.WriteLine(
                        $"Embedding vector (first 10 values) before upserting: {string.Join(", ", model.Values.Take(10))}");
                    Console.WriteLine();
                }

                var index = _pineconeClient.Index(IndexName);

                var upsertRequest = new UpsertRequest
                {
                    Vectors = models.Select(model => new Vector
                    {
                        Id = model.Id,
                        Values = model.Values.ToArray(),
                        Metadata = new Metadata(model.Metadata)
                    }).ToArray(),
                    Namespace = namespaceName
                };

                var upsertResponse = await index.UpsertAsync(upsertRequest);
                Console.WriteLine("Upsert completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during the upsert operation: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new InvalidOperationException("Failed to upsert embeddings.", ex);
            }
        }

        public IndexClient GetPineconeIndex()
        {
            return _pineconeClient.Index(IndexName);
        }

        public async Task<List<PineconeModel>> QueryEmbeddingsAsync(List<float> queryEmbedding, string namespaceName,
            uint topK, string languageFilter = null)
        {
            Console.WriteLine($"Language Filter in received pinecone service: {languageFilter}");
            var index = _pineconeClient.Index(IndexName);
          
            var queryRequest = new QueryRequest
            {
                Vector = queryEmbedding.ToArray(),
                TopK = topK,
                Namespace = namespaceName,
                IncludeValues = true,
                IncludeMetadata = true,
                Filter = new Metadata
                {
                    ["Language"] = new Metadata {  ["$eq"] = languageFilter }
                }
            };
            var queryResponse = await index.QueryAsync(queryRequest);

            if (queryResponse.Matches == null) return [];
            var retrievedVectors = new List<PineconeModel>();
            foreach (var match in queryResponse.Matches)
            {

                var score = match.Score ?? 0f;
                var values = match.Values.HasValue ? match.Values.Value.Span.ToArray().ToList() : [];
                var metadata = match.Metadata?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Value)
                               ?? new Dictionary<string, object?>();
                var language = metadata.ContainsKey("language") ? metadata["language"]?.ToString() : string.Empty;
                var text = metadata.ContainsKey("text") ? metadata["text"]?.ToString() : string.Empty;

                var pineconeModel = new PineconeModel(match.Id, values, metadata, score);
                retrievedVectors.Add(pineconeModel);
            }

            return retrievedVectors;
        }

    }
}
