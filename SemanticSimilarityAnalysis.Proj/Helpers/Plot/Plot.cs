using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using SemanticSimilarityAnalysis.Proj.Models;
using static System.Net.Mime.MediaTypeNames;

namespace SemanticSimilarityAnalysis.Proj.Models
{
    /// Represents a data point for plotting similarity values.
    public class SimilarityPlotPoint
    {
        /// Label representing the movie or item being compared.
        public string Label { get; set; }
        /// Cosine similarity score with the first input parameter.
        public double SimilarityWithInput1 { get; set; }
        /// Cosine similarity score with the second input parameter.
        public double SimilarityWithInput2 { get; set; }
        /// Cosine similarity score with the third input parameter.
        public double SimilarityWithInput3 { get; set; }

        // Map the similarity values to coordinate system.
        public double X => SimilarityWithInput1;
        public double Y => SimilarityWithInput2;
        public double Z => SimilarityWithInput3;
    }

    /// Initializes a new instance of the SimilarityPlotPoint class. 
    /// Parameter Names
    /// "label"The label or name of the data point.
    /// "similarity1"Similarity score with the first input.
    /// "similarity2"Similarity score with the second input (default is 0).
    /// "similarity3"Similarity score with the third input (default is 0).
    
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
    /// Plots a 1D cosine similarity graph. 
    /// Parameter Names
    /// "points" A list of similarity plot points.
    public void Plot1D(List<SimilarityPlotPoint> points)
    {
        // Create a new plot model with a title.
        var model = new PlotModel { Title = "1D Cosine Similarity" };
        var scatterSeries = new ScatterSeries();
        // Add each data point to the scatter series.
        foreach (var point in points)
        {
            scatterSeries.Points.Add(new ScatterPoint(point.X, 0));
        }
        // Add the series to the model and display the plot.
        model.Series.Add(scatterSeries);
        DisplayPlot(model);
    }


    /// Plots a 2D cosine similarity graph. 
    /// Parameter Names
    /// "points" A list of similarity plot points.
    public void Plot2D(List<SimilarityPlotPoint> points)
    {
        // Create a new plot model with a title.
        var model = new PlotModel { Title = "2D Cosine Similarity" };
        var scatterSeries = new ScatterSeries();
        // Add each data point using X and Y values.

        foreach (var point in points)
        {
            scatterSeries.Points.Add(new ScatterPoint(point.X, point.Y));
        }
        // Add the series to the model and display the plot.
        model.Series.Add(scatterSeries);
        DisplayPlot(model);
    }


    /// Plots a 3D cosine similarity graph (Placeholder implementation). 
    /// Parameter Names
    /// "points" A list of similarity plot points. 

    public void Plot3D(List<SimilarityPlotPoint> points)
    {
        // 3D plotting is not implemented due to library constraints.
        Console.WriteLine("3D plotting requires additional library support.");
    }
}
/// Displays the given plot using a Windows Forms window.
/// Parameter Names
/// "model" The plot model to be displayed.
private void DisplayPlot(PlotModel model)
{
    // Enable Windows Forms visual styles for better UI rendering.
    Application.EnableVisualStyles();
    // Create a new form window.
    Form form = new Form { Width = 800, Height = 600 };
    // Create a plot view and set its model.
    var plotView = new OxyPlot.WindowsForms.PlotView { Model = model, Dock = DockStyle.Fill };
    // Add the plot view to the form.
    form.Controls.Add(plotView);
    // Display the form with the plot.
    Application.Run(form);
}

