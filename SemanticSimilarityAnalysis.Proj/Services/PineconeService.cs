using Pinecone;
using SemanticSimilarityAnalysis.Proj.Models;
using LanguageDetection;
namespace SemanticSimilarityAnalysis.Proj.Services
{
    /// <summary>
    /// PineconeService provides methods to interact with Pinecone, a vector database.
    /// It handles index creation, embedding generation, upserting embeddings, and querying stored vectors.
    /// </summary>
    public class PineconeService
    {
        private readonly OpenAiEmbeddingService _embeddingService;
        private readonly PineconeClient _pineconeClient;
        private readonly LanguageDetector _detector;
        private const int Dimension = 1536;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PineconeService"/> class.
        /// </summary>
        /// <param name="embeddingService">Service for generating OpenAI embeddings.</param>
        /// <param name="pineconeClient">Pinecone client for interacting with the database.</param>
        /// <param name="detector">Language detection service.</param>
        public PineconeService(OpenAiEmbeddingService embeddingService,PineconeClient pineconeClient, LanguageDetector detector)
        {
            _pineconeClient = pineconeClient ?? throw new ArgumentNullException(nameof(pineconeClient));
            _embeddingService = embeddingService ?? throw new ArgumentNullException(nameof(embeddingService));
            _detector = detector ?? throw new ArgumentNullException(nameof(detector));
        }

        /// <summary>
        /// Initializes a Pinecone index with the given name.
        /// If the index already exists, it will not be created again.
        /// </summary>
        /// <param name="indexName">The name of the index to initialize.</param>
        public async Task InitializeIndexAsync(string indexName)
        {
            try
            {
                var indexes = await _pineconeClient.ListIndexesAsync();
                var indexList = indexes.Indexes;
                if (indexes?.Indexes == null || !indexes.Indexes.Any())
                {
                    Console.WriteLine("No indexes found.");
                }

                if (indexes != null && indexes.Indexes != null && indexes.Indexes.Any(index => index?.Name == indexName))
                {
                    Console.WriteLine($"Index '{indexName}' already exists.");
                    return;
                }

                Console.WriteLine($"Creating Pinecone index: {indexName}...");

                var request = new CreateIndexRequest
                {
                    Name = indexName,
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
                Console.WriteLine($"Index '{indexName}' created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // throw new InvalidOperationException($"Failed to Initialize index. {indexName} Error:", ex.Message);
            }
        }

        /// <summary>
        /// Generates embeddings for a given dataset using OpenAI's embedding service.
        /// Each embedding is stored with metadata including the text and detected language.
        /// </summary>
        /// <param name="dataset">List of text samples to generate embeddings for.</param>
        /// <returns>A list of PineconeModel objects containing embeddings and metadata.</returns>
        public async Task<List<PineconeModel>> GenerateEmbeddingsAsync(List<string> dataset)
        {
            Console.WriteLine("Generating embeddings for dataset...");
            var embeddings = await _embeddingService.CreateEmbeddingsAsync(dataset);
            var pineconeModels = embeddings.Select((embedding, index) => new PineconeModel(
                id: $"doc_{index}",
                values: embedding.Values.ToList(),
                metadata: new Dictionary<string, object?>
                {
                    { "Text", dataset[index] },
                    { "Language", _detector.Detect(dataset[index]) ?? "unknown" }
                }
            )).ToList();
            Console.WriteLine("✅ Embeddings generated successfully.");
            return pineconeModels;
        }

        /// <summary>
        /// Upserts (inserts or updates) a list of embeddings into the specified Pinecone index and namespace.
        /// </summary>
        /// <param name="models">The list of embeddings to upsert.</param>
        /// <param name="indexName">The Pinecone index name.</param>
        /// <param name="namespaceName">The namespace within the index.</param>
        public async Task UpsertEmbeddingAsync(List<PineconeModel> models, string indexName, string namespaceName)
        {
            Console.WriteLine($"In pinecone service upsert method {namespaceName}...");
            Console.WriteLine("Printing the received embeddings in upsert method call");
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
                    Console.WriteLine($"Embedding vector (first 10 values) before upserting: {string.Join(", ", model.Values.Take(10))}");
                    Console.WriteLine();
                }

                var index = _pineconeClient.Index(indexName);
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
        
        /// <summary>
        /// Queries the Pinecone index to find the top-k most similar embeddings to a given input query.
        /// An optional language filter can be applied.
        /// </summary>
        /// <param name="query">The input query text.</param>
        /// <param name="indexName">The Pinecone index to query.</param>
        /// <param name="namespaceName">The namespace within the index.</param>
        /// <param name="topK">The number of closest matches to retrieve.</param>
        /// <param name="languageFilter">Optional filter to limit results to a specific language.</param>
        /// <returns>A list of the most similar text results.</returns>
        public async Task<List<string>> QueryEmbeddingsAsync(string query, string indexName, string namespaceName, uint topK, string? languageFilter = null)
        {
            Console.WriteLine($"Creating embedding for query: {query}");
            var queryEmbeddings = await _embeddingService.CreateEmbeddingsAsync(new List<string> { query });
            Console.WriteLine($"Language Filter received in pinecone service: {languageFilter}");
            var index = _pineconeClient.Index(indexName);

            try
            {
                var queryRequest = new QueryRequest
                {
                    Vector = new ReadOnlyMemory<float>(queryEmbeddings[0].Values.ToArray()),
                    TopK = topK,
                    Namespace = namespaceName,
                    IncludeValues = true,
                    IncludeMetadata = true,
                };
                
                // Only add the filter if a language filter is explicitly provided
                if (!string.IsNullOrEmpty(languageFilter))
                {
                    queryRequest.Filter = new Metadata { ["Language"] = new Metadata { ["$eq"] = languageFilter } };
                }
                
                var queryResponse = await index.QueryAsync(queryRequest);
                if (queryResponse.Matches == null) return new List<string>();
                var pineconeTopKparagraphs = new List<string>();
                foreach (var match in queryResponse.Matches)
                {
                    if (match.Metadata != null && match.Metadata.TryGetValue("Text", out var textValue) && textValue?.Value is string text)
                    {
                        pineconeTopKparagraphs.Add(text);
                    }
                }
                return pineconeTopKparagraphs;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }
}
