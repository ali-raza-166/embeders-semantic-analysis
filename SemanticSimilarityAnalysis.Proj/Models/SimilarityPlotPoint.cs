namespace SemanticSimilarityAnalysis.Proj.Models
{
    /// <summary>
    /// Represents a point in the similarity plot. It contains a label and a dictionary of similarity values between the selected record's attribute embeddings and input embeddings.
    /// </summary>
    public class SimilarityPlotPoint
    {

        /// Dictionary to store similarity values between the selected record's attribute embeddings and input embeddings.
        /// 
        /// Key: Input ID (e.g., "Input1", "Input2", "Input3").
        /// Value: Similarity value between the embeddings of the input and the selected record's attribute.
        public Dictionary<string, double> Similarities { get; set; }
        public string Label { get; set; }

        public SimilarityPlotPoint(string label, Dictionary<string, double> similarities)
        {
            Label = label;
            Similarities = similarities;
        }
    }
}