using MathNet.Numerics.LinearAlgebra;
using SemanticSimilarityAnalysis.Proj.Helpers;
using SemanticSimilarityAnalysis.Proj.Helpers.Csv;
using SemanticSimilarityAnalysis.Proj.Services;

namespace SemanticSimilarityAnalysis.Proj.Pipelines
{
    /// <summary>
    /// This class executes a complete pipeline for OpenAI embeddings, including:
    /// 1. Generating OpenAI embeddings for a given list of words or phrases.
    /// 2. Applying dimensionality reduction techniques (PCA and t-SNE).
    /// 3. Scaling the reduced-dimensional data for better visualization.
    /// 4. Saving the reduced embeddings in a CSV file for further analysis.
    /// 5. Using a Python script to generate scatter plots from the CSV data.
    /// The pipeline automatically handles input validation and file storage.
    /// </summary>
    public class OpenAiEmbeddingsDimReductionAndPlotting
    {
        private readonly OpenAiEmbeddingService _embeddingService;
        private readonly DimensionalityReductionService _dimensionalityReductionService;
        private readonly CSVHelper _csvHelper;
        private readonly CSharpPythonConnector _pythonConnector;

        /// <summary>
        /// Initializes the OpenAI embedding pipeline with required services.
        /// </summary>
        /// <param name="embeddingService">Generates embeddings using OpenAI.</param>
        /// <param name="dimensionalityReductionService">Handles PCA and t-SNE dimensionality reduction.</param>
        /// <param name="csvHelper">Manages exporting reduced data to CSV format.</param>
        /// <param name="pythonConnector">Runs Python scripts for visualization.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the dependencies are null.</exception>
        public OpenAiEmbeddingsDimReductionAndPlotting(
            OpenAiEmbeddingService embeddingService,
            DimensionalityReductionService dimensionalityReductionService,
            CSVHelper csvHelper,
            CSharpPythonConnector pythonConnector)
        {
            _embeddingService = embeddingService ?? throw new ArgumentNullException(nameof(embeddingService));
            _dimensionalityReductionService = dimensionalityReductionService ?? throw new ArgumentNullException(nameof(dimensionalityReductionService));
            _csvHelper = csvHelper ?? throw new ArgumentNullException(nameof(csvHelper));
            _pythonConnector = pythonConnector ?? throw new ArgumentNullException(nameof(pythonConnector));
        }

        /// <summary>
        /// Executes the OpenAI embedding pipeline, including dimensionality reduction and visualization.
        /// </summary>
        /// <param name="inputs">A list of words or phrases to process.</param>
        /// <exception cref="ArgumentException">Thrown if the input list is empty.</exception>
        public async Task RunPipelineAsync(List<string> inputs)
        {
            if (inputs == null || !inputs.Any())
            {
                throw new ArgumentException("The input list cannot be empty. Please provide a list of words or phrases for embedding generation.", nameof(inputs));
            }

            var reductionMethods = new List<(string methodName, Func<List<List<float>>, Matrix<double>> reductionMethod)>
            {
                ("pca", embeddings => _dimensionalityReductionService.PerformPca(embeddings)),
                ("tsne", embeddings => _dimensionalityReductionService.ReduceDimensionsUsingTsne(embeddings))
            };

            var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../.."));
            foreach (var (methodName, reductionMethod) in reductionMethods)
            {
                var csvFileName = $"openai_{methodName}_reduced.csv";
                var csvFilePath = Path.Combine(projectRoot, "Outputs", "CSV", csvFileName);
                var plotFileName = $"openai_{methodName}_scatterplot.png";
                var plotFilePath = Path.Combine(projectRoot, "Outputs", "Plots", plotFileName);

                var existingInputs = LoadInputsFromCsv(csvFilePath);
                if (existingInputs.SequenceEqual(inputs))
                {
                    Console.WriteLine($"Inputs have not changed. Skipping embedding creation. Proceeding to plot {methodName}...");
                    _pythonConnector.PlotScatterFromCsv(csvFilePath, plotFilePath);
                    continue;
                }

                Console.WriteLine($"Generating embeddings using OpenAI for {methodName}...");
                var embeddings = await _embeddingService.CreateEmbeddingsAsync(inputs);
                var validEmbeddings = embeddings.Select(e => e.Values).ToList();

                if (!validEmbeddings.Any())
                {
                    Console.WriteLine("No valid embeddings found. Skipping pipeline.");
                    continue;
                }

                var reducedDataMatrix = reductionMethod(validEmbeddings);
                var scaledData = _dimensionalityReductionService.MinMaxScaleData(reducedDataMatrix);
                
                ExportReducedDimensionalityData(scaledData, inputs, csvFileName);
                _pythonConnector.PlotScatterFromCsv(csvFilePath, plotFilePath);
            }

            Console.WriteLine("✅ OpenAI Embeddings Pipeline completed successfully!");
        }

        /// <summary>
        /// Saves the reduced-dimensional data to a CSV file.
        /// </summary>
        /// <param name="scaledData">Matrix containing reduced embeddings.</param>
        /// <param name="inputs">List of words or phrases corresponding to embeddings.</param>
        /// <param name="outputCsvFileName">Filename for the output CSV file.</param>
        private void ExportReducedDimensionalityData(Matrix<double> scaledData, List<string> inputs, string outputCsvFileName)
        {
            _csvHelper.ExportReducedDimensionalityData(scaledData, inputs, outputCsvFileName);
        }

        /// <summary>
        /// Loads the inputs (words or phrases) from an existing CSV file, if available.
        /// </summary>
        /// <param name="existingCsvFilePath">Path to the existing CSV file with previously processed data.</param>
        /// <returns>A list of inputs from the CSV file.</returns>
        private List<string> LoadInputsFromCsv(string existingCsvFilePath)
        {
            var inputs = new List<string>();
            if (!File.Exists(existingCsvFilePath)) return inputs;

            using (var reader = new StreamReader(existingCsvFilePath))
            {
                reader.ReadLine(); // Skip header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var columns = line?.Split(',');
                    if (columns?.Length >= 1 && !string.IsNullOrWhiteSpace(columns[0]))
                    {
                        inputs.Add(columns[0].Trim());
                    }
                }
            }

            return inputs;
        }
    }
}
