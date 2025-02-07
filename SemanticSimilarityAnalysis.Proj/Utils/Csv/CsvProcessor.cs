using SemanticSimilarityAnalysis.Proj.Models; // Importing the MovieModel class
using SemanticSimilarityAnalysis.Proj.Services; // Importing the OpenAiEmbeddingService class
using System.Text.Json; // Importing JSON serialization utilities

namespace SemanticSimilarityAnalysis.Proj.Utils
{
    // Class responsible for processing movie data and generating semantic embeddings.
    public class CsvProcessor
    {
        private readonly OpenAiEmbeddingService _embeddingService;
        private readonly string _jsonFilePath;

        public CsvProcessor(OpenAiEmbeddingService embeddingService, string jsonFilePath)
        {
            _embeddingService = embeddingService ?? throw new ArgumentNullException(nameof(embeddingService));
            _jsonFilePath = jsonFilePath ?? throw new ArgumentNullException(nameof(jsonFilePath));
        }
        // Method responsible for processing the movie list and generating embeddings for titles and overviews asynchronously.
        public async Task ProcessAndGenerateEmbeddingsAsync(List<MovieModel> movies)
        {
            if (movies == null || movies.Count == 0)
                throw new ArgumentException("Movie list cannot be null or empty.", nameof(movies));

            var titles = movies.ConvertAll(m => m.Title);
            var overviews = movies.ConvertAll(m => m.Overview);

            var titleEmbeddings = await _embeddingService.CreateEmbeddingsAsync(titles);
            var overviewEmbeddings = await _embeddingService.CreateEmbeddingsAsync(overviews);
            // Assigning the generated embeddings to the corresponding movie models.
            for (int i = 0; i < movies.Count; i++)
            {
                movies[i].TitleEmbedding = titleEmbeddings[i].Vector;
                movies[i].OverviewEmbedding = overviewEmbeddings[i].Vector;
            }
            // Serialize the 'movies' list into a formatted JSON string with indentation for readability.
            var jsonData = JsonSerializer.Serialize(movies, new JsonSerializerOptions { WriteIndented = true });

            var outputDirectory = Path.GetDirectoryName(_jsonFilePath);
            // Check if the directory exists; if not, create it to ensure the file can be saved.
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            await File.WriteAllTextAsync(_jsonFilePath, jsonData);
        }
    }
}
