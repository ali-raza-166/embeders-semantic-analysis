using LanguageDetection;
using Microsoft.Extensions.DependencyInjection;
using SemanticSimilarityAnalysis.Proj.Pipelines;
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
        var pineconeService = _serviceProvider.GetRequiredService<PineconeService>();
        var textGenerationService = _serviceProvider.GetRequiredService<OpenAiTextGenerationService>();
        var openAiEmbeddingsDimReductionAndPlotting = _serviceProvider.GetRequiredService<OpenAiEmbeddingsDimReductionAndPlotting>();
        var word2VecEmbeddingsDimReductionAndPlotting = _serviceProvider.GetRequiredService<Word2VecEmbeddingsDimReductionAndPlotting>();
        var pineconeSetupService = _serviceProvider.GetRequiredService<PineconeSetup>();
        var chatbotService = _serviceProvider.GetRequiredService<ChatbotService>();
        var ragPipeline = _serviceProvider.GetRequiredService<RagPipeline>();

        LanguageDetector detector = new();
        detector.AddAllLanguages();
        try
        {

            //--------------- Creating Embeddings and Storing in a new list. SEE DATA AT THE END OF THIS FILE ----------------------------
            // var embeddings = await embeddingService.CreateEmbeddingsAsync(inputs);
            // var listofEmbeddingVectors = new List<List<float>>();
            //
            // foreach (var vectorValues in embeddings)
            // {
            //     var vector = vectorValues.Values; // Get the vector for the current embedding
            //     listofEmbeddingVectors.Add(vector);
            // }


            //--------------- Testing Manual Method for TopK Searching --------------------------------------
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


            //---------------Testing Dimensionality Reduction Pipelines ---------------------------------- 
            // await openAiEmbeddingsDimReductionAndPlotting.RunPipelineAsync(inputs); 
            // word2VecEmbeddingsDimReductionAndPlotting.RunPipeline(inputs);


            //---------------Testing pinecone refactored classes (setup+service) , Plus Multilingual testing------- 
            //After the initial setup, the immediate query's response is not generated. Comment the third line and run the application again.
            //Now the index will be available and query will be answered.

            // string namespaceName = "profiles";
            // string indexName = "dr-dobric-index";
            // await pineconeSetupService.RunAsync(inputs, indexName, namespaceName); //Uncomment if new index creation setup is required. CHANGE PARAMS ACCORDINGLY
            // string query = "Who is dr dobric?";
            // var pineconeTopKparagraphs = await pineconeService.QueryEmbeddingsAsync(query, indexName, namespaceName, 3, "en");
            // var answer = await textGenerationService.GenerateTextAsync(query, pineconeTopKparagraphs);
            // Console.WriteLine($"\nAnswer: {answer}");



            // -----------------Testing the chatbot------------------
            string namespaceName = "profiles";
            string indexName = "dr-dobric-index";
            await chatbotService.StartChatAsync(indexName, namespaceName);



            // ---------------Testing RagPipeline------------
            // string namespaceName = "profiles";
            // string indexName = "dr-dobric-index";
            // List<string> inputQueries = new()
            // {
            //     "Where does Dr. Dobric live?",
            //     "What company does Dr. Damir Dobric work for?",
            //     "What is the research topic of Dr. Damir's in his Phd?",
            //     "What is Dr. Damir Dobric’s academic background?",
            //     "Where can I find more about Dr. Damir Dobric?"
            // };
            //
            // List<string> groundTruthAnswers = new()
            // {
            //     "Dr. Dobric lives in Frankfurt Rhine-Main Metropolitan Area",
            //     "Damir Dobric works for DAENET GmbH – ACP Digital, a Microsoft Gold Certified Partner.",
            //     "Dr Damir's research topic in Phd is Computational Intelligence",
            //     "Dr Damir Dobic holds a PhD in Computational Intelligence from the University of Plymouth, UK.",
            //     "Damir's LinkedIn profile is www.linkedin.com/in/damirdobric, and his personal website is https://damirdobric.me."
            // };
            // List<string> generatedResponses = await ragPipeline.BatchRetrieveAndGenerateResponsesAsync(inputQueries, indexName, namespaceName, 3);
            // for (int i = 0; i < generatedResponses.Count; i++)
            // {
            //     RagEvaluationResult result = await ragPipeline.EvaluateAccuracy(generatedResponses[i], groundTruthAnswers[i]);
            //     Console.WriteLine($"Query: {inputQueries[i]}");
            //     Console.WriteLine($"Generated Answer: {generatedResponses[i]}");
            //     Console.WriteLine($"Ground truth Answer: {groundTruthAnswers[i]}");
            //     Console.WriteLine($"Accuracy Results:");
            //     Console.WriteLine($"Cosine Similarity: {result.CosineSimilarity}");
            //     Console.WriteLine($"ROUGE-1 Score: {result.Rouge1Score}");
            //     Console.WriteLine($"ROUGE-2 Score: {result.Rouge2Score}");
            // }




            //-------------Word2Vec Testing for phrases and words----
            // string txtFileName = "glove.6B.300d.txt";
            // string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"../../.."));
            // string filePath = Path.Combine(projectRoot, "Datasets", txtFileName);
            // Console.WriteLine($"Loading file: {filePath}");
            //
            // var word2VecService = new Word2VecService(filePath);
            //
            // var vector1 = word2VecService.GetPhraseVector("machine learning revolution in technology"); //case-sensitive
            // var vector2 = word2VecService.GetPhraseVector("law and legal regulations"); //case-sensitive
            //
            // var listVector1 = vector1.ToList();
            // var listVector2 = vector2.ToList();
            //
            // var cosineSimilarity = new CosineSimilarity();
            // var cosineSimilarityVaue = cosineSimilarity.ComputeCosineSimilarity(listVector1, listVector2);
            //
            // // Display the result
            // Console.WriteLine($"Cosine Similarity using word2vec: {cosineSimilarityVaue}");

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

    // var inputs = new List<string>{"Waiter work at the hospital", "hospital", "Airport", "Cat", "Dog" };

    // var inputs = new List<string>
    // {
    //     // Manual 1: Setting up a new email account
    //     // First paragraph in German:
    //     "Um ein neues E-Mail-Konto auf Ihrem mobilen Gerät einzurichten, öffnen Sie zunächst die 'Einstellungen'-App auf Ihrem Telefon und wählen Sie 'Konten' oder 'Passwörter & Konten', je nach Ihrem Betriebssystem. Tippen Sie auf die Option 'Konto hinzufügen' oder 'Neues Konto hinzufügen'. Wählen Sie nun den E-Mail-Anbieter aus der Liste aus, wie z. B. Gmail, Outlook oder Yahoo. Wenn Ihr E-Mail-Anbieter nicht aufgeführt ist, wählen Sie 'Andere', um die Kontodaten manuell einzugeben. Geben Sie Ihre vollständige E-Mail-Adresse ein, z. B. 'benutzername@domain.com', gefolgt von Ihrem E-Mail-Passwort. Wenn erforderlich, geben Sie zusätzliche Einstellungen wie die Mail-Server-Adresse, IMAP/POP-Einstellungen und SMTP-Serverdetails ein. Nachdem die Daten eingegeben wurden, überprüft das System Ihre Anmeldedaten und verbindet sich mit Ihrem E-Mail-Konto.",
    //
    //     // Second paragraph in Chinese:
    //     "在成功添加电子邮件帐户后，返回到“帐户”部分检查您的设置。现在，您可以打开手机上的电子邮件应用程序，它应该会自动与您的电子邮件帐户同步。如果系统提示您设置额外的安全功能，例如双因素身份验证，建议启用这些功能以提高保护级别。一些电子邮件提供商可能会要求您完成其他设置步骤，例如链接到特定的应用程序或启用某些功能，如联系人和日历同步。一旦一切验证并同步，您可以直接从手机发送和接收电子邮件。",
    //
    //     // Third paragraph in English:
    //     "For added security and easy management, you can customize your email account settings. Navigate to the 'Settings' section within the email app and explore options such as 'Notification Preferences', 'Signature Settings', and 'Sync Frequency'. You may also want to organize your inbox by creating folders and labels to better manage incoming emails. Regularly check your inbox and archive unnecessary emails to keep your account tidy. Lastly, consider setting up email forwarding or filters if you need to redirect or sort incoming messages automatically.",
    //
    //     // Manual 2: Calibrating a smart thermostat
    //     // First paragraph in German:
    //     "Die Kalibrierung Ihres Smart-Thermostats ist wichtig, um sicherzustellen, dass Ihr Zuhause eine angenehme Temperatur beibehält und gleichzeitig den Energieverbrauch optimiert. Beginnen Sie damit, sicherzustellen, dass Ihr Thermostat ordnungsgemäß installiert und mit Ihrem WLAN-Netzwerk verbunden ist. Öffnen Sie die Thermostat-App auf Ihrem Telefon oder Tablet und gehen Sie zum Menü 'Einstellungen'. In diesem Menü finden Sie eine Option mit der Bezeichnung 'Kalibrierung' oder 'Temperaturanpassung'. Tippen Sie auf diese Option, um den Kalibrierungsprozess zu starten. Während der Kalibrierung überwacht das Thermostat die Raumtemperatur über einen längeren Zeitraum, in der Regel 24 Stunden, um seine Messungen genau anzupassen. Stellen Sie sicher, dass sich das Thermostat in einem Bereich mit stabiler Luftzirkulation befindet und vermeiden Sie direkte Sonneneinstrahlung oder Zugluft, die die Messungen beeinträchtigen könnten.",
    //
    //     // Second paragraph in Chinese:
    //     "一旦温控器完成了其校准过程，它将自动调整其温度设置，以匹配您期望的舒适度。这时，您可以通过应用程序或直接在设备上访问您的温控器界面，根据个人偏好调整温度。许多现代温控器还提供像地理围栏等功能，该功能在您在家或外出时自动调整温度，以节省能源。为了获得更个性化的体验，您可以设置计划，以便在一天中的特定时间调整温度，确保系统高效工作并与您的日常生活保持一致。",
    //
    //     // Third paragraph in English:
    //     "In addition to calibration, consider placing your thermostat in an optimal location for the most accurate temperature reading. Avoid placing it near doors, windows, or vents, as these areas may cause incorrect temperature readings due to drafts. If your thermostat has additional features, like humidity sensing, make sure that these settings are enabled to further enhance comfort. Remember to periodically check the thermostat app for updates to ensure you're taking full advantage of the latest features and optimizations. Some models also allow you to monitor energy usage and set reminders to maintain your heating and cooling system, helping you save both money and energy in the long run.",
    //
    //     // Manual 3: Pairing and using wireless headphones
    //     // First paragraph in German:
    //     "Das Koppeln und Verwenden von kabellosen Kopfhörern mit Ihrem Smartphone ist ein unkomplizierter Prozess, der Ihre Audioerfahrung verbessert. Stellen Sie sicher, dass Ihre kabellosen Kopfhörer vollständig aufgeladen sind. Schalten Sie sie ein, indem Sie den Einschaltknopf gedrückt halten, bis die LED-Leuchte blinkt, was darauf hinweist, dass sie sich im Pairing-Modus befinden. Öffnen Sie auf Ihrem Smartphone die 'Einstellungen'-App und wählen Sie 'Bluetooth'. Aktivieren Sie Bluetooth und warten Sie, bis Ihr Telefon nach verfügbaren Geräten sucht. In der Liste der Geräte finden Sie Ihre Kopfhörer und wählen Sie diese aus. Sobald Sie ausgewählt haben, wird auf Ihrem Telefon eine Koppelanfrage angezeigt; bestätigen Sie die Kopplung, indem Sie auf 'Ja' oder 'OK' tippen. Die Kopfhörer werden dann verbunden und Sie sollten einen Bestätigungston hören.",
    //
    //     // Second paragraph in Chinese:
    //     "现在您的无线耳机已连接，您可以直接在耳机上或使用智能手机调整音量。大多数无线耳机具有附加功能，如噪声取消或触摸敏感控制，这些可以通过耳机的配套应用程序或设置启用或自定义。为了增强音频体验，可以考虑在智能手机上调整均衡器设置，以根据您的偏好调整声音。您还可以通过播放歌曲或拨打电话来测试连接，确保音频质量和麦克风的正常工作。一些耳机支持连接多个设备，您可以通过在蓝牙设置中切换连接来无缝切换设备。",
    //
    //     // Third paragraph in English:
    //     "To disconnect the wireless headphones, simply turn off Bluetooth on your smartphone or power off the headphones themselves. If you're planning to use the headphones with another device, repeat the pairing process by following the same steps. It’s important to store your headphones properly when not in use to prevent any physical damage. Most headphones come with a carrying case, which should be used for protection. For longer battery life, remember to turn off the headphones when you're done using them. If your headphones support software updates, make sure to regularly check for any available updates, as these can improve the performance and introduce new features to your device."
    // };

    List<string> inputs = new List<string>
    {
        "Dr. Damir Dobric CEO, Lead Software Architect @ daenet | Microsoft AI MVP, Microsoft Regional Director Frankfurt Rhine-Main Metropolitan Area.",
        "Summary: CEO and Lead Architect of DAENET GmbH – ACP Digital, Microsoft's long-term Gold Certified Partner and a leading technology integrator specialized in software technologies, with a strong focus on Cloud Computing, IoT, and Machine Learning. Damir Dobric is a highly skilled and experienced Lead Software Architect at DAENET, a company specializing in delivering innovative software solutions and consulting services. With a strong background in software development, Damir specializes in various areas such as cloud computing, IoT, and artificial intelligence.",
        "In addition to his role at DAENET, Damir is also a Microsoft Most Valuable Professional (MVP). The Microsoft MVP Award is a prestigious recognition given to exceptional technical experts who are passionate about sharing their knowledge and experiences with others. As an MVP, Damir is part of an elite group of professionals known as Microsoft Regional Directors who actively contribute to the Microsoft community by offering guidance, support, and expertise in various Microsoft technologies. Damir's commitment to excellence in software development and his dedication to helping others have earned him a reputation as a thought leader in the industry. His contributions to the Microsoft community and his work as a Lead Software Architect at DAENET showcase his expertise and passion for technology. With a keen eye for innovation and a deep understanding of cutting-edge technologies,",
        "Damir Dobric is a valuable asset to both DAENET, ACP and the broader Microsoft ecosystem. His work continues to inspire and support other professionals in their pursuit of technical excellence and innovation. He serves as an external professor for Software Engineering and Cloud Computing at the Frankfurt University of Applied Sciences.",
        "Damir holds a PhD in Computational Intelligence from the University of Plymouth, UK. Experience daenet CEO, Lead Software Architect 1998 - Present (27 years) Frankfurt Am Main Area, Germany Microsoft Regional Director, Most Valuable Professional and Partner Technology Solution Professional for Microsoft Azure. Education University of Plymouth PhD Computational Intelligence, Artificial Intelligence",
        "Contact www.linkedin.com/in/damirdobric (LinkedIn) https://damirdobric.me/ (Personal Website). Twitter: @ddobric",
        "Top Skills Windows Azure .NET Cloud Applications.",
        "Publications: 1) Artifficial Intelligence: Ready, Steady Gp Blog DEVELOPERS.DE, 2) Azure Best Practices: Running th code on a memory limit Load Balancers in Microsoft Azure cloud platform. 3) Why the cortical algorithm does need a baby phase?"
    };

}