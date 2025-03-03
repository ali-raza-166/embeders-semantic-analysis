//using SemanticSimilarityAnalysis.Proj.Models;
//using static System.Net.Mime.MediaTypeNames;

//public class PlotHelper
//{
//    /// Plots a 1D cosine similarity graph. 
//    /// Parameter Names
//    /// "points" A list of similarity plot points.
//    public void Plot1D(List<SimilarityPlotPoint> points)
//    {
//        // Create a new plot model with a title.
//        var model = new PlotModel { Title = "1D Cosine Similarity" };
//        var scatterSeries = new ScatterSeries();
//        // Add each data point to the scatter series.
//        foreach (var point in points)
//        {
//            scatterSeries.Points.Add(new ScatterPoint(point.X, 0));
//        }
//        // Add the series to the model and display the plot.
//        model.Series.Add(scatterSeries);
//        DisplayPlot(model);
//    }


//    /// Plots a 2D cosine similarity graph. 
//    /// Parameter Names
//    /// "points" A list of similarity plot points.
//    public void Plot2D(List<SimilarityPlotPoint> points)
//    {
//        // Create a new plot model with a title.
//        var model = new PlotModel { Title = "2D Cosine Similarity" };
//        var scatterSeries = new ScatterSeries();
//        // Add each data point using X and Y values.

//        foreach (var point in points)
//        {
//            scatterSeries.Points.Add(new ScatterPoint(point.X, point.Y));
//        }
//        // Add the series to the model and display the plot.
//        model.Series.Add(scatterSeries);
//        DisplayPlot(model);
//    }


//    /// Plots a 3D cosine similarity graph (Placeholder implementation). 
//    /// Parameter Names
//    /// "points" A list of similarity plot points. 

//    public void Plot3D(List<SimilarityPlotPoint> points)
//    {
//        // 3D plotting is not implemented due to library constraints.
//        Console.WriteLine("3D plotting requires additional library support.");
//    }
//}
///// Displays the given plot using a Windows Forms window.
///// Parameter Names
///// "model" The plot model to be displayed.
//private void DisplayPlot(PlotModel model)
//{
//    // Enable Windows Forms visual styles for better UI rendering.
//    Application.EnableVisualStyles();
//    // Create a new form window.
//    Form form = new Form { Width = 800, Height = 600 };
//    // Create a plot view and set its model.
//    var plotView = new OxyPlot.WindowsForms.PlotView { Model = model, Dock = DockStyle.Fill };
//    // Add the plot view to the form.
//    form.Controls.Add(plotView);
//    // Display the form with the plot.
//    Application.Run(form);
//}

