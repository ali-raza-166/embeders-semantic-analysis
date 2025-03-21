namespace SemanticSimilarityAnalysis.Proj.Services;

/// <summary>
/// Provides chatbot functionality by integrating Pinecone for retrieval-augmented generation (RAG) 
/// and OpenAI for text generation. The chatbot continuously interacts with users until they exit.
/// </summary>
public class ChatbotService
{
    private readonly PineconeService _pineconeService;
    private readonly OpenAiTextGenerationService _textGenerationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatbotService"/> class.
    /// </summary>
    /// <param name="pineconeService">Service for querying Pinecone vector database.</param>
    /// <param name="textGenerationService">Service for generating text responses using OpenAI.</param>
    public ChatbotService(PineconeService pineconeService, OpenAiTextGenerationService textGenerationService)
    {
        _pineconeService = pineconeService;
        _textGenerationService = textGenerationService;
    }

    /// <summary>
    /// Starts an interactive chatbot session, allowing the user to ask questions.
    /// The chatbot retrieves relevant information from Pinecone and generates responses using OpenAI.
    /// </summary>
    /// <param name="indexName">The name of the Pinecone index.</param>
    /// <param name="namespaceName">The namespace within the Pinecone index.</param>
    public async Task StartChatAsync(string indexName, string namespaceName)
    {
        Console.WriteLine("Chatbot started. Type 'exit' to quit.");

        while (true)
        {
            Console.Write("You: ");
            string query = Console.ReadLine()!;
            if (query.ToLower() == "exit") break;
           
            var pineconeTopKparagraphs = await _pineconeService.QueryEmbeddingsAsync(query, indexName, namespaceName, 3);
            var answer = await _textGenerationService.GenerateTextAsync(query, pineconeTopKparagraphs);

            Console.WriteLine($"Chatbot: {answer}");
        }
    }
}
