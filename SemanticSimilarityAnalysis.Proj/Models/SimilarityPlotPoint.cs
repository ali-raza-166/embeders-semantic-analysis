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
        public float SimilarityWithInput1 { get; set; }
        public float SimilarityWithInput2 { get; set; }
        public float SimilarityWithInput3 { get; set; }

        public float X => SimilarityWithInput1;
        public float Y => SimilarityWithInput2;
        public float Z => SimilarityWithInput3;

        public SimilarityPlotPoint(string label, float similarity1, float similarity2, float similarity3)
        {
            Label = label;
            SimilarityWithInput1 = similarity1;
            SimilarityWithInput2 = similarity2;
            SimilarityWithInput3 = similarity3;
        }
    }
}