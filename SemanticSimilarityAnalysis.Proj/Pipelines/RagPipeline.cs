using SemanticSimilarityAnalysis.Proj.Services;
using SemanticSimilarityAnalysis.Proj.Utils;

namespace SemanticSimilarityAnalysis.Proj.Pipelines
{
    /// <summary>
    /// Implements the Retrieval-Augmented Generation (RAG) pipeline.
    /// This class integrates Pinecone for retrieval, OpenAI for text generation, 
    /// and accuracy calculation utilities to evaluate generated responses.
    /// </summary>
    public class RagPipeline
    {
        private readonly PineconeSetup _pineconeSetupService;
        private readonly PineconeService _pineconeService;
        private readonly OpenAiTextGenerationService _textGenerationService;
        private readonly RagAccuracyCalculator _accuracyCalculator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RagPipeline"/> class.
        /// </summary>
        /// <param name="pineconeSetupService">Service for setting up Pinecone indexes and embeddings.</param>
        /// <param name="pineconeService">Service for interacting with Pinecone.</param>
        /// <param name="textGenerationService">Service for generating text responses using OpenAI.</param>
        /// <param name="accuracyCalculator">Service for evaluating the accuracy of generated responses.</param>
        public RagPipeline(
            PineconeSetup pineconeSetupService, 
            PineconeService pineconeService,
            OpenAiTextGenerationService textGenerationService,
            RagAccuracyCalculator accuracyCalculator)
        {
            _pineconeSetupService = pineconeSetupService;
            _pineconeService = pineconeService;
            _textGenerationService = textGenerationService;
            _accuracyCalculator = accuracyCalculator;
        }
        
        /// <summary>
        /// Initializes the RAG pipeline by setting up the Pinecone index and uploading embeddings.
        /// </summary>
        /// <param name="inputs">List of input texts to generate embeddings for.</param>
        /// <param name="indexName">The Pinecone index name.</param>
        /// <param name="namespaceName">The namespace within the Pinecone index.</param>
        public async Task InitializeAndUploadEmbeddingsAsync(List<string> inputs, string indexName, string namespaceName)
        {
            Console.WriteLine("Running RAG Pipeline Initialization...");
            
            Console.WriteLine("Setting up Pinecone index...");
            await _pineconeSetupService.RunAsync(inputs, indexName, namespaceName);
        }

        /// <summary>
        /// Retrieves relevant documents from Pinecone and generates a response using OpenAI.
        /// </summary>
        /// <param name="query">The query text to retrieve related documents for.</param>
        /// <param name="indexName">The Pinecone index name.</param>
        /// <param name="namespaceName">The namespace within the Pinecone index.</param>
        /// <param name="topK">The number of top matching documents to retrieve.</param>
        /// <returns>The generated response based on retrieved documents.</returns>
        public async Task<string> RetrieveAndGenerateResponseAsync(string query, string indexName, string namespaceName, uint topK)
        {
            Console.WriteLine($"Querying Pinecone for: {query}");
            
            var pineconeTopKparagraphs = await _pineconeService.QueryEmbeddingsAsync(query, indexName, namespaceName, topK);
            var answer = await _textGenerationService.GenerateTextAsync(query, pineconeTopKparagraphs);

            Console.WriteLine($"Generated Answer: {answer}\n");

            return answer;
        }
        
        /// <summary>
        /// Processes multiple queries by retrieving relevant documents and generating responses.
        /// Used for batch processing and accuracy evaluation.
        /// </summary>
        /// <param name="inputs">A list of query texts.</param>
        /// <param name="indexName">The Pinecone index name.</param>
        /// <param name="namespaceName">The namespace within the Pinecone index.</param>
        /// <param name="topK">The number of top matching documents to retrieve per query.</param>
        /// <returns>A list of generated responses for each query.</returns>
        public async Task<List<string>> BatchRetrieveAndGenerateResponsesAsync(List<string> inputs, string indexName, string namespaceName, uint topK)
        {
            List<string> generatedResponses = new();
            for (int i = 0; i < inputs.Count; i++)
            {
                string query = inputs[i];
                var answer = await RetrieveAndGenerateResponseAsync(query, indexName, namespaceName, topK);
                generatedResponses.Add(answer);
            }

            return generatedResponses;
        }

        /// <summary>
        /// Evaluates the accuracy of generated responses by comparing them with ground truth answers.
        /// </summary>
        /// <param name="generatedResponses">The generated responses to evaluate.</param>
        /// <param name="groundTruthAnswers">The correct answers to compare against.</param>
        /// <returns>A <see cref="RagEvaluationResult"/> object containing accuracy metrics.</returns>
        public async Task<RagEvaluationResult> EvaluateAccuracy(string generatedResponses, string groundTruthAnswers)
        {
            Console.WriteLine("Calculating Accuracy...");
            var accuracyResults = await _accuracyCalculator.EvaluateAsync(generatedResponses, groundTruthAnswers);

            return accuracyResults;
        }
    }
}
