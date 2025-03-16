namespace Helper.Tests
{
    [TestClass]
    public class TextHelperTests
    {
        private TextHelper _textHelper = new();

        [TestInitialize]
        public void Initialize()
        {
            _textHelper = new TextHelper();
        }
        [TestMethod]
        public void ExtractWordsFromTextFile_ShouldReturnWords()
        {
            // Arrange
            string fileName = "test.txt";
            string inputDir = "../../../TestData"; // Create a test data directory
            string filePath = Path.Combine(inputDir, fileName);

            // Create a test file with sample content
            Directory.CreateDirectory(inputDir);
            File.WriteAllText(filePath, "Hello, world! This is a test.");

            // Act
            var words = _textHelper.ExtractWordsFromTextFile(fileName, inputDir);

            // Assert
            Assert.IsNotNull(words); // Ensure the result is not null
            Assert.AreEqual(2, words.Count); // Ensure the correct number of segments is returned
            CollectionAssert.Contains(words, "hello"); // Ensure the words are cleaned and split correctly
            CollectionAssert.Contains(words, "world this is a test");

            // Clean up
            RetryFileDelete(filePath); // Retry deleting the file
            Directory.Delete(inputDir);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ExtractWordsFromTextFile_ShouldThrowFileNotFoundException()
        {
            // Arrange
            string fileName = "nonexistent.txt";
            string inputDir = "../../../TestData";

            // Act
            _textHelper.ExtractWordsFromTextFile(fileName, inputDir);

            // Assert is handled by ExpectedException
        }
        [TestMethod]
        public void CleanAndSplitText_ShouldReturnCleanedWords()
        {
            // Arrange
            string text = "Hello, world! This is a test.";

            // Act
            var words = _textHelper.CleanAndSplitText(text);

            // Assert
            Assert.IsNotNull(words); // Ensure the result is not null
            Assert.AreEqual(2, words.Count); // Ensure the correct number of segments is returned
            CollectionAssert.Contains(words, "hello"); // Ensure the words are cleaned and split correctly
            CollectionAssert.Contains(words, "world this is a test");
        }
        [TestMethod]
        public void IsTextFilePath_ShouldReturnTrueForTxtFile()
        {
            // Arrange
            string filePath = "example.txt";

            // Act
            bool result = _textHelper.IsTextFilePath(filePath);

            // Assert
            Assert.IsTrue(result); // Ensure the result is true for a .txt file
        }

        [TestMethod]
        public void IsTextFilePath_ShouldReturnFalseForNonTxtFile()
        {
            // Arrange
            string filePath = "example.pdf";

            // Act
            bool result = _textHelper.IsTextFilePath(filePath);

            // Assert
            Assert.IsFalse(result); // Ensure the result is false for a non-.txt file
        }
        private void RetryFileDelete(string filePath, int maxRetries = 5, int delayMs = 100)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    File.Delete(filePath);
                    return; // Success
                }
                catch (IOException)
                {
                    System.Threading.Thread.Sleep(delayMs); // Wait before retrying
                }
            }
            throw new IOException($"Failed to delete file '{filePath}' after {maxRetries} retries.");
        }
    }
}

