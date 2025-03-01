using OpenAI;
using OpenAI.Chat;
using SemanticSimilarityAnalysis.Proj.Interfaces;

namespace SemanticSimilarityAnalysis.Proj.Services
{
    public class OpenAiTextGenerationService: ITextGenerator
    {
        private readonly ChatClient _openAiClient;

        public OpenAiTextGenerationService(ChatClient openAiClient)
        {
            _openAiClient = openAiClient ?? throw new ArgumentNullException(nameof(openAiClient));;
        }
        public async Task<string> GenerateTextAsync( string query, List<string> paragraphs)
        {
            var contextText = string.Join("\n\n", paragraphs);

            var prompt =
                $"Here are paragraphs:\n\n{contextText}\n\nBased on the above information, answer the following question:\n{query}";

            ChatCompletion completion = await _openAiClient.CompleteChatAsync(prompt);
            return completion.Content[0].Text;
        }
    }
}
