using SemanticSimilarityAnalysis.Proj.Services;
using SemanticSimilarityAnalysis.Proj.Utils;

namespace SemanticSimilarityAnalysis.Proj.Pipelines
{
    public class RagPipeline
    {
        private readonly PineconeSetup _pineconeSetupService;
        private readonly PineconeService _pineconeService;
        private readonly OpenAiTextGenerationService _textGenerationService;
        private readonly RagAccuracyCalculator _accuracyCalculator;

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

        // Method for initializing the pipeline, indexing, and uploading embeddings
        public async Task InitializeAndUploadEmbeddingsAsync(List<string> inputs, string indexName, string namespaceName)
        {
            Console.WriteLine("Running RAG Pipeline Initialization...");
            
            Console.WriteLine("Setting up Pinecone index...");
            await _pineconeSetupService.RunAsync(inputs, indexName, namespaceName);
        }

        // Method for retrieving the query embedding and generating the response fot that query
        public async Task<string> RetrieveAndGenerateResponseAsync(string query, string indexName, string namespaceName, uint topK)
        {
            Console.WriteLine($"Querying Pinecone for: {query}");
            
            var pineconeTopKparagraphs = await _pineconeService.QueryEmbeddingsAsync(query, indexName, namespaceName, topK);
            var answer = await _textGenerationService.GenerateTextAsync(query, pineconeTopKparagraphs);

            Console.WriteLine($"Generated Answer: {answer}\n");

            return answer;
        }
        
        // Combined method for querying and generating answers for a list of queries. NOTE: for accuracy measurement
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
        
        // Method to calculate accuracy when comparing the generated answers to ground truth answers
        public async Task<RagEvaluationResult> EvaluateAccuracy(string generatedResponses, string groundTruthAnswers)
        {
            Console.WriteLine("Calculating Accuracy...");
            var accuracyResults = await _accuracyCalculator.EvaluateAsync(generatedResponses, groundTruthAnswers);

            return accuracyResults;
        }
    }
}
