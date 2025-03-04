using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Accord.MachineLearning.Clustering;
using Accord.Math;


namespace SemanticSimilarityAnalysis.Proj.Services
{
    public class DimensionalityReductionService
    {
        private readonly int _components;

        public DimensionalityReductionService(int components)
        {
            _components = components;
        }
        
        /// <summary>
        /// Reduces the dimensions of input data using the t-SNE (t-Distributed Stochastic Neighbor Embedding) algorithm.
        /// </summary>
        /// <param name="data">A list of lists where each inner list represents a data point (a row) with float values.</param>
        /// <param name="targetDimensions">The desired number of output dimensions (typically 2 or 3 for visualization).</param>
        /// <returns>A matrix representing the data reduced to the specified number of dimensions (default is 2).</returns>
        /// <remarks>Used to reduce the high-dimensional input data into a lower-dimensional space (typically for visualization). t-SNE is useful for uncovering complex structures in data.</remarks>
        public Matrix<double> ReduceDimensionsUsingTsne(List<List<float>> data, int targetDimensions = 2)
        {
            Console.WriteLine("Performing Tsne");
            // Ensure that targetDimensions is 2 or higher
            if (targetDimensions < 2)
                throw new ArgumentException("t-SNE requires at least 2 dimensions for output.");

            // Create the t-SNE object and set the parameters
            var tsne = new TSNE()
            {
                Perplexity = 0.65, // You can adjust the perplexity
                Theta = 0.5, // You can adjust the theta
                NumberOfOutputs = targetDimensions // Set the target dimensionality (default is 2)
            };
          
            // Convert List<List<float>> to double[][]
            var dataArray = ConvertListToJaggedArray(data);
            var reducedData = new double[dataArray.Length][];
            for (var i = 0; i < dataArray.Length; i++)
            {
                reducedData[i] = new double[targetDimensions]; // Initialize each row with the target dimension size
            }
            // Apply t-SNE transformation
            try
            {
                tsne.Transform(dataArray, reducedData);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error during t-SNE transformation", ex);
            }
            return ConvertJaggedArrayToMatrix(reducedData);
        }
        
        /// <summary>
        /// Converts a list of lists of floats into a jagged array of doubles.
        /// </summary>
        /// <param name="data">A list of lists where each inner list represents a data point (a row) with float values.</param>
        /// <returns>A jagged array of doubles, where each row represents a data point and each element represents a feature.</returns>
        /// <remarks>Converts the data to a jagged array format suitable for t-SNE transformation, which expects this format.</remarks>
        private static double[][] ConvertListToJaggedArray(List<List<float>> data)
        {
            int rowCount = data.Count;
            int columnCount = data[0].Count;
            double[][] array = new double[rowCount][];

            for (int i = 0; i < rowCount; i++)
            {
                array[i] = new double[columnCount];
                for (int j = 0; j < columnCount; j++)
                {
                    array[i][j] = data[i][j]; // Convert float to double
                }
            }

            return array;
        }
        /// <summary>
        /// Converts a jagged array of doubles into a dense matrix of doubles.
        /// </summary>
        /// <param name="array">A jagged array of doubles, where each row represents a data point and each element represents a feature.</param>
        /// <returns>A dense matrix of type Matrix<double> representing the jagged array input.</returns>
        /// <remarks>Converts the output of t-SNE or other transformation algorithms from a jagged array back to a matrix for further analysis or visualization.</remarks>
        private static Matrix<double> ConvertJaggedArrayToMatrix(double[][] array)
        {
            // Convert the jagged array (double[][]) into a 2D array (double[,])
            var rows = array.Length;
            var cols = array[0].Length;
            var matrixData = new double[rows, cols];

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    matrixData[i, j] = array[i][j];
                }
            }

            // Now return the Matrix<double> from the 2D array
            return DenseMatrix.OfArray(matrixData);
        }
       //---------------------------------------- PCA---------------------------------------------------
        public Matrix<double> PerformPca(List<List<float>> data)
        {
            // Convert the List of embeddings (List<float>) to a Matrix
            var matrixData = ConvertListToMatrix(data);
            Console.WriteLine($"Convert the List of embeddings (List<float>) to a Matrix: {matrixData}");
            
            // Perform mean centering
            var centeredData = CenterData(matrixData);
            Console.WriteLine($"Perform mean centering: {centeredData}");

            
            // Calculate covariance matrix
            var covarianceMatrix = ComputeCovarianceMatrix(centeredData);
            Console.WriteLine($"Covariance Matrix:\n{covarianceMatrix}");

            
            // Perform eigen decomposition to get the principal components
            var eigenDecomposition = covarianceMatrix.Evd();
            Console.WriteLine($"eigenDecomposition:\n{eigenDecomposition.EigenVectors}");


            // Select the top 'n' eigenvectors corresponding to the largest eigenvalues
            var topEigenVectors = eigenDecomposition.EigenVectors.SubMatrix(0, covarianceMatrix.RowCount, 0, _components);
            Console.WriteLine($"Top Eigen Vectors:\n{topEigenVectors}");
            
            // Project the centered data onto the principal components
            var projectedData = centeredData * topEigenVectors;
            Console.WriteLine($"Projected Data:\n{projectedData}");

            return projectedData;
        }


        /// <summary>
        /// Performs mean centering on the data by subtracting the mean of each feature (column) from the data points (rows).
        /// </summary>
        /// <param name="data">The input matrix where each row is a data point and each column is a feature.</param>
        /// <returns>A matrix where each data point is mean-centered with respect to each feature.</returns>
        /// <remarks>Mean centering is a common preprocessing step, especially for PCA, where each feature's mean is subtracted to have a centered dataset around 0.</remarks>
        private static Matrix<double> CenterData(Matrix<double> data)
        {
            // Compute the mean for each column (feature)
            var mean = data.ColumnSums() / data.RowCount;
            
            var rows = data.RowCount;
            var cols = data.ColumnCount;
            
            // Step 2: Create a mean matrix and subtract manually
            var centeredData = DenseMatrix.Create(data.RowCount, data.ColumnCount, 0.0);
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    centeredData[i, j] = data[i, j] - mean[j];
                }
            }

            // Step 3: Return the new centered matrix
            return centeredData;
        }

        /// <summary>
        /// Computes the covariance matrix of the given mean-centered data.
        /// </summary>
        /// <param name="centeredData">The mean-centered input matrix where each row is a data point and each column is a feature.</param>
        /// <returns>A covariance matrix representing the relationships between the features.</returns>
        /// <remarks>The covariance matrix is crucial for PCA, as it captures how the features vary with respect to one another. It is used to identify the principal components.</remarks>

        private static Matrix<double> ComputeCovarianceMatrix(Matrix<double> centeredData)
        {
            var rowCount = centeredData.RowCount;
            var colCount = centeredData.ColumnCount;

            // Initialize covariance matrix
            var covarianceMatrix = DenseMatrix.Create(colCount, colCount, 0.0);

            // Compute covariance using nested loops
            for (var i = 0; i < colCount; i++)
            {
                for (var j = 0; j < colCount; j++)
                {
                    var sum = 0.0;
                    for (var k = 0; k < rowCount; k++)
                    {
                        sum += centeredData[k, i] * centeredData[k, j];
                    }
                    covarianceMatrix[i, j] = sum / (rowCount - 1);
                }
            }
            
            return covarianceMatrix;
        }
        
        /// <summary>
        /// Applies Min-Max scaling to the data, transforming it into a specific range (e.g., 0 to 536 for the x-axis and -1 to 1 for the y-axis).
        /// </summary>
        /// <param name="reducedData">The data to be scaled, typically the result of dimensionality reduction.</param>
        /// <returns>A scaled matrix where the values are rescaled to the target range for each dimension.</returns>
        /// <remarks>Min-Max scaling is often used to normalize the data to a specific range, which can be important for visualization and comparison, especially when plotting reduced dimensions.</remarks>

        public Matrix<double> MinMaxScaleData(Matrix<double> reducedData)
        {
            // Get min and max values for each column (x and y axes)
            var minX = reducedData.Column(0).Minimum();
            var maxX = reducedData.Column(0).Maximum();

            var minY = reducedData.Column(1).Minimum();
            var maxY = reducedData.Column(1).Maximum();
            Console.WriteLine($"minX: {minX}, maxX: {maxX}, minY: {minY}, maxY: {maxY}");
            // Create a new matrix to store the scaled data
            var scaledData = DenseMatrix.Create(reducedData.RowCount, reducedData.ColumnCount, 0.0);
            
            /*
             Min Max Scaling formula
                scaled_x= (x-xmin)/(xmax-xmin)* (b-a) + a
                x=original number to be scaled
                b=max_allowed after scaling
                a=min_allowed after scaling
                [a,b] defines the range of new scaling. i.e [0, 536], and [1,-1] in out case
             */
            for (var i = 0; i < reducedData.RowCount; i++)
            {
                for (var j = 0; j < reducedData.ColumnCount; j++)
                {
                    // Scale x-axis (between 0 and 536)
                    var a=0;
                    var b=0;
                    if (j == 0)
                    {
                        a = 0;
                        b = 536;
                        scaledData[i, j] = (reducedData[i, j] - minX) / (maxX - minX) * (b-a) + a;
                    }
                    // Scale y-axis (between -1 and 1)
                    else if (j == 1)
                    {
                        a = -1;
                        b = 1;
                        scaledData[i, j] = (reducedData[i, j] - minY) / (maxY - minY) * (b - a) + a;
                    }
                }
            }

            // Return the scaled data as a DenseMatrix
            return scaledData;
        }
        
        
        /// <summary>
        /// Converts a list of lists of floats into a dense matrix of doubles.
        /// </summary>
        /// <param name="data">A list of lists where each inner list represents a data point (a row) with float values.</param>
        /// <returns>A dense matrix of type Matrix<double> representing the input data.</returns>
        /// <remarks>Used for transforming raw data (list format) into a matrix format suitable for mathematical operations like PCA or t-SNE.</remarks>
        private static Matrix<double> ConvertListToMatrix(List<List<float>> data)
        {
            var rowCount = data.Count;
            var columnCount = data[0].Count;
            var matrixData = new double[rowCount, columnCount];

            for (var i = 0; i < rowCount; i++)
            {
                for (var j = 0; j < columnCount; j++)
                {
                    matrixData[i, j] = data[i][j];
                }
            }

            return DenseMatrix.OfArray(matrixData);
        }


    }
}
