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
            
            var service = new DimensionalityReductionService(2);
            var inputData = new List<List<float>>
            {
                new List<float> { 1.0f, 2.0f, 3.0f },
                new List<float> { 4.0f, 5.0f, 6.0f },
                new List<float> { 7.0f, 8.0f, 9.0f }
             };

           
            service.ReduceDimensionsUsingTsne(inputData, 1); 
        }

        


    }
}

