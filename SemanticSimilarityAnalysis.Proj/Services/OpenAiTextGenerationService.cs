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
            Console.WriteLine($"Context: {contextText}");

            var prompt =
                $"I will give you text paragraphs and a question. You must answer the question in the light of the paragraph I provided as context. The paragrpahs can be in different languages. Always answer back in the language in which the question is asked. Here are paragraphs:\n\n{contextText}\n\nBased on the above information, answer the following question:\n{query} \n Keep in mind, always use the same language of answer as the language of query. You mind recieve paragraphs in different language but use them to create the answer";

            ChatCompletion completion = await _openAiClient.CompleteChatAsync(prompt);
            return completion.Content[0].Text;
        }
    }
}
