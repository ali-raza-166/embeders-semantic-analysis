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
        public async Task ProcessAndGenerateEmbeddingsAsync(List<MovieData> movies)
        {
            var titles = movies.ConvertAll(m => m.Title);
            var overviews = movies.ConvertAll(m => m.Overview);
            var titleEmbeddings = await _embeddingService.CreateEmbeddingsAsync(titles);
            var overviewEmbeddings = await _embeddingService.CreateEmbeddingsAsync(overviews);

        }


    }
}
