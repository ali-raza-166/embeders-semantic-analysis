namespace SemanticSimilarityAnalysis.Proj.Services;

public class ChatbotService
{
    private readonly PineconeService _pineconeService;
    private readonly OpenAiTextGenerationService _textGenerationService;

    public ChatbotService(PineconeService pineconeService, OpenAiTextGenerationService textGenerationService)
    {
        _pineconeService = pineconeService;
        _textGenerationService = textGenerationService;
    }

    public async Task StartChatAsync(string indexName, string namespaceName)
    {
        Console.WriteLine("Chatbot started. Type 'exit' to quit.");

        while (true)
        {
            // Get user input
            Console.Write("You: ");
            string query = Console.ReadLine()!;
            if (query.ToLower() == "exit") break;

            // Query Pinecone for relevant paragraphs
            var pineconeTopKparagraphs = await _pineconeService.QueryEmbeddingsAsync(query, indexName, namespaceName, 3);

            // Generate response using the text generation model
            var answer = await _textGenerationService.GenerateTextAsync(query, pineconeTopKparagraphs);

            Console.WriteLine($"Chatbot: {answer}");
        }
    }
}
