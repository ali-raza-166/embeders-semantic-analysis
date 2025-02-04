using SemanticSimilarityAnalysis.Proj.Services;
using SemanticSimilarityAnalysis.Proj.Utils;

namespace SemanticSimilarityAnalysis.Proj
{
    internal abstract class Program
    {
        private static async Task Main(string[] args)
        {
            var embeddingService = new OpenAiEmbeddingService();
            var similarityCalculator = new CosineSimilarity();
            var euclideanDistCalc = new EuclideanDistance();
            var embeddingUtils = new EmbeddingUtils();
            var csvExtractor = new CsvExtractor();

            /// For comparing document embeddings
            // string pdfPath1 = @"..\..\..\PDFs\Gundam.pdf"; // Gundam
            // string pdfPath2 = @"..\..\..\PDFs\StarWars.pdf"; // Star Wars
            //string pdfPath2 = @"..\..\..\PDFs\badminton.pdf"; // Badminton
            //string pdfPath1 = @"..\..\..\PDFs\text.pdf"; // 2-paragraph Comparison between Gundam and Starwars
            //string pdfPath2 = @"..\..\..\PDFs\text2.pdf"; // Another 2-paragraph Comparison between Gundam and Starwars

            // var textExtractor = new TextExtractor();
            // var inputsDoc1 = textExtractor.ExtractTextChunks(pdfPath1);
            // var inputsDoc2 = textExtractor.ExtractTextChunks(pdfPath2);

            string csvFilePath = @"..\..\..\Datasets\imdb_1000.csv";

            try
            {
                /// Text Comparison
                //await embeddingUtils.CompareTextEmbeddings(embeddingService, similarityCalculator, euclideanDistCalc, "Cat", "Kitten");

                /// Document Comparison
                //await embeddingUtils.CompareDocumentEmbeddings(embeddingService, similarityCalculator, euclideanDistCalc, inputsDoc1, inputsDoc2);

                await embeddingUtils.ProcessMovieEmbeddingsAsync(csvExtractor, embeddingService, csvFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
