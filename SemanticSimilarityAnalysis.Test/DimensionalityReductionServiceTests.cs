using MathNet.Numerics.LinearAlgebra.Double;
using SemanticSimilarityAnalysis.Proj.Services;



namespace Service.Tests
{
    [TestClass]
    public class DimensionalityReductionServiceTests
    {

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReduceDimensionsUsingTsne_ShouldThrowExceptionForInvalidDimensions()
        {
            // Arrange: Creating service and input data
            var service = new DimensionalityReductionService(2);
            var inputData = new List<List<float>>
            {
                new List<float> { 1.0f, 2.0f, 3.0f },
                new List<float> { 4.0f, 5.0f, 6.0f },
                new List<float> { 7.0f, 8.0f, 9.0f }
             };

            // Act: Attempt to reduce to invalid dimensions
            service.ReduceDimensionsUsingTsne(inputData, 1);
        }

        [TestMethod]
        public void MinMaxScaleData_ShouldScaleCorrectly()
        {
            // Arrange: Initializing the service and defining input data
            var service = new DimensionalityReductionService(2);
            var inputData = DenseMatrix.OfArray(new double[,]
            {
                { 1.0, 2.0 },
                { 3.0, 6.0 },
                { 5.0, 10.0 }
            });

            // Act: Applying MinMax scaling to the input data
            var result = service.MinMaxScaleData(inputData);

            // Assert: Verifying that the scaled values match expected results
            Assert.AreEqual(0, result[0, 0], "First X value should be scaled to 0.");
            Assert.AreEqual(536, result[2, 0], "Last X value should be scaled to 536.");
            Assert.AreEqual(-1, result[0, 1], "First Y value should be scaled to -1.");
            Assert.AreEqual(1, result[2, 1], "Last Y value should be scaled to 1.");
        }



    }
}

