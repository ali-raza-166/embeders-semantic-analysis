using SemanticSimilarityAnalysis.Proj.Models;
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
            var pineconeService = new PineconeService();

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

                var inputs = new List<string>
            {
                "Cat",
                "Kitten"
            };

                await pineconeService.InitializeIndexAsync();
                var embeddings = await embeddingService.CreateEmbeddingsAsync(inputs);
                if (embeddings.Count >= 2)
                {
                    Console.WriteLine($"Found {embeddings} ");
                    var vectorA = embeddings[0].Values;
                    var vectorB = embeddings[1].Values;


                    var cosineSimilarity = similarityCalculator.ComputeCosineSimilarity(vectorA, vectorB);
                    Console.WriteLine($"Cosine Similarity between '{inputs[0]}' and '{inputs[1]}': {cosineSimilarity}");

                    var euclideanDistance = euclideanDistCalc.ComputeEuclideanDistance(vectorA, vectorB);
                    Console.WriteLine($"Euclidean Distance between '{inputs[0]}' and '{inputs[1]}': {euclideanDistance}");
                }
                // var vectors = new[]
                // {
                //     new Vector
                //     {
                //         Id = embeddings[0].Id,  // ID for "Cat"
                //         Values = embeddings[0].Vector.ToArray(),  // Embedding for "Cat"
                //         Metadata = new Metadata
                //         {
                //             ["Text"] = new MetadataValue(inputs[0]),
                //         }
                //     },
                //     new Vector
                //     {
                //         Id = embeddings[1].Id,  // ID for "Kitten"
                //         Values = embeddings[1].Vector.ToArray(),  // Embedding for "Kitten"
                //         Metadata = new Metadata
                //         {
                //             ["Text"] = new MetadataValue(inputs[1]),
                //         }
                //     }
                // };

                // Upsert request
                // var upsertRequest = new UpsertRequest
                // {
                //     Vectors = vectors,
                //     Namespace = "default"
                // };
                // Accessing Pinecone index and upserting data
                // var index = pineconeService.GetPineconeIndex();
                // var upsertResponse = await index.UpsertAsync(upsertRequest);
                // Console.WriteLine("Embeddings successfully upserted into Pinecone.");
                // Create PineconeModel instances for each embedding
                var models = embeddings.Select((embedding, index) => new PineconeModel(
                    embedding.Id,            // ID for the vector
                    embedding.Values.ToList(), // Convert embedding vector to List<float>
                    new Dictionary<string, object?> { { "Text", inputs[index] } } // Metadata
                )).ToList();
                // var ids = embeddings.Select(e => e.Id).ToList();
                // var vectors = embeddings.Select(e => e.Vector.ToList()).ToList();
                // // Create metadata using the original input list directly
                // var metadataList = embeddings.Select((embedding, index) => new Dictionary<string, object?>
                // {
                //     { "Text", inputs[index] }  // Use the original input text directly from the `inputs` list
                // }).ToList();


                // Call UpsertEmbeddingAsync to insert data into Pinecone
                // await pineconeService.UpsertEmbeddingAsync(ids, vectors, metadataList, "default");

                Console.WriteLine("Embeddings successfully upserted into Pinecone.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
