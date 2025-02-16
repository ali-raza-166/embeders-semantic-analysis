using SemanticSimilarityAnalysis.Proj.Helpers.Csv;
using SemanticSimilarityAnalysis.Proj.Helpers.Json;
using SemanticSimilarityAnalysis.Proj.Models;
using SemanticSimilarityAnalysis.Proj.Utils;

namespace SemanticSimilarityAnalysis.Proj.Services
{
    public class EmbeddingAnalysisService
    {
        private readonly OpenAiEmbeddingService _embeddingService;
        private readonly CosineSimilarity _similarityCalculator;
        private readonly EuclideanDistance _euclideanDistCalc;
        private readonly EmbeddingUtils _embeddingUtils;
        private readonly CSVHelper _csvHelper;
        private readonly JsonHelper _jsonHelper;

        public EmbeddingAnalysisService(
             OpenAiEmbeddingService embeddingService,
             CosineSimilarity similarityCalculator,
             EuclideanDistance euclideanDistCalc,
             EmbeddingUtils embeddingUtils,
             CSVHelper csvHelper,
             JsonHelper jsonHelper)
        {
            _embeddingService = embeddingService;
            _similarityCalculator = similarityCalculator;
            _euclideanDistCalc = euclideanDistCalc;
            _embeddingUtils = embeddingUtils;
            _csvHelper = csvHelper;
            _jsonHelper = jsonHelper;
        }

        /// <summary>
        /// Compares embeddings for two input texts using cosine similarity and Euclidean distance.
        /// </summary>
        public async Task CompareTextEmbeddingsAsync(string text1, string text2)
        {
            var inputs = new List<string> { text1, text2 };
            var embeddings = await _embeddingService.CreateEmbeddingsAsync(inputs);

            if (embeddings.Count < 2)
                throw new InvalidOperationException("Insufficient embeddings generated.");

            var vectorA = embeddings[0].Values;
            var vectorB = embeddings[1].Values;

            double cosineSimilarity = _similarityCalculator.ComputeCosineSimilarity(vectorA, vectorB);
            double euclideanDistance = _euclideanDistCalc.ComputeEuclideanDistance(vectorA, vectorB);

            Console.WriteLine($"Cosine Similarity: {cosineSimilarity}");
            Console.WriteLine($"Euclidean Distance: {euclideanDistance}");
        }

        /// <summary>
        /// Compares document embeddings by averaging all embeddings in each document.
        /// </summary>
        public async Task CompareDocumentEmbeddingsAsync(List<string> doc1Texts, List<string> doc2Texts)
        {
            Console.WriteLine("Generating embeddings... Document 1");
            var embeddingsDoc1 = await _embeddingService.CreateEmbeddingsAsync(doc1Texts);

            Console.WriteLine("Generating embeddings... Document 2");
            var embeddingsDoc2 = await _embeddingService.CreateEmbeddingsAsync(doc2Texts);

            var vectorA = _embeddingUtils.GetAverageEmbedding(embeddingsDoc1);
            var vectorB = _embeddingUtils.GetAverageEmbedding(embeddingsDoc2);

            double cosineSimilarity = _similarityCalculator.ComputeCosineSimilarity(vectorA, vectorB);
            double euclideanDistance = _euclideanDistCalc.ComputeEuclideanDistance(vectorA, vectorB);

            Console.WriteLine($"Cosine Similarity: {cosineSimilarity}");
            Console.WriteLine($"Euclidean Distance: {euclideanDistance}");
        }

        /// <summary>
        /// Processes dataset embeddings from a CSV file and saves them as JSON.
        /// </summary>
        public async Task ProcessDataSetEmbeddingsAsync(
            List<string> fields,
            string csvFilePath = @"..\..\..\Datasets\imdb_1000.csv",
            string outputDirectory = @"..\..\..\Output",
            string outputFile = "embeddings.json")
        {
            var records = _csvHelper.ExtractRecordsFromCsv(csvFilePath, fields);
            var embeddings = await _csvHelper.GenerateTextEmbeddingsAsync(records);

            string jsonOutputPath = Path.Combine(outputDirectory, outputFile);
            await _jsonHelper.SaveRecordToJson(embeddings, jsonOutputPath);

            Console.WriteLine("Embeddings successfully generated and saved to JSON.");
        }

        /// <summary>
        /// Analyzes similarity between input text embeddings and dataset embeddings.
        /// </summary>
        public async Task<List<SimilarityPlotPoint>> AnalyzeEmbeddingsAsync(
            string jsonFilePath, string label, string attribute, List<string> inputStrings)
        {
            // Check if JSON file exists
            if (!File.Exists(jsonFilePath))
                throw new FileNotFoundException($"JSON file not found: {jsonFilePath}");

            // Load records from JSON
            var records = _jsonHelper.GetRecordFromJson(jsonFilePath);

            // Generate input embeddings from the input strings
            var inputEmbeddings = await _embeddingService.CreateEmbeddingsAsync(inputStrings);
            var inputVectors = inputEmbeddings.Select(e => e.Values).ToList();

            var similarityResults = new List<SimilarityPlotPoint>();

            foreach (var record in records)
            {
                if (record.Vectors.TryGetValue(attribute, out var vectorList) && vectorList.Count > 0)
                {
                    var attributeEmbedding = vectorList.First().Values;

                    var similarity1 = inputVectors.Count > 0 ? _similarityCalculator.ComputeCosineSimilarity(attributeEmbedding, inputVectors[0]) : 0f;
                    var similarity2 = inputVectors.Count > 1 ? _similarityCalculator.ComputeCosineSimilarity(attributeEmbedding, inputVectors[1]) : 0f;
                    var similarity3 = inputVectors.Count > 2 ? _similarityCalculator.ComputeCosineSimilarity(attributeEmbedding, inputVectors[2]) : 0f;

                    similarityResults.Add(new SimilarityPlotPoint(label, similarity1, similarity2, similarity3));
                }
            }

            return similarityResults.OrderByDescending(p => p.SimilarityWithInput1).ToList();
        }
    }
}
