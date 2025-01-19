using Patagames.Pdf.Net;
using System.Text.RegularExpressions;

namespace SemanticSimilarityAnalysis.Proj.Utils
{
    public class TextExtractor
    {
        public enum ChunkType
        {
            Paragraph,
            Sentence,
            Token
        }

        // Function to extract text chunks from the PDF file with different chunking options
        public List<string> ExtractTextChunks(string pdfFilePath, ChunkType chunkType = ChunkType.Paragraph)
        {
            // Initialize the SDK library
            PdfCommon.Initialize();

            var textChunks = new List<string>();

            try
            {
                // Open and load the PDF document
                using (var doc = PdfDocument.Load(pdfFilePath))
                {
                    // Flag to track if the PDF is empty
                    bool isEmpty = true;

                    foreach (var page in doc.Pages)
                    {
                        // Get the total number of characters on the page
                        int totalCharCount = page.Text.CountChars;

                        // Extract text from the page
                        string pageText = page.Text.GetText(0, totalCharCount);

                        if (!string.IsNullOrWhiteSpace(pageText))
                        {
                            isEmpty = false;

                            // Call the appropriate chunking method based on the user's choice
                            var chunks = chunkType switch
                            {
                                ChunkType.Paragraph => SplitByParagraphs(pageText),
                                ChunkType.Sentence => SplitBySentences(pageText),
                                ChunkType.Token => SplitByTokens(pageText),
                                _ => SplitByParagraphs(pageText)
                            };

                            foreach (var chunk in chunks)
                            {
                                textChunks.Add(chunk.Trim());
                            }
                        }

                        // Dispose of the page to free resources
                        page.Dispose();
                    }

                    if (isEmpty)
                    {
                        throw new InvalidOperationException("The PDF file is empty.");
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"The PDF file was not found: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("The PDF file is empty.");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting text from PDF: {ex.Message}");
            }
            finally
            {
                // Clean up the SDK resources
                PdfCommon.Release();
            }

            return textChunks;
        }

        // Split text into chunks by paragraphs
        public List<string> SplitByParagraphs(string text)
        {
            return text.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        // Split text into chunks by sentences
        public List<string> SplitBySentences(string text)
        {
            var sentenceEndings = new[] { ".", "!", "?" }; // Define sentence-ending punctuation
            var sentences = Regex.Split(text, @"(?<=[.!?])\s+").Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            return sentences;
        }

        // Split text into chunks by tokens
        public List<string> SplitByTokens(string text)
        {
            var tokens = new List<string>();

            // Use regex to match words and punctuation
            var pattern = @"[\w'-]+|[.,!?;]";
            var matches = Regex.Matches(text, pattern);

            foreach (Match match in matches)
            {
                tokens.Add(match.Value);
            }

            return tokens;
        }
    }
}
