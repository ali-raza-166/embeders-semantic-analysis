using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticSimilarityAnalysis.Proj.Services;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;



namespace SemanticSimilarityAnalysis.Tests
{
    [TestClass]
    public class DimensionalityReductionServiceTests
    {
        [TestMethod]
        public void ReduceDimensionsUsingTsne_ShouldReduceToTargetDimensions()
        {
            // Arrange: Create service and input data
            var service = new DimensionalityReductionService(2);
            var inputData = new List<List<float>>
            {
                new List<float> { 1.0f, 2.0f, 3.0f },
                new List<float> { 4.0f, 5.0f, 6.0f },
                new List<float> { 7.0f, 8.0f, 9.0f }
            };

            // Act: Dimensions reduction
            var result = service.ReduceDimensionsUsingTsne(inputData, 2);

            // Assert: Result validation
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.AreEqual(2, result.ColumnCount, "Result should have exactly 2 columns.");
        }

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
            
            var service = new DimensionalityReductionService(2);
            var inputData = DenseMatrix.OfArray(new double[,]
            {
                { 1.0, 2.0 },
                { 3.0, 6.0 },
                { 5.0, 10.0 }
            });

            
            var result = service.MinMaxScaleData(inputData);

            
            Assert.AreEqual(0, result[0, 0], "First X value should be scaled to 0.");
            Assert.AreEqual(536, result[2, 0], "Last X value should be scaled to 536."); 
            Assert.AreEqual(-1, result[0, 1], "First Y value should be scaled to -1.");
            Assert.AreEqual(1, result[2, 1], "Last Y value should be scaled to 1.");
        }



    }
}

