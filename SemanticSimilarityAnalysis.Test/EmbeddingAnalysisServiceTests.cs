using OpenAI.Embeddings;
using SemanticSimilarityAnalysis.Proj.Helpers.Csv;
using SemanticSimilarityAnalysis.Proj.Helpers.Json;
using SemanticSimilarityAnalysis.Proj.Helpers.Pdf;
using SemanticSimilarityAnalysis.Proj.Helpers.Text;
using SemanticSimilarityAnalysis.Proj.Services;
using SemanticSimilarityAnalysis.Proj.Utils;

namespace Service.Tests
{
    [TestClass]
    public class EmbeddingAnalysisServiceTests
    {
        private EmbeddingAnalysisService _service = null!;
        private string _testDataDir = "TestData";
        private string _testOutputDir = "TestOutputs";

        [TestInitialize]
        public void Setup()
        {
            // Get API key from environment variables
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                         throw new ArgumentException("OPENAI_API_KEY environment variable must be set");

            // Initialize real dependencies
            var embeddingClient = new EmbeddingClient("text-embedding-3-small", apiKey);
            var embeddingService = new OpenAiEmbeddingService(embeddingClient);
            var similarityCalculator = new CosineSimilarity();
            var embeddingUtils = new EmbeddingUtils();
            var pdfHelper = new PdfHelper();
            var csvHelper = new CSVHelper();
            var jsonHelper = new JsonHelper();
            var textHelper = new TextHelper();

            _service = new EmbeddingAnalysisService(
                embeddingService,
                similarityCalculator,
                embeddingUtils,
                pdfHelper,
                csvHelper,
                jsonHelper,
                textHelper);

            // Create test directories
            Directory.CreateDirectory(_testDataDir);
            Directory.CreateDirectory(_testOutputDir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test directories
            if (Directory.Exists(_testDataDir))
            {
                Directory.Delete(_testDataDir, true);
            }
            if (Directory.Exists(_testOutputDir))
            {
                Directory.Delete(_testOutputDir, true);
            }
        }

        [TestMethod]
        public async Task CompareWordsVsWords_WithRealEmbeddings_ReturnsSimilarities()
        {
            // Arrange
            var words1 = new List<string> { "king", "queen" };
            var words2 = new List<string> { "man", "woman" };

            // Act
            var result = await _service.CompareWordsVsWords(words1, words2);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("king", result[0].Label);
            Assert.AreEqual("queen", result[1].Label);

            foreach (var word in words2)
            {
                Assert.IsTrue(result[0].Similarities[word] > 0.3,
                    $"Similarity between 'king' and '{word}' should be > 0.3");
                Assert.IsTrue(result[1].Similarities[word] > 0.3,
                    $"Similarity between 'queen' and '{word}' should be > 0.3");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CompareWordsVsWords_WithEmptyInput_ThrowsException()
        {
            // Arrange
            var emptyList = new List<string>();

            // Act
            await _service.CompareWordsVsWords(emptyList, new List<string> { "word" });

            // Assert handled by ExpectedException
        }

        [TestMethod]
        public async Task ComparePdfsvsWords_WithExistingPdf_ReturnsReasonableSimilarities()
        {
            // Arrange
            var words = new List<string> { "technology", "science" };

            // Get the path to your test PDF file in the project
            var pdfDir = Path.Combine(@"..\..\..\", "PDFs");
            var pdfPath = Path.Combine(pdfDir, "test.pdf");

            // Verify the test file exists
            if (!File.Exists(pdfPath))
            {
                Assert.Inconclusive($"Test PDF file not found at: {pdfPath}");
                return;
            }

            // Act
            var result = await _service.ComparePdfsvsWords(words, pdfDir);

            // Assert
            Assert.AreEqual(1, result.Count, "Should return one PDF result");
            var pdfResult = result[0];

            Console.WriteLine($"Technology similarity: {pdfResult.Similarities["technology"]}");
            Console.WriteLine($"Science similarity: {pdfResult.Similarities["science"]}");

            Assert.IsTrue(pdfResult.Similarities["technology"] > 0.3,
                "Technology similarity should be > 0.3");
            Assert.IsTrue(pdfResult.Similarities["science"] > 0.2,
                "Science similarity should be > 0.2");
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public async Task CompareAllPhrasesAsync_WithInvalidDirectory_ThrowsException()
        {
            await _service.CompareAllPhrasesAsync("nonexistent_directory");
        }

        [TestMethod]
        public async Task CompareAllPhrasesAsync_WithEmptyFile_SkipsFile()
        {
            // Arrange
            var sentencesDir = Path.Combine(_testDataDir, "TXTs", "Sentences");
            Directory.CreateDirectory(sentencesDir);

            var emptyFile = Path.Combine(sentencesDir, "empty.txt");
            File.WriteAllText(emptyFile, "");

            var validFile = Path.Combine(sentencesDir, "valid.txt");
            File.WriteAllText(validFile, "Test phrase\nAnother phrase");

            // Act
            var result = await _service.CompareAllPhrasesAsync(sentencesDir);

            // Assert
            Assert.IsFalse(result.ContainsKey("empty.txt"),
                "Empty file should be skipped");
            Assert.IsTrue(result.ContainsKey("valid.txt"),
                "Valid file should be processed");
        }

        [TestMethod]
        public async Task CreateDataSetEmbeddingsAsync_WithValidCSV_CreatesJsonFile()
        {
            // Arrange
            var fields = new List<string> { "title", "content" };
            var csvContent = "title,content\nTest Title,Test Content";
            var csvPath = Path.Combine(_testDataDir, "test.csv");
            File.WriteAllText(csvPath, csvContent);

            // Act
            var result = await _service.CreateDataSetEmbeddingsAsync(
                fields,
                "test.csv",
                inputDir: _testDataDir,
                outputDir: _testOutputDir);

            // Assert
            var outputPath = Path.Combine(_testOutputDir, "test_Embeddings.json");
            Assert.IsTrue(File.Exists(outputPath), "Output JSON file should be created");

            var fileContent = File.ReadAllText(outputPath);
            Assert.IsTrue(fileContent.Contains("Test Title"), "JSON should contain title");
            Assert.IsTrue(fileContent.Contains("Test Content"), "JSON should contain content");

            Assert.AreEqual(1, result.Count, "Should return one record");
        }



    }
}