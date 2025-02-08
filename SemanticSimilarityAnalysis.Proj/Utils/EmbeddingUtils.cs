using SemanticSimilarityAnalysis.Proj.Interfaces;
using SemanticSimilarityAnalysis.Proj.Services;
namespace SemanticSimilarityAnalysis.Proj.Utils
{
    internal class EmbeddingUtils
    {
        public static List<float> GetAverageEmbedding(List<IVectorData> embeddings)
        {
            if (embeddings == null || embeddings.Count == 0)
            {
                throw new ArgumentException("The embeddings list cannot be null or empty.");
            }
            var numOfEmbeddings = embeddings.Count;
            var vectorLength = embeddings[0].Values.Count;
            var avgEmbedding = new List<float>(new float[vectorLength]);

            foreach (var embedding in embeddings)
            {
                for (var i = 0; i < vectorLength; i++)
                {
                    avgEmbedding[i] += embedding.Values[i];
                }
            }

            for (var i = 0; i < vectorLength; i++)
            {
                avgEmbedding[i] /= numOfEmbeddings;
            }

            return avgEmbedding;
        }


        public async Task CompareTextEmbeddings(
            OpenAiEmbeddingService embeddingService,
            CosineSimilarity similarityCalculator,
            EuclideanDistance euclideanDistCalc,
            string text1,
            string text2
        )
        {
            var inputs = new List<string> { text1, text2 };
            var embeddings = await embeddingService.CreateEmbeddingsAsync(inputs);

            if (embeddings.Count < 2)
                throw new InvalidOperationException("Insufficient embeddings generated.");

            var vectorA = embeddings[0].Vector;
            var vectorB = embeddings[1].Vector;

            double cosineSimilarity = similarityCalculator.ComputeCosineSimilarity(vectorA, vectorB);
            double euclideanDistance = euclideanDistCalc.ComputeEuclideanDistance(vectorA, vectorB);

            Console.WriteLine($"Cosine Similarity between '{text1}' and '{text2}': {cosineSimilarity}");
            Console.WriteLine($"Euclidean Distance between '{text1}' and '{text2}': {euclideanDistance}");
        }


        public async Task CompareDocumentEmbeddings(
            OpenAiEmbeddingService embeddingService,
            CosineSimilarity similarityCalculator,
            EuclideanDistance euclideanDistCalc,
            List<string> doc1Texts,
            List<string> doc2Texts
        )
        {
            Console.WriteLine("Generating embeddings... Document 1");
            var embeddingsDoc1 = await embeddingService.CreateEmbeddingsAsync(doc1Texts);

            Console.WriteLine("\nGenerating embeddings... Document 2");
            var embeddingsDoc2 = await embeddingService.CreateEmbeddingsAsync(doc2Texts);

            var vectorA = EmbeddingUtils.GetAverageEmbedding(embeddingsDoc1);
            var vectorB = EmbeddingUtils.GetAverageEmbedding(embeddingsDoc2);

            double cosineSimilarity = similarityCalculator.ComputeCosineSimilarity(vectorA, vectorB);
            double euclideanDistance = euclideanDistCalc.ComputeEuclideanDistance(vectorA, vectorB);

            Console.WriteLine(embeddingsDoc1);
            Console.WriteLine(embeddingsDoc2);

            Console.WriteLine($"Cosine Similarity: {cosineSimilarity}");
            Console.WriteLine($"Euclidean Distance: {euclideanDistance}");

        }

        public async Task ProcessMovieEmbeddingsAsync(
            CsvExtractor csvExtractor,
            OpenAiEmbeddingService embeddingService,
            string csvFilePath,
            string outputDirectory = @"..\..\..\Output",
            string outputFile = "embeddings.json"
        )
        {
            string jsonFilePath = Path.Combine(outputDirectory, outputFile);

            var movies = csvExtractor.ExtractRecordsFromCsv(csvFilePath);

            var csvProcessor = new CsvProcessor(embeddingService, jsonFilePath);
            await csvProcessor.ProcessAndGenerateEmbeddingsAsync(movies);

            Console.WriteLine("Embeddings successfully generated and saved to JSON.");
        }

    }
}