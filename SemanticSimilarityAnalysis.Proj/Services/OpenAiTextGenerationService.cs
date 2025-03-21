using OpenAI;
using OpenAI.Chat;
using SemanticSimilarityAnalysis.Proj.Interfaces;

namespace SemanticSimilarityAnalysis.Proj.Services
{
    /// <summary>
    /// Service for generating text responses using OpenAI's Chat API.
    /// </summary>
    /// <remarks>
    /// This service takes a query and relevant context paragraphs, then generates a response 
    /// based on the provided information while preserving the query's original language.
    /// </remarks>
    public class OpenAiTextGenerationService: ITextGenerator
    {
        private readonly ChatClient _openAiClient;
        

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAiTextGenerationService"/> class.
        /// </summary>
        /// <param name="openAiClient">An instance of OpenAI's chat client.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="openAiClient"/> is null.</exception>
        public OpenAiTextGenerationService(ChatClient openAiClient)
        {
            _openAiClient = openAiClient ?? throw new ArgumentNullException(nameof(openAiClient));;
        }
        /// <summary>
        /// Generates a text response based on a query and relevant context paragraphs.
        /// </summary>
        /// <param name="query">The user's query for which a response is generated.</param>
        /// <param name="paragraphs">A list of text paragraphs providing contextual information.</param>
        /// <returns>A generated response as a string.</returns>
        /// <remarks>
        /// The method constructs a prompt combining the provided context and query, ensuring the response maintains
        /// the language of the query. It then sends the prompt to OpenAI’s Chat API for response generation.
        /// </remarks>
        public async Task<string> GenerateTextAsync( string query, List<string> paragraphs)
        {
            var contextText = string.Join("\n\n", paragraphs);
            Console.WriteLine($"Context: {contextText}");

             var prompt = $@"I will give you text paragraphs and a question. You must answer the question in the light of the 
                             paragraph I provided as context. The paragraphs can be in different languages. Always determine
                             the language of the query and keep the same langauge for you answer. 

                             Here is the Context:

                             {contextText}

                             Based on the above information, answer the following question:
                             {query}";
            
            ChatCompletion completion = await _openAiClient.CompleteChatAsync(prompt);
            return completion.Content[0].Text;
        }
    }
}
