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

            // string pdfPath1 = @"..\..\..\PDFs\Gundam.pdf"; // Gundam
            // string pdfPath2 = @"..\..\..\PDFs\StarWars.pdf"; // Star Wars
            //string pdfPath2 = @"..\..\..\PDFs\badminton.pdf"; // Badminton
            //string pdfPath1 = @"..\..\..\PDFs\text.pdf"; // 2-paragraph Comparison between Gundam and Starwars
            //string pdfPath2 = @"..\..\..\PDFs\text2.pdf"; // Another 2-paragraph Comparison between Gundam and Starwars

            // var textExtractor = new TextExtractor();
            // var inputsDoc1 = textExtractor.ExtractTextChunks(pdfPath1);
            // var inputsDoc2 = textExtractor.ExtractTextChunks(pdfPath2);

            var inputs = new List<string>
            {
                "Cat",
                "Kitten"
            };

            try
            {

                /// Word Comparison
                //var embeddings = await embeddingService.CreateEmbeddingsAsync(inputs);
                //if (embeddings.Count >= 2)
                //{
                //    var vectorA = embeddings[0].EmbeddingVector;
                //    var vectorB = embeddings[1].EmbeddingVector;

                // var embeddings = await embeddingService.CreateEmbeddingsAsync(inputs);
                // if (embeddings.Count >= 2)
                // {
                //    Console.WriteLine($"Found {embeddings} ");
                //    var vectorA = embeddings[0].Vector;
                //    var vectorB = embeddings[1].Vector;


                //    var cosineSimilarity = similarityCalculator.ComputeCosineSimilarity(vectorA, vectorB);
                //    Console.WriteLine($"Cosine Similarity between '{inputs[0]}' and '{inputs[1]}': {cosineSimilarity}");

                //    var euclideanDistance = euclideanDistCalc.ComputeEuclideanDistance(vectorA, vectorB);
                //    Console.WriteLine($"Euclidean Distance between '{inputs[0]}' and '{inputs[1]}': {euclideanDistance}");

                }


                /// PDF Text Comparison
                //Console.WriteLine("Generating embeddings... PDF1");
                //var embeddingsDoc1 = await embeddingService.CreateEmbeddingsAsync(inputsDoc1);
                //Console.WriteLine("\nGenerating embeddings... PDF2");
                //var embeddingsDoc2 = await embeddingService.CreateEmbeddingsAsync(inputsDoc2);

                //var vectorA = EmbeddingUtils.GetAverageEmbedding(embeddingsDoc1);
                //var vectorB = EmbeddingUtils.GetAverageEmbedding(embeddingsDoc2);


                //var cosineSimilarity = similarityCalculator.ComputeCosineSimilarity(vectorA, vectorB);
                //var euclideanDistance = euclideanDistCalc.ComputeEuclideanDistance(vectorA, vectorB);


                //Console.WriteLine($"Cosine Similarity: {cosineSimilarity}");
                //Console.WriteLine($"Euclidean Distance: {euclideanDistance}");

                // Generate embeddings
                // Console.WriteLine("Generating embeddings... PDF1");
                // var embeddingsDoc1 = await embeddingService.CreateEmbeddingsAsync(inputsDoc1);
                // Console.WriteLine("\nGenerating embeddings... PDF2");
                // var embeddingsDoc2 = await embeddingService.CreateEmbeddingsAsync(inputsDoc2);
                
                // var vectorA = EmbeddingUtils.GetAverageEmbedding(embeddingsDoc1);
                // var vectorB = EmbeddingUtils.GetAverageEmbedding(embeddingsDoc2);
                
                // var cosineSimilarity = similarityCalculator.ComputeCosineSimilarity(vectorA, vectorB);
                // var euclideanDistance = euclideanDistCalc.ComputeEuclideanDistance(vectorA, vectorB);

       
                // Console.WriteLine($"Cosine Similarity: {cosineSimilarity}");
                // Console.WriteLine($"Euclidean Distance: {euclideanDistance}");

                //Console.WriteLine(embeddingsDoc1);
                //Console.WriteLine(embeddingsDoc2);

                CsvExtractor csvExtractor = new CsvExtractor();
                var movies = csvExtractor.ExtractMoviesFromCsv(@"..\..\..\Datasets\imdb_1000.csv");
                Console.WriteLine(movies.Count);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
