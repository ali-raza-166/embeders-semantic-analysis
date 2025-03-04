using LanguageDetection;
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

        LanguageDetector detector=new LanguageDetector();
        detector.AddAllLanguages();
        try
        {
            // var inputs = new List<string>{"Car", "vehicle", "Airplane", "Cat", "kitten" };
            // var inputs = new List<string>
            // {
            //     "Luxury sedan with leather interior and premium sound system", 
            //     "Affordable family car with spacious seating and good fuel economy", 
            //     "Sports coupe with high-performance engine and sleek design", 
            //     "Electric vehicle with zero emissions and fast charging technology", 
            //     "Large commercial truck for transporting goods across long distances", 
            //     "Heavy-duty freight truck used for shipping large containers", 
            //     "Eco-friendly hybrid car with advanced safety features", 
            //     "Compact city car designed for urban driving with low maintenance costs"
            // };


            // await pineconeService.InitializeIndexAsync();
            // var embeddings = await embeddingService.CreateEmbeddingsAsync(inputs);
            // var listofEmbeddingVectors = new List<List<float>>();
            //
            // foreach (var vectorValues in embeddings)
            // {
            //     var vector = vectorValues.Values; // Get the vector for the current embedding
            //     // Add the vector to the vectors list
            //     listofEmbeddingVectors.Add(vector);
            // }

            
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
            //     new Dictionary<string, object?>
            //     {
            //         { "Text", inputs[index] }, 
            //         {"Language", detector.Detect(inputs[index])}
            //     }
            // )).ToList();
            // await pineconeService.UpsertEmbeddingAsync(models, "default");
            // Console.WriteLine("Vector Embeddings successfully upserted into Pinecone.");

            //create embedding for Query item to test 
            // const string query =  "苹果手机 15 的摄像头规格是什么？";
            // var queryEmbeddings = await embeddingService.CreateEmbeddingsAsync([query]);
            // var queryResponse =
            //     await pineconeService.QueryEmbeddingsAsync(queryEmbeddings[0].Values.ToList(), "default", 4, detector.Detect(query));
            // var pineconeTopKparagraphs = new List<string>();
            // Console.WriteLine($"Count of matched vectors from pinecone: {queryResponse.Count}");
            // foreach (var retrievedVector in queryResponse)
            // {
            //     foreach (var metadataItem in retrievedVector.Metadata)
            //     {
            //         Console.WriteLine($"Key: {metadataItem.Key}, Value: {metadataItem.Value}");
            //         // The first .Value accesses the MetadataValue, and the second .Value gets the actual data (e.g., string, double).
            //         if (metadataItem.Key == "Text" && metadataItem.Value?.Value is string textValue)
            //         {
            //             pineconeTopKparagraphs.Add(textValue);
            //         }           
            //     }
            //     Console.WriteLine($"ID: {retrievedVector.Id}");
            //     Console.WriteLine($"Score: {retrievedVector.Score}");
            //     Console.WriteLine(
            //         $"Embedding vector (first 10 values): {string.Join(", ", retrievedVector.Values.Take(10))}");
            //     Console.WriteLine();
            // }

            
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
            
            // var answer = await textGenerationService.GenerateTextAsync(query, pineconeTopKparagraphs);
            // Console.WriteLine(answer);
            
            
            // // Example list of embeddings (list of float lists)
            var inputs = new List<List<float>>()
            {
                new List<float> { 2.5f,  2.4f },
                new List<float> { 0.5f,  0.7f },
                new List<float> { 2.2f,  2.9f }
            };
            // Apply Dimensionality Reduction PCA
            var dimensionalityReductionService = new DimensionalityReductionService(2);  // Reduce to 2 principal components
            var reducedPcaMatrix = dimensionalityReductionService.PerformPca(inputs);
            var scaledDataPca = dimensionalityReductionService.MinMaxScaleData(reducedPcaMatrix);     
            Console.WriteLine($"Scaled Data PCA: {scaledDataPca}");

            var reducedTsneMatrix = dimensionalityReductionService.ReduceDimensionsUsingTsne(inputs, 2);
            var scaledDataTsne= dimensionalityReductionService.MinMaxScaleData(reducedTsneMatrix);
            Console.WriteLine($"Scaled Data TSNE: {scaledDataTsne}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error haha: {ex.Message}");
        }

    }
    // public List<string> GetMultilingualParagraphs()
    //     {
    //         return new List<string>
    //         {
    //             // English Paragraphs
    //             "The iPhone 15 is equipped with the latest A17 chip, providing faster performance and improved power efficiency. With the new chip, users can expect faster app launches, smoother multitasking, and a more responsive overall experience. It also features better graphics performance, making gaming and multimedia applications more immersive. Additionally, the A17 chip supports advanced machine learning capabilities, which enhances AI-driven features like facial recognition and voice assistants.",
    //             "The iPhone 15 boasts a 48MP primary camera, offering incredibly sharp and detailed images. The new camera system features a larger sensor that allows more light to be captured, resulting in better low-light performance. Additionally, the ultra-wide camera and telephoto lenses have been upgraded to provide even more versatility in capturing wide-angle shots and telephoto zoom. The phone also supports ProRAW and ProRes video recording, offering content creators professional-grade features.",
    //             "Apple's MacBook Pro models, now powered by the M2 chip, bring a significant performance upgrade compared to their predecessors. The M2 chip is designed to handle even the most demanding tasks with ease, providing an enhanced experience for video editing, gaming, and software development. With a higher GPU core count, the MacBook Pro also delivers better graphics performance, allowing for smoother rendering and better handling of 3D applications.",
    //             "The MacBook Pro comes with a stunning Retina display, offering vibrant colors and deep contrasts for an immersive viewing experience. The True Tone technology adjusts the display's white balance to match the surrounding light, ensuring that images appear natural regardless of the environment. The display also supports P3 wide color and 500 nits of brightness, making it ideal for both professional creatives and casual users who enjoy high-quality visuals.",
    //             "One of the key highlights of the iPhone 15 is its impressive battery life. Thanks to the energy-efficient A17 chip and an optimized power management system, the iPhone 15 can last up to 20 hours of video playback and up to 75 hours of audio playback. Fast charging is also available, with the device able to charge up to 50% in just 30 minutes with a compatible charger. This makes the iPhone 15 perfect for users who need a device that can keep up with their busy day-to-day activities.",
    //             "The new MacBook Pro models offer an ultra-thin design that is both lightweight and powerful. Despite its slim profile, the MacBook Pro is built to handle intensive tasks without compromising on performance. The keyboard has also been redesigned for a quieter and more comfortable typing experience. With a larger trackpad and improved speakers, the MacBook Pro provides an exceptional user experience for both work and entertainment.",
    //             
    //             // German Translations
    //             "Das iPhone 15 ist mit dem neuesten A17-Chip ausgestattet, der eine schnellere Leistung und eine verbesserte Energieeffizienz bietet. Mit dem neuen Chip können Benutzer schnellere App-Starts, flüssigeres Multitasking und eine insgesamt reaktionsschnellere Erfahrung erwarten. Es bietet auch eine bessere Grafikleistung, die Spiele- und Multimedia-Anwendungen noch fesselnder macht. Darüber hinaus unterstützt der A17-Chip fortschrittliche Funktionen für maschinelles Lernen, die KI-gesteuerte Funktionen wie Gesichtserkennung und Sprachassistenten verbessern.",
    //             "Das iPhone 15 verfügt über eine 48-MP-Hauptkamera, die unglaublich scharfe und detailreiche Bilder bietet. Das neue Kamerasystem verfügt über einen größeren Sensor, der mehr Licht einfängt, was zu einer besseren Leistung bei schwachem Licht führt. Darüber hinaus wurden das Ultraweitwinkel- und das Teleobjektiv verbessert, um noch mehr Vielseitigkeit bei der Aufnahme von Weitwinkelaufnahmen und Telezoom zu bieten. Das iPhone 15 unterstützt auch ProRAW und ProRes Videoaufnahmen, die professionelle Funktionen für Content-Ersteller bieten.",
    //             "Die MacBook Pro-Modelle von Apple, die jetzt mit dem M2-Chip ausgestattet sind, bieten einen erheblichen Leistungszuwachs im Vergleich zu ihren Vorgängern. Der M2-Chip wurde entwickelt, um selbst die anspruchsvollsten Aufgaben mühelos zu bewältigen und bietet eine verbesserte Erfahrung für Video-Bearbeitung, Gaming und Software-Entwicklung. Mit einer höheren GPU-Kernanzahl bietet das MacBook Pro auch eine bessere Grafikleistung, was für flüssigeres Rendering und bessere Handhabung von 3D-Anwendungen sorgt.",
    //             "Das MacBook Pro verfügt über ein atemberaubendes Retina-Display, das lebendige Farben und tiefe Kontraste für ein intensives Seherlebnis bietet. Die True Tone-Technologie passt die Weißabgleich des Displays an das umgebende Licht an, sodass Bilder unabhängig von der Umgebung natürlich erscheinen. Das Display unterstützt auch P3-Weißfarbe und 500 Nits Helligkeit, was es ideal für sowohl professionelle Kreative als auch für Benutzer macht, die hochwertige visuelle Darstellungen genießen.",
    //             "Ein Höhepunkt des iPhone 15 ist die beeindruckende Akkulaufzeit. Dank des energieeffizienten A17-Chips und eines optimierten Energiemanagementsystems kann das iPhone 15 bis zu 20 Stunden Videowiedergabe und bis zu 75 Stunden Audiowiedergabe bieten. Auch schnelles Laden ist verfügbar, mit dem Gerät, das in nur 30 Minuten bis zu 50 % aufgeladen werden kann. Dies macht das iPhone 15 ideal für Benutzer, die ein Gerät benötigen, das mit ihrem hektischen Alltag mithalten kann.",
    //             "Die neuen MacBook Pro-Modelle bieten ein ultradünnes Design, das sowohl leicht als auch leistungsstark ist. Trotz seines schlanken Profils ist das MacBook Pro so konzipiert, dass es intensive Aufgaben problemlos bewältigen kann, ohne die Leistung zu beeinträchtigen. Die Tastatur wurde ebenfalls neu gestaltet, um eine ruhigere und komfortablere Tipp-Erfahrung zu bieten. Mit einem größeren Trackpad und verbesserten Lautsprechern bietet das MacBook Pro ein außergewöhnliches Benutzererlebnis für Arbeit und Unterhaltung.",
    //             
    //             // Chinese Translations
    //             "iPhone 15 配备了最新的 A17 芯片，提供更快的性能和更高的能效。凭借这款新芯片，用户可以期待更快的应用启动，更流畅的多任务处理以及更加响应迅速的整体体验。它还具有更强的图形性能，使得游戏和多媒体应用更加沉浸式。此外，A17 芯片还支持先进的机器学习能力，增强了面部识别和语音助手等 AI 驱动的功能。",
    //             "iPhone 15 配备了一颗 48MP 的主摄像头，提供令人惊叹的锐利和细致图像。新的摄像头系统具有更大的传感器，可以捕捉更多的光线，从而提高低光环境下的表现。此外，超广角摄像头和长焦镜头也经过升级，提供更大的拍摄视角和更强的远摄变焦功能。该设备还支持 ProRAW 和 ProRes 视频录制，为内容创作者提供专业级功能。",
    //             "Apple 的新款 MacBook Pro 配备了 M2 芯片，相较于前代产品带来了显著的性能提升。M2 芯片旨在轻松处理最繁重的任务，为视频编辑、游戏和软件开发提供更强的支持。凭借更高的 GPU 核心数，MacBook Pro 还提供更强的图形性能，使渲染更加流畅，3D 应用程序的处理能力更强。",
    //             "MacBook Pro 配备了令人惊叹的 Retina 显示屏，提供鲜艳的色彩和深邃的对比度，带来身临其境的视觉体验。True Tone 技术会根据周围的光线调整显示屏的白平衡，确保图像在任何环境下都显得自然。该显示屏还支持 P3 广色域和 500 尼特的亮度，非常适合专业创作者以及喜欢高质量视觉体验的普通用户。",
    //             "iPhone 15 的一个亮点是其出色的电池续航。得益于高效能的 A17 芯片和优化的电源管理系统，iPhone 15 可支持最长 20 小时的视频播放和 75 小时的音频播放。还支持快速充电，使用兼容充电器时可在 30 分钟内充电至 50%。这使得 iPhone 15 非常适合那些需要全天候设备支持的用户。",
    //             "新款 MacBook Pro 提供了超薄的设计，兼具轻巧和强大的性能。尽管其外形纤薄，MacBook Pro 依然能应对高负荷任务，性能毫不妥协。键盘也经过重新设计，带来更加安静和舒适的打字体验。凭借更大的触控板和更强的扬声器，MacBook Pro 提供了卓越的工作和娱乐体验。",
    //         };
    // }
}