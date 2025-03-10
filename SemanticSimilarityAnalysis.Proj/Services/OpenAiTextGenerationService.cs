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

             var prompt = $@"I will give you text paragraphs and a question. You must answer the question in the light of the 
                             paragraph I provided as context. The paragraphs can be in different languages. Always determine
                             the language of the query and keep the same langauge for you answer. 

                             Here is the Context:

                             {contextText}

                             Based on the above information, answer the following question:
                             {query}";
//             var prompt = $@"You will receive a text paragraph (context) and a question. Your task is to answer the question **only using the information from the context** while maintaining consistency in language.
//                 
//                 **Instructions:**
//                 1. **Identify the language of the query.** Your response **must** be in the same language as the query.
//                 2. **Do not introduce external knowledge.** Base your answer strictly on the provided context.
//                 3. **Be precise and clear.** Avoid vague or overly broad responses.
//                 4. **If the context does not contain enough information, clearly state that.** Do not guess.
//
//                 **Context:**  
//                 {contextText}  
//
//                 **Question:**  
//                 {query}  
//  
//                 **Provide your answer below, strictly following the instructions:**";
            
            ChatCompletion completion = await _openAiClient.CompleteChatAsync(prompt);
            return completion.Content[0].Text;
        }
    }
}
