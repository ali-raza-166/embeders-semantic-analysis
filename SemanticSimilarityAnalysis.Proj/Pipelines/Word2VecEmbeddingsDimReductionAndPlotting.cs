using MathNet.Numerics.LinearAlgebra;
using SemanticSimilarityAnalysis.Proj.Helpers;
using SemanticSimilarityAnalysis.Proj.Helpers.Csv;
using SemanticSimilarityAnalysis.Proj.Services;

namespace SemanticSimilarityAnalysis.Proj.Pipelines
{
    /// <summary>
    /// This class executes a complete pipeline for Word2Vec embeddings, including:
    /// 1. Generating Word2Vec embeddings for a given list of words or phrases.
    /// 2. Filtering out words without valid embeddings.
    /// 3. Applying dimensionality reduction techniques (PCA and t-SNE).
    /// 4. Scaling the reduced-dimensional data for better visualization.
    /// 5. Saving the reduced embeddings in a CSV file for further analysis.
    /// 6. Using a Python script to generate scatter plots from the CSV data.
    /// The pipeline automatically handles missing embeddings, input validation, and file storage.
    /// </summary>
    public class Word2VecEmbeddingsDimReductionAndPlotting
    {
        // private readonly Word2VecService _word2VecService;
        private readonly DimensionalityReductionService _dimensionalityReductionService;
        private readonly CSVHelper _csvHelper;
        private readonly CSharpPythonConnector _pythonConnector;
        private readonly Lazy<Word2VecService> _word2VecService;


        /// <summary>
        /// Initializes the Word2Vec pipeline with required services.
        /// </summary>
        /// <param name="dimensionalityReductionService">Handles PCA and t-SNE dimensionality reduction.</param>
        /// <param name="csvHelper">Manages exporting reduced data to CSV format.</param>
        /// <param name="pythonConnector">Runs Python scripts for visualization.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the dependencies are null.</exception>
        public Word2VecEmbeddingsDimReductionAndPlotting(
            DimensionalityReductionService dimensionalityReductionService,
            CSVHelper csvHelper,
            CSharpPythonConnector pythonConnector)
        {
            _word2VecService = new Lazy<Word2VecService>(() =>
                new Word2VecService(Path.Combine(
                    Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"../../..")), 
                    "Datasets", "glove.6B.300d.txt"))
            );

            _dimensionalityReductionService = dimensionalityReductionService ?? throw new ArgumentNullException(nameof(dimensionalityReductionService));
            _csvHelper = csvHelper ?? throw new ArgumentNullException(nameof(csvHelper));
            _pythonConnector = pythonConnector ?? throw new ArgumentNullException(nameof(pythonConnector));
        }

        /// <summary>
        /// Executes the Word2Vec embedding pipeline, including dimensionality reduction and visualization.
        /// </summary>
        /// <param name="inputs">A list of words or phrases to process.</param>
        /// <exception cref="ArgumentException">Thrown if the input list is empty.</exception>
        public void RunPipeline(List<string> inputs)
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
            
            var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"../../.."));
            foreach (var (methodName, reductionMethod) in reductionMethods)
            {
                var csvFileName = $"word2vec_{methodName}_reduced.csv";
                var csvFilePath = Path.Combine(projectRoot, "Outputs", "CSV", csvFileName);
                var plotFileName = $"word2vec_{methodName}_scatterplot.png";
                var plotFilePath = Path.Combine(projectRoot, "Outputs", "Plots", plotFileName);
                
                // Filter out words with missing embeddings
                List<string> validWords;
                List<List<float>> validEmbeddings;
                CreateValidEmbeddings(inputs, out validWords, out validEmbeddings);

                if (validWords.Count == 0)
                {
                    Console.WriteLine("No valid embeddings found. Skipping pipeline.");
                    continue; // Skip this iteration if no valid embeddings are found
                }

                // Apply dimensionality reduction
                var reducedDataMatrix = reductionMethod(validEmbeddings);
                var scaledData = _dimensionalityReductionService.MinMaxScaleData(reducedDataMatrix);

                // Export reduced data to CSV
                ExportReducedDimensionalityData(scaledData, validWords, csvFileName);

                // Plot the results
                _pythonConnector.PlotScatterFromCsv(csvFilePath, plotFilePath);
            }

            Console.WriteLine("Word2Vec Pipeline completed successfully!");
        }

        /// <summary>
        /// Generates valid Word2Vec embeddings for the provided inputs.
        /// </summary>
        /// <param name="inputs">List of words or phrases.</param>
        /// <param name="validWords">Output list of words with valid embeddings.</param>
        /// <param name="validEmbeddings">Output list of corresponding embeddings.</param>
        private void CreateValidEmbeddings(List<string> inputs, out List<string> validWords, out List<List<float>> validEmbeddings)
        {
            Console.WriteLine($"Generating embeddings using Word2Vec...");

            validWords = new List<string>();
            validEmbeddings = new List<List<float>>();

            foreach (var input in inputs)
            {
                List<float> embedding; // Ensure this is properly assigned

                if (input.Contains(" "))
                {
                    embedding = _word2VecService.Value.GetPhraseVector(input).ToList();
                }
                else
                    embedding = _word2VecService.Value.GetWordVector(input)!.ToList();

                if (embedding.Any())  // Ensure embedding is valid
                {
                    validWords.Add(input);
                    validEmbeddings.Add(embedding);
                }
                else
                {
                    Console.WriteLine($"No valid embedding found for: {input}");
                }
            }
        }

        /// <summary>
        /// Saves the reduced-dimensional data to a CSV file.
        /// </summary>
        /// <param name="scaledData"></param>
        /// <param name="inputs">List of words or phrases corresponding to embeddings.</param>
        /// <param name="outputCsvFileName">Filename for the output CSV file.</param>
        private void ExportReducedDimensionalityData(Matrix<double> scaledData, List<string> inputs, string outputCsvFileName)
        {
            _csvHelper.ExportReducedDimensionalityData(scaledData, inputs, outputCsvFileName);
        }
    }
}
