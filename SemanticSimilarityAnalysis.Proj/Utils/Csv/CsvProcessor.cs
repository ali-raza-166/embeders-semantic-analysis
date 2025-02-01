using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SemanticSimilarityAnalysis.Proj.Services;

namespace SemanticSimilarityAnalysis.Proj.Utils
{
    public class CsvProcessor
    {
        private readonly OpenAiEmbeddingService _embeddingService;
        public CsvProcessor()
        {
            _embeddingService = embeddingService ?? throw new ArgumentNullException(nameof(embeddingService));
        }
    }
}
