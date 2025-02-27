using SemanticSimilarityAnalysis.Proj.Helpers.Csv;
using SemanticSimilarityAnalysis.Proj.Helpers.Json;
using SemanticSimilarityAnalysis.Proj.Helpers.Pdf;
using SemanticSimilarityAnalysis.Proj.Models;
using SemanticSimilarityAnalysis.Proj.Utils;
using System.Data;

namespace SemanticSimilarityAnalysis.Proj.Services
{
    /// <summary>
    /// Provides services for analyzing and comparing text embeddings using cosine similarity and Euclidean distance.
    /// This class supports tasks such as comparing text/document embeddings, processing dataset embeddings from CSV files,
    /// and analyzing similarity between input texts and precomputed embeddings stored in JSON files.
    /// </summary>
    public class EmbeddingAnalysisService
    {
        private readonly OpenAiEmbeddingService _embeddingService;
        private readonly CosineSimilarity _similarityCalculator;
        private readonly EuclideanDistance _euclideanDistCalc;
        private readonly EmbeddingUtils _embeddingUtils;
        private readonly PdfHelper _pdfHelper;
        private readonly CSVHelper _csvHelper;
        private readonly JsonHelper _jsonHelper;

        public EmbeddingAnalysisService()
        {
            _embeddingService = new OpenAiEmbeddingService();
            _similarityCalculator = new CosineSimilarity();
            _euclideanDistCalc = new EuclideanDistance();
            _embeddingUtils = new EmbeddingUtils();
            _pdfHelper = new PdfHelper();
            _csvHelper = new CSVHelper();
            _jsonHelper = new JsonHelper();
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
        public async Task ComparePdfDocumentsAsync(string pdf1, string pdf2, PdfHelper.ChunkType chunkType = PdfHelper.ChunkType.None)
        {
            // Default directory for PDF files
            string defaultPath = @"..\..\..\PDFs";

            // Construct full file paths
            string pdfFilePath1 = Path.Combine(defaultPath, pdf1);
            string pdfFilePath2 = Path.Combine(defaultPath, pdf2);

            Console.WriteLine($"Extracting text from: {pdf1} and {pdf2}");

            // Extract text from both PDFs
            var textChunks1 = _pdfHelper.ExtractTextChunks(pdfFilePath1, chunkType);
            var textChunks2 = _pdfHelper.ExtractTextChunks(pdfFilePath2, chunkType);

            if (textChunks1.Count == 0 || textChunks2.Count == 0)
            {
                throw new InvalidOperationException("One or both PDF files contain no extractable text.");
            }

            Console.WriteLine("Generating embeddings for both documents...");

            // Generate embeddings for extracted text
            var embeddingsDoc1 = await _embeddingService.CreateEmbeddingsAsync(textChunks1);
            var embeddingsDoc2 = await _embeddingService.CreateEmbeddingsAsync(textChunks2);

            // Compute average embeddings for comparison
            var vectorA = _embeddingUtils.GetAverageEmbedding(embeddingsDoc1);
            var vectorB = _embeddingUtils.GetAverageEmbedding(embeddingsDoc2);

            // Compute similarity metrics
            double cosineSimilarity = _similarityCalculator.ComputeCosineSimilarity(vectorA, vectorB);
            double euclideanDistance = _euclideanDistCalc.ComputeEuclideanDistance(vectorA, vectorB);

            // Output results
            Console.WriteLine($"Cosine Similarity: {cosineSimilarity}");
            Console.WriteLine($"Euclidean Distance: {euclideanDistance}");
        }


        /// <summary>
        /// Processes dataset embeddings from a CSV file and saves them as JSON.
        /// </summary>
        public async Task<List<MultiEmbeddingRecord>> ProcessDataSetEmbeddingsAsync(
            List<string> fields,
            string csvFileName
        )
        {
            // Validate that the fields list is not null or empty
            if (fields == null || fields.Count == 0)
            {
                throw new ArgumentException("The fields list cannot be null or empty.");
            }

            // Construct the dataset file path (assuming 'Datasets' folder is in the project directory)
            string datasetPath = Path.Combine(@"..\..\..\Datasets", csvFileName);
            Console.WriteLine($"Extracting records from: {datasetPath}");

            // Check if the CSV file exists
            if (!File.Exists(datasetPath))
            {
                throw new FileNotFoundException($"CSV file not found: {datasetPath}");
            }

            // Extract records from the CSV file
            var records = _csvHelper.ExtractRecordsFromCsv(fields, datasetPath);

            // Process embeddings
            foreach (var record in records)
            {
                foreach (var attribute in record.Attributes)
                {
                    var attributeName = attribute.Key;
                    var attributeValue = attribute.Value;

                    if (string.IsNullOrWhiteSpace(attributeValue)) continue;

                    // Generate embedding for the attribute value
                    var attributeEmbedding = await _embeddingService.CreateEmbeddingsAsync(new List<string> { attributeValue });

                    // Add the generated attribute embedding to the record
                    record.AddEmbedding(attributeName, new VectorData(attributeEmbedding[0].Values));
                }
            }

            // Ensure the 'Outputs' directory exists
            string outputDir = @"..\..\..\Outputs";
            Directory.CreateDirectory(outputDir);

            // Save the embeddings as a JSON file
            string jsonFilePath = Path.Combine(outputDir, $"{csvFileName}_Embeddings.json");
            await _jsonHelper.SaveRecordToJson(records, jsonFilePath);

            Console.WriteLine($"Embeddings saved to: {jsonFilePath}");

            return records;
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
