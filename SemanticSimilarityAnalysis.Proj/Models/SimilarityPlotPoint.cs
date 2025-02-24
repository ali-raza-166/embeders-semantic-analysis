namespace SemanticSimilarityAnalysis.Proj.Models
{
    /// <summary>
    /// Represents a data point for plotting similarity values in a 3D coordinate system.
    /// </summary>
    public class SimilarityPlotPoint
    {
        public string Label { get; set; }
        public double SimilarityWithInput1 { get; set; }
        public double SimilarityWithInput2 { get; set; }
        public double SimilarityWithInput3 { get; set; }

        // Map the similarity values to a 3D coordinate system.
        public double X => SimilarityWithInput1;
        public double Y => SimilarityWithInput2;
        public double Z => SimilarityWithInput3;

        // Constructor
        public SimilarityPlotPoint(string label, double similarity1, double similarity2, double similarity3)
        {
            Label = label;
            SimilarityWithInput1 = similarity1;
            SimilarityWithInput2 = similarity2;
            SimilarityWithInput3 = similarity3;
        }
    }
}