using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace SemanticSimilarityAnalysis.Proj.Helpers.Pdf
{
    /// <summary>
    /// Helper class to extract text chunks from a PDF file.
    /// </summary>
    public class PdfHelper
    {
        public enum ChunkType
        {
            Paragraph,
            Sentence,
            None
        }

        /// <summary>
        /// If splitting the text into smaller chunks is not necessary, use ChunkType.None to return the entire page text as a single string
        /// </summary>
        /// <param name="pdfFilePath">The path to the PDF file.</param>
        /// <param name="chunkType">The type of chunks to extract (paragraphs, sentences, or none).</param>
        /// <returns>A list of text chunks.</returns>
        public List<string> ExtractTextChunks(string pdfFilePath, ChunkType chunkType = ChunkType.None)
        {
            var textChunks = new List<string>();

            try
            {
                // Open the PDF file using PdfPig
                using (var document = PdfDocument.Open(pdfFilePath))
                {
                    bool isEmpty = true;

                    // Iterate through each page
                    foreach (var page in document.GetPages())
                    {
                        // Extract text from the page
                        string pageText = page.Text;

                        if (!string.IsNullOrWhiteSpace(pageText))
                        {
                            isEmpty = false;

                            // Clean the text (optional)
                            pageText = CleanText(pageText);

                            // Split the text into chunks based on the specified chunk type
                            var chunks = chunkType switch
                            {
                                ChunkType.Paragraph => SplitByParagraphs(pageText),
                                ChunkType.Sentence => SplitBySentences(pageText),
                                ChunkType.None => new List<string> { pageText },
                                _ => throw new ArgumentOutOfRangeException(nameof(chunkType), "Invalid chunk type")
                            };

                            // Add the chunks to the list
                            textChunks.AddRange(chunks);
                        }
                    }

                    // Throw an exception if the PDF file is empty
                    if (isEmpty)
                    {
                        throw new InvalidOperationException("The PDF file is empty.");
                    }
                }
            }
            // Handle the file not found exception
            catch (InvalidOperationException ex) when (ex.Message.Contains("No file exists at"))
            {
                Console.WriteLine($"The PDF file was not found: {ex.Message}");
                throw; // Re-throw the exception if needed
            }
            // Handle other exceptions
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting text from PDF: {ex.Message}");
                throw;
            }

            return textChunks;
        }


        /// <summary>
        /// Cleans the extracted text by removing extra spaces, line breaks, and non-printable characters.
        /// </summary>
        /// <param name="text">The text to clean.</param>
        /// <returns>The cleaned text.</returns>
        private string CleanText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            // Replace multiple spaces, tabs, and line breaks with a single space
            text = Regex.Replace(text, @"\s+", " ");

            // Remove non-printable characters (optional)
            text = Regex.Replace(text, @"[\x00-\x1F\x7F]", "");

            // Trim leading and trailing whitespace
            text = text.Trim();

            return text;
        }


        /// <summary>
        /// If the PDF contains clearly separated paragraphs (e.g., separated by double newlines \n\n), 
        /// splitting by paragraphs helps process the text in meaningful chunks.
        public List<string> SplitByParagraphs(string text)
        {
            return text.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        /// <summary>
        /// If text process needs to be done at a more granular level (e.g., for sentence-level analysis, machine learning, or natural language processing tasks),
        /// splitting by sentences is helpful
        /// </summary>    
        public List<string> SplitBySentences(string text)
        {
            var sentences = Regex.Split(text, @"(?<=[.!?])\s+").Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            return sentences;
        }
    }

    ///
    /// Comment out this function because the model internally handles tokenization
    ///
    // Split text into chunks by tokens
    //public List<string> SplitByTokens(string text)
    //{
    //    var tokens = new List<string>();

    //    // Use regex to match words and punctuation
    //    var pattern = @"[\w'-]+|[.,!?;]";
    //    var matches = Regex.Matches(text, pattern);

    //    foreach (Match match in matches)
    //    {
    //        tokens.Add(match.Value);
    //    }

    //    return tokens;
    //}
}
