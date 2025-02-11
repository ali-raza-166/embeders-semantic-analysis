namespace SemanticSimilarityAnalysis.Proj.Models
{
    using System.Collections.Generic;

    public class MovieModel
    {
        public MovieModel(string title, string genre, string overview, List<float> titleEmbedding, List<float> overviewEmbedding)
        {
            Title = title;
            Genre = genre;
            Overview = overview;
            TitleEmbedding = titleEmbedding;
            OverviewEmbedding = overviewEmbedding;
        }

        public string Title { get; set; }

        public string Genre { get; set; }
        public string Overview { get; set; }
        public List<float> TitleEmbedding { get; set; }
        public List<float> OverviewEmbedding { get; set; }
    }
}
