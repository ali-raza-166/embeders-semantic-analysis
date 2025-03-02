using Microsoft.Extensions.DependencyInjection;
using SemanticSimilarityAnalysis.Proj.Models;
using SemanticSimilarityAnalysis.Proj.Services;
namespace SemanticSimilarityAnalysis.Proj.Utils;

public class ProcessorAli
{
    private readonly IServiceProvider _serviceProvider;
    public ProcessorAli(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task RunAsync()
    {
        var embeddingService = _serviceProvider.GetRequiredService<OpenAiEmbeddingService>();
        var similarityCalculator = _serviceProvider.GetRequiredService<CosineSimilarity>();
        var euclideanDistCalc = _serviceProvider.GetRequiredService<EuclideanDistance>();
        var pineconeService = _serviceProvider.GetRequiredService<PineconeService>();
        var textGenerationService = _serviceProvider.GetRequiredService<OpenAiTextGenerationService>();
        var inputs = new List<string>
        {
            // iPhone 15
            "The iPhone 15, released in 2023, boasts an advanced A16 Bionic chip, offering significant improvements in processing speed and energy efficiency. With a 6.1-inch Super Retina XDR display, it supports HDR10 and Dolby Vision for a vibrant visual experience. The iPhone 15 features a dual-camera system with a 48 MP primary lens and a 12 MP ultra-wide lens, ensuring high-quality photos and videos. It has 128GB of base storage and offers 5G connectivity for faster internet speeds. The battery life is impressive, providing up to 20 hours of video playback. The iPhone 15 is available in multiple colors, including black, white, and blue, and runs on iOS 17, with improvements in security and multitasking.",

            // iPhone 15 Pro
            "The iPhone 15 Pro, introduced alongside the iPhone 15, is a premium variant with a 6.1-inch OLED display, which delivers brighter images and more accurate colors, with a maximum brightness of 2000 nits. It is powered by the new A17 Pro chip, providing enhanced performance in gaming and multitasking. The iPhone 15 Pro’s triple-camera system includes a 48 MP wide-angle lens, a 12 MP telephoto lens with 3x optical zoom, and a 12 MP ultra-wide lens. The 5G capability ensures faster download speeds. Its battery supports up to 22 hours of video playback, and it also offers USB-C charging, making it compatible with various devices. The iPhone 15 Pro is available in titanium and other premium finishes.",

            // iPhone 15 Plus
            "The iPhone 15 Plus features a 6.7-inch Super Retina XDR display, making it the largest model in the iPhone 15 series. The display is designed for an immersive viewing experience with HDR10 support. Powered by the A16 Bionic chip, the iPhone 15 Plus is highly efficient and fast, providing an excellent performance for both everyday tasks and more demanding applications. The dual-camera setup consists of a 48 MP primary camera and a 12 MP ultra-wide lens. The iPhone 15 Plus also supports 5G and offers up to 26 hours of video playback on a single charge, making it an ideal choice for users who need a larger screen and long-lasting battery life. It comes in a variety of colors, including pink, blue, and black.",

            // MacBook Air M2 (2023)
            "The MacBook Air M2, released in 2023, is equipped with Apple’s new M2 chip, which delivers up to 18% faster CPU performance compared to the previous generation. The 13.6-inch Retina display offers vibrant colors and sharp details, making it ideal for creative professionals. With a fanless design, the MacBook Air M2 is both quiet and powerful. It features a 256GB base storage option and can be configured with up to 2TB of storage. The battery life is exceptional, offering up to 18 hours of wireless web browsing. The MacBook Air M2 also supports Wi-Fi 6 and comes in space gray, starlight, and midnight colors.",

            // MacBook Pro 14-inch (2023)
            "The MacBook Pro 14-inch (2023) is designed for power users, featuring the Apple M2 Pro chip with up to 10-core CPU performance. Its Liquid Retina XDR display, 14.2 inches in size, supports ProMotion for a smoother experience and has a brightness of up to 1600 nits. The MacBook Pro comes with 512GB of storage, which can be expanded to 8TB, and offers up to 32GB of unified memory. The battery life of up to 17 hours makes it suitable for extended use, even during intense workloads. This model also includes more ports than the MacBook Air, with HDMI, Thunderbolt 4, and an SDXC card slot, and it supports 5G for fast data speeds.",

            // MacBook Pro 16-inch (2023)
            "The MacBook Pro 16-inch (2023) is a powerhouse designed for creative professionals and developers. With the M2 Max chip, it provides up to 38-core GPU performance, making it ideal for video editing, 3D rendering, and other graphics-intensive tasks. The 16.2-inch Retina display provides an immersive visual experience with stunning details, offering up to 1600 nits of peak brightness. It supports up to 64GB of unified memory and up to 8TB of SSD storage. The battery life is impressive, offering up to 21 hours of video playback. Additionally, it includes a full-sized HDMI port, three Thunderbolt 4 ports, and an SD card slot, making it perfect for professionals who need the best performance and connectivity."
        };
        
        try
        {
            // await pineconeService.InitializeIndexAsync();
            // var embeddings = await embeddingService.CreateEmbeddingsAsync(inputs);

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
                "What is the camera resolution of the iPhone 15?"
            ]);
            var queryResponse =
                await pineconeService.QueryEmbeddingsAsync(queryEmbeddings[0].Values.ToList(), "default", 1);
            var pineconeTopKparagraphs = new List<string>();
            Console.WriteLine($"Count of matched vectors from pinecone: {queryResponse.Count}");
            foreach (var retrievedVector in queryResponse)
            {
                foreach (var metadataItem in retrievedVector.Metadata)
                {
                    Console.WriteLine($"Key: {metadataItem.Key}, Value: {metadataItem.Value}");
                    // The first .Value accesses the MetadataValue, and the second .Value gets the actual data (e.g., string, double).
                    if (metadataItem.Key == "Text" && metadataItem.Value?.Value is string textValue)
                    {
                        pineconeTopKparagraphs.Add(textValue);
                    }           
                }
                Console.WriteLine($"ID: {retrievedVector.Id}");
                Console.WriteLine($"Score: {retrievedVector.Score}");
                Console.WriteLine(
                    $"Embedding vector (first 10 values): {string.Join(", ", retrievedVector.Values.Take(10))}");
                Console.WriteLine();
            }

            
            // Console.WriteLine("Results computed by Manual TopK Method");
            // var topKResults =
            //     similarityCalculator.GetTopKCosineSimilarities(queryEmbeddings[0].Values, models, topK: 1);
            // var topKParagraphs = new List<string>();
            // foreach (KeyValuePair<string, double> kvp in topKResults)
            // {
            //     int modelIndex = int.Parse(kvp.Key);
            //     topKParagraphs.Add(inputs[modelIndex]);
            //     Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
            // }
            
            const string query =  "What is the camera resolution of the iPhone 15?";
            var answer = await textGenerationService.GenerateTextAsync(query, pineconeTopKparagraphs);
            Console.WriteLine(answer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }
}