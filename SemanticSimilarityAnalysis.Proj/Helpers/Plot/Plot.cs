using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using SemanticSimilarityAnalysis.Proj.Models;

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
    public SimilarityPlotPoint(string label, double similarity1, double similarity2 = 0, double similarity3 = 0)
        {
            Label = label;
            SimilarityWithInput1 = similarity1;
            SimilarityWithInput2 = similarity2;
            SimilarityWithInput3 = similarity3;
        }
    }
}

public class SimilarityPlotter
{
    public void Plot1D(List<SimilarityPlotPoint> points)
    {
        var model = new PlotModel { Title = "1D Cosine Similarity" };
        var scatterSeries = new ScatterSeries();

        foreach (var point in points)
        {
            scatterSeries.Points.Add(new ScatterPoint(point.X, 0));
        }
        model.Series.Add(scatterSeries);
        DisplayPlot(model);
    }
}
