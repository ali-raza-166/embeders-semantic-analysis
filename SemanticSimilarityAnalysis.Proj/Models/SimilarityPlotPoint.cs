using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticSimilarityAnalysis.Proj.Models
{
    public class SimilarityPlotPoint
    {
        public string Label { get; set; }
        public double SimilarityWithInput1 { get; set; }
        public double SimilarityWithInput2 { get; set; }
        public double SimilarityWithInput3 { get; set; }

        public double X => SimilarityWithInput1;
        public double Y => SimilarityWithInput2;
        public double Z => SimilarityWithInput3;

        public SimilarityPlotPoint(string label, double similarity1, double similarity2, double similarity3)
        {
            Label = label;
            SimilarityWithInput1 = similarity1;
            SimilarityWithInput2 = similarity2;
            SimilarityWithInput3 = similarity3;
        }
    }
}