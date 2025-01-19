using SemanticSimilarityAnalysis.Proj.Utils;

namespace TextExtractorTests
{
    [TestClass]
    public class TextExtractorTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestEmptyPdf()
        {
            string emptyPdfPath = @"..\..\..\PDFs\empty.pdf";
            var extractor = new TextExtractor();
            var stringWriter = new StringWriter();
            var consoleOutput = stringWriter.ToString();
            extractor.ExtractTextChunks(emptyPdfPath, TextExtractor.ChunkType.Paragraph);
            StringAssert.Contains(consoleOutput, "The PDF file is empty.");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestFileNotFound()
        {
            string notFoundPath = @"..\..\..\PDFs\notFound.pdf";
            var extractor = new TextExtractor();
            var stringWriter = new StringWriter();
            var consoleOutput = stringWriter.ToString();
            extractor.ExtractTextChunks(notFoundPath, TextExtractor.ChunkType.Paragraph);

            StringAssert.Contains(consoleOutput, "The PDF file was not found:");
        }

        [TestMethod]
        public void TestSplitByParagraphs()
        {
            var extractor = new TextExtractor();
            string text = "Paragraph 1.\n\nParagraph 2.\n\nParagraph 3.";
            List<string> paragraphs = extractor.SplitByParagraphs(text);

            Assert.AreEqual(3, paragraphs.Count, "Should split text into 3 paragraphs.");
        }

        [TestMethod]
        public void TestSplitBySentences()
        {
            var extractor = new TextExtractor();
            string text = "This is sentence 1. This is sentence 2! Is this sentence 3?";
            List<string> sentences = extractor.SplitBySentences(text);

            Assert.AreEqual(3, sentences.Count, "Should split text into 3 sentences.");
        }

        [TestMethod]
        public void TestSplitByTokens()
        {
            var extractor = new TextExtractor();
            string text = "Token1 Token2\nToken3\tToken4";
            List<string> tokens = extractor.SplitByTokens(text);

            Assert.AreEqual(4, tokens.Count, "Should split text into 4 tokens.");
        }
    }
}
