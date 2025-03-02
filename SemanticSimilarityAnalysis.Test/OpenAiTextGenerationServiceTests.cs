using OpenAI.Chat;
using SemanticSimilarityAnalysis.Proj.Services;

namespace SemanticSimilarityAnalysis.Test;

[TestClass]
public class OpenAiTextGenerationServiceTests
{
    private OpenAiTextGenerationService _textGenerationService;

    [TestInitialize]
    public void Setup()
    {
        // Setup with real OpenAI API key from environment variable or configuration
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentException("API key must be provided for testing");
        }
        var chatClient = new ChatClient("gpt-4", apiKey);
        _textGenerationService = new OpenAiTextGenerationService(chatClient);
    }

    [TestMethod]
    public async Task GenerateTextAsync_ShouldReturnValidResponse_ForValidInput()
    {
        // Arrange
        var query = "What is the capital of France?";
        var paragraphs = new List<string>
        {
            "France is a country in Western Europe.",
            "Its capital is Paris, which is known for its art, fashion, and culture."
        };

        // Act
        var result = await _textGenerationService.GenerateTextAsync(query, paragraphs);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("Paris"), "Response should contain 'Paris'");
    }
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task GenerateTextAsync_ShouldThrowArgumentNullException_WhenApiKeyIsMissing()
    {
        // Arrange
        string invalidApiKey = string.Empty; // Invalid API key or missing key
        var chatClient = new ChatClient("gpt-4", invalidApiKey);

        var service = new OpenAiTextGenerationService(chatClient);

        // Act
        await service.GenerateTextAsync("Some query", new List<string> { "Some paragraph" });

        // Assert handled by ExpectedException
    }
    [TestMethod]
    public async Task GenerateTextAsync_ShouldHandleInvalidQueryGracefully()
    {
        // Arrange
        var query = "Invalid query that OpenAI may not understand!";
        var paragraphs = new List<string>
        {
            "Paragraph 1: Some valid content.",
            "Paragraph 2: Another valid content."
        };

        // Act
        var result = await _textGenerationService.GenerateTextAsync(query, paragraphs);

        // Assert
        Assert.IsNotNull(result); 
    }


    

}