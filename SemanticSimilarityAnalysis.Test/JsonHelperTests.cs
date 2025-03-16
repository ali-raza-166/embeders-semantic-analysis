using SemanticSimilarityAnalysis.Proj.Helpers.Json;
using SemanticSimilarityAnalysis.Proj.Models;
using System.Text.Json;

namespace Helper.Tests
{
    [TestClass]
    public class JsonHelperTests
    {
        private JsonHelper _jsonHelper = new();

        [TestMethod]
        public void TestValidJsonFile()
        {
            // Arrange
            string filePath = @"../../../JSONs/input.json";
            var expectedRecords = new List<MultiEmbeddingRecord>
        {
            new MultiEmbeddingRecord(
                new Dictionary<string, string> { { "title", "Movie 1" } },
                new Dictionary<string, List<VectorData>> { { "field1", new List<VectorData> { new VectorData(new List<float> { 1.0f, 2.0f, 3.0f }) } } }
            ),
            new MultiEmbeddingRecord(
                new Dictionary<string, string> { { "title", "Movie 2" } },
                new Dictionary<string, List<VectorData>> { { "field1", new List<VectorData> { new VectorData(new List<float> { 4.0f, 5.0f, 6.0f }) } } }
            )
        };

            // Create a temporary JSON file for testing
            File.WriteAllText(filePath, JsonSerializer.Serialize(expectedRecords));

            // Act
            var result = _jsonHelper.GetRecordFromJson(filePath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedRecords.Count, result.Count);
            for (int i = 0; i < expectedRecords.Count; i++)
            {
                Assert.AreEqual(expectedRecords[i].Attributes["title"], result[i].Attributes["title"]);
                Assert.AreEqual(expectedRecords[i].Vectors["field1"][0].Values[0], result[i].Vectors["field1"][0].Values[0]);
            }

            // Clean up
            File.Delete(filePath);
        }

        [TestMethod]
        public void TestInvalidJsonFile()
        {
            // Arrange
            string filePath = "NonExistentFile.json"; // Non-existent file path
            Console.WriteLine($"Testing with file path: {filePath}");

            // Act 
            var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _jsonHelper.GetRecordFromJson(filePath);
            });

            // Assert
            Assert.AreEqual("File not found.", exception.Message);
            Console.WriteLine("Test passed: InvalidOperationException was thrown.");
        }

        [TestMethod]
        public async Task TestSaveRecordToJson_DirectoryDoesNotExist()
        {
            // Arrange
            var records = new List<MultiEmbeddingRecord>
    {
        new MultiEmbeddingRecord(
            new Dictionary<string, string> { { "title", "Movie 1" } },
            new Dictionary<string, List<VectorData>>
            {
                { "field1", new List<VectorData> { new VectorData(new List<float> { 1.0f, 2.0f, 3.0f }) } } }
            )
    };

            string jsonFilePath = "NonExistentDirectory/output.json";
            var directoryPath = Path.GetDirectoryName(jsonFilePath);

            // Ensure the directory does not exist before the test
            if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath, true);

            // Act
            await _jsonHelper.SaveRecordToJson(records, jsonFilePath);

            // Assert
            Assert.IsTrue(Directory.Exists(directoryPath)); // Directory was created
            Assert.IsTrue(File.Exists(jsonFilePath)); // File was saved

            // Verify file content
            var fileContent = await File.ReadAllTextAsync(jsonFilePath);
            var deserializedRecords = JsonSerializer.Deserialize<List<MultiEmbeddingRecord>>(fileContent);
            Assert.AreEqual(records[0].Attributes["title"], deserializedRecords?[0].Attributes["title"]);

            // Clean up
            File.Delete(jsonFilePath);
            Directory.Delete(directoryPath);
        }


        [TestMethod]
        public async Task SaveRecordToJson_DirectoryExists()
        {
            // Arrange
            var records = new List<MultiEmbeddingRecord>
            {
                new MultiEmbeddingRecord(
                    new Dictionary<string, string> { { "title", "Movie 1" } },
                    new Dictionary<string, List<VectorData>>
                    {
                        { "field1", new List<VectorData> { new VectorData(new List<float> { 1.0f, 2.0f, 3.0f }) } }
                    })
            };

            string jsonFilePath = "ExistingDirectory/output.json";
            var directoryPath = Path.GetDirectoryName(jsonFilePath);

            // Ensure the directory exists before the test
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath!);
            }

            // Act
            await _jsonHelper.SaveRecordToJson(records, jsonFilePath);

            // Assert
            Assert.IsTrue(File.Exists(jsonFilePath)); // File was saved

            // Clean up
            File.Delete(jsonFilePath);
            Directory.Delete(directoryPath!);
        }
    }
}