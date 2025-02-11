using Pinecone;
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
            var pineconeService = new PineconeService();
            var textGenerationService = new OpenAiTextGenerationService();

            var inputs = new List<string>
            {
                // AI in Healthcare - Paragraph 1
                "Artificial intelligence (AI) is revolutionizing healthcare by enhancing diagnostic accuracy, streamlining administrative processes, and personalizing treatment plans. AI-powered tools, such as machine learning algorithms, analyze vast amounts of medical data to detect patterns that human doctors might miss. Radiology has particularly benefited from AI applications, where deep learning models can identify anomalies in X-rays, MRIs, and CT scans with remarkable precision. Additionally, AI chatbots and virtual assistants improve patient engagement by answering medical queries, scheduling appointments, and offering preliminary diagnoses. Beyond diagnostics, AI is transforming drug discovery by accelerating research timelines, reducing costs, and enabling pharmaceutical companies to develop targeted therapies.",

                // AI in Healthcare - Paragraph 2
                "Despite its promise, AI in healthcare presents challenges, including data privacy concerns, ethical considerations, and regulatory compliance. AI algorithms require large datasets to train effectively, raising concerns about patient confidentiality and security. Bias in AI models is another critical issue, as algorithms trained on non-representative data can produce skewed results, potentially leading to misdiagnosis. Furthermore, while AI can enhance decision-making, it cannot replace human judgment entirely. Doctors must interpret AI-generated insights within the broader context of a patient’s medical history and symptoms. As AI adoption in healthcare grows, regulatory bodies must establish clear guidelines to ensure safe, fair, and ethical use of AI-driven solutions.",

                // Future of Space Exploration - Paragraph 1
                "Space exploration is entering an exciting new era, driven by technological advancements and international collaborations. Private companies like SpaceX and Blue Origin are developing reusable rockets, significantly reducing the cost of space travel. NASA’s Artemis program aims to establish a sustainable human presence on the Moon, serving as a stepping stone for future Mars missions. Meanwhile, countries like China and India are expanding their space programs, launching lunar and interplanetary missions. Advancements in artificial intelligence and robotics are also playing a key role, enabling autonomous spacecraft to navigate and conduct research in deep space without direct human intervention.",

                // Future of Space Exploration - Paragraph 2
                "The future of space exploration extends beyond government programs, with growing interest in commercial space travel and space tourism. Companies like Virgin Galactic are pioneering suborbital flights, bringing space travel closer to the public. Long-term goals include asteroid mining, which could provide valuable resources like rare metals, and space colonization to ensure humanity’s survival beyond Earth. However, space exploration also presents challenges, including high costs, radiation exposure, and the psychological effects of long-duration missions. Overcoming these obstacles will require continued innovation, collaboration, and international regulations to govern space activities responsibly.",

                // Sustainable Energy Solutions - Paragraph 1
                "Sustainable energy solutions are critical to mitigating climate change and reducing dependence on fossil fuels. Renewable energy sources such as solar, wind, and hydroelectric power are becoming increasingly viable alternatives. Solar panels have become more efficient and affordable, enabling widespread adoption in both residential and commercial sectors. Wind farms are expanding globally, providing clean energy to millions. Energy storage technologies, such as advanced lithium-ion and solid-state batteries, are improving grid reliability by addressing the intermittency of renewable energy sources. Governments and businesses worldwide are investing in clean energy initiatives, aiming for carbon neutrality in the coming decades.",

                // Sustainable Energy Solutions - Paragraph 2
                "Despite progress, transitioning to sustainable energy faces challenges such as infrastructure limitations, energy storage constraints, and economic barriers. The intermittency of solar and wind energy necessitates better grid management and large-scale battery solutions. Additionally, transitioning from traditional energy sources requires significant investments in new technologies and policy frameworks to incentivize adoption. Nuclear power, often debated, offers a low-carbon energy alternative but raises concerns about waste disposal and safety. To achieve a sustainable future, governments, businesses, and individuals must work together to develop and implement innovative energy solutions that balance environmental impact, affordability, and efficiency."
            };

            try
            {
                await pineconeService.InitializeIndexAsync();
                var embeddings = await embeddingService.CreateEmbeddingsAsync(inputs);

                // if (embeddings.Count >= 2)
                // {
                //     for (var i = 0; i < embeddings.Count - 1; i++)
                //     {
                //         var vectorA = embeddings[i].Values;
                //         var vectorB = embeddings[i + 1].Values;
                //         
                //         var cosineSimilarity = similarityCalculator.ComputeCosineSimilarity(vectorA, vectorB);
                //         Console.WriteLine($"Cosine Similarity between '{inputs[i]}' and '{inputs[i + 1]}': {cosineSimilarity}");
                //         
                //         var euclideanDistance = euclideanDistCalc.ComputeEuclideanDistance(vectorA, vectorB);
                //         Console.WriteLine($"Euclidean Distance between '{inputs[i]}' and '{inputs[i + 1]}': {euclideanDistance}");
                //     }
                //
                // } 

                
                // var models = embeddings.Select((embedding, index) => new PineconeModel(
                //     embedding.Id,
                //     embedding.Values.ToList(),
                //     new Dictionary<string, object?> { { "Text", inputs[index] } }
                // )).ToList();
                // await pineconeService.UpsertEmbeddingAsync(models, "default");
                // Console.WriteLine("Vector Embeddings successfully upserted into Pinecone.");

                //create embedding for Query item to test 
                var queryEmbeddings = await embeddingService.CreateEmbeddingsAsync([
                    "What regulatory bodies should do with growth of AI in healthcare?"
                ]);
                var queryResponse =
                    await pineconeService.QueryEmbeddingsAsync(queryEmbeddings[0].Values.ToList(), "default", 1);
                
                Console.WriteLine($"Count of matched vectors from pinecone: {queryResponse.Count}");
                foreach (var model in queryResponse)
                {
                    Console.WriteLine($"ID: {model.Id}");
                    Console.WriteLine($"Score: {model.Score}");
                    Console.WriteLine(
                        $"Embedding vector (first 10 values): {string.Join(", ", model.Values.Take(10))}");
                    Console.WriteLine(); 
                }

                Console.WriteLine("Results computed by Manual TopK Method");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
    
}