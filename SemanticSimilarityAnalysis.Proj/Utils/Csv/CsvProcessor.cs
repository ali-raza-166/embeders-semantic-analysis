using SemanticSimilarityAnalysis.Proj.Models;
using SemanticSimilarityAnalysis.Proj.Services;
using System.Text.Json;

namespace SemanticSimilarityAnalysis.Proj.Utils
{
    public class CsvProcessor
    {
        private readonly OpenAiEmbeddingService _embeddingService;
        private readonly string _jsonFilePath;

        public CsvProcessor(OpenAiEmbeddingService embeddingService, string jsonFilePath)
        {
            _embeddingService = embeddingService ?? throw new ArgumentNullException(nameof(embeddingService));
            _jsonFilePath = jsonFilePath ?? throw new ArgumentNullException(nameof(jsonFilePath));
        }

        public async Task ProcessAndGenerateEmbeddingsAsync(List<MovieModel> movies)
        {
            if (movies == null || movies.Count == 0)
                throw new ArgumentException("Movie list cannot be null or empty.", nameof(movies));

            var titles = movies.ConvertAll(m => m.Title);
            var overviews = movies.ConvertAll(m => m.Overview);

            var titleEmbeddings = await _embeddingService.CreateEmbeddingsAsync(titles);
            var overviewEmbeddings = await _embeddingService.CreateEmbeddingsAsync(overviews);

            for (int i = 0; i < movies.Count; i++)
            {
                movies[i].TitleEmbedding = titleEmbeddings[i].Vector;
                movies[i].OverviewEmbedding = overviewEmbeddings[i].Vector;
            }

            var jsonData = JsonSerializer.Serialize(movies, new JsonSerializerOptions { WriteIndented = true });

            var outputDirectory = Path.GetDirectoryName(_jsonFilePath);
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            await File.WriteAllTextAsync(_jsonFilePath, jsonData);
        }
    }
}
