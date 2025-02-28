using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;

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
    }
}
