using SemanticSimilarityAnalysis.Proj.Helpers.Csv;

namespace Helper.Tests
{
    [TestClass]
    public class CSVHelperTests
    {
        [TestMethod]
        public void ExtractRecordsFromCsv_ValidCsv_ReturnsCorrectRecords()
        {
            var csvHelper = new CSVHelper();
            var fields = new List<string> { "Field1", "Field2", "Field3" };
            var validCsvFilePath = "valid_test.csv";

            // Create a valid CSV file
            File.WriteAllText(validCsvFilePath, "Field1,Field2,Field3\nValue1,Value2,Value3\nValue4,Value5,Value6\nValue7,Value8,Value9");

            // Act
            var result = csvHelper.ExtractRecordsFromCsv(fields, validCsvFilePath);

            // Assert
            Assert.AreEqual(3, result.Count);

            Assert.AreEqual("Value1", result[0].Attributes["Field1"]);
            Assert.AreEqual("Value2", result[0].Attributes["Field2"]);
            Assert.AreEqual("Value3", result[0].Attributes["Field3"]);

            Assert.AreEqual("Value4", result[1].Attributes["Field1"]);
            Assert.AreEqual("Value5", result[1].Attributes["Field2"]);
            Assert.AreEqual("Value6", result[1].Attributes["Field3"]);

            Assert.AreEqual("Value7", result[2].Attributes["Field1"]);
            Assert.AreEqual("Value8", result[2].Attributes["Field2"]);
            Assert.AreEqual("Value9", result[2].Attributes["Field3"]);

            File.Delete(validCsvFilePath);
        }

        // Test if the method throws an exception for non-existent file
        [TestMethod]
        public void ExtractRecordsFromCsv_FileNotFound_ThrowsFileNotFoundException()
        {
            var csvHelper = new CSVHelper();
            var fields = new List<string> { "Field1", "Field2", "Field3" };
            var nonExistentFilePath = "non_existent.csv";

            // Act & Assert: Check if the method throws FileNotFoundException
            var exception = Assert.ThrowsException<FileNotFoundException>(() =>
                csvHelper.ExtractRecordsFromCsv(fields, nonExistentFilePath));

            // Assert: Verify the exception message
            Assert.AreEqual($"CSV file not found: {nonExistentFilePath}", exception.Message, "Exception message is incorrect.");
        }

        // Test if the method throws an exception for the CSV missing one of the fields
        [TestMethod]
        public void ExtractRecordsFromCsv_MissingCsv_ThrowsCsvHelperException()
        {
            var csvHelper = new CSVHelper();
            var fields = new List<string> { "Field1", "Field2", "Field3" };
            var missingCsvFilePath = "missing_test.csv";

            // Create an invalid CSV file (e.g., missing headers or inconsistent data)
            File.WriteAllText(missingCsvFilePath, "Field1,Field2\nValue1,Value2");

            // Act & Assert: Check if the method throws CsvHelperException
            Assert.ThrowsException<CsvHelper.MissingFieldException>(() =>
                 csvHelper.ExtractRecordsFromCsv(fields, missingCsvFilePath));

            // Clean up
            File.Delete(missingCsvFilePath);
        }
    }
}