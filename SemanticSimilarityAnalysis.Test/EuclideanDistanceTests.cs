using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticSimilarityAnalysis.Proj.Utils; // Namespace of the CosineSimilarity class
using System.Collections.Generic;

namespace SemanticSimilarityAnalysis.Tests
{
    [TestClass] // Marks this class as containing unit tests
    public class EuclideanDistanceTests
    {
        [TestMethod] // Marks this method as a unit test
        public void ComputeEuclideanDistance_ValidInputs_ReturnsCorrectResult()
        {
            // Arrange
            var euclideanDistance = new EuclideanDistance();
            var vectorA = new List<float> { 1, 2, 3 };
            var vectorB = new List<float> { 4, 5, 6 };

            // Expected result manually calculated
            var expected = 5.196152422;

            // Act
            var result = euclideanDistance.ComputeEuclideanDistance(vectorA, vectorB);

            // Assert
            Assert.AreEqual(expected, result, 0.000001, "Euclidean distance calculation is incorrect.");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void ComputeEuclideanDistance_DifferentLengths_ThrowsException()
        {
            // Arrange
            var euclideanDistance = new EuclideanDistance();
            var vectorA = new List<float> { 1, 2, 3 };
            var vectorB = new List<float> { 4, 5 };

            // Act
            euclideanDistance.ComputeEuclideanDistance(vectorA, vectorB);
        }


        [TestMethod]
        public void ComputeEuclideanDistance_IdenticalVectors_ReturnsZero()
        {
            // Arrange
            var euclideanDistance = new EuclideanDistance();
            var vectorA = new List<float> { 1, 2, 3 };
            var vectorB = new List<float> { 1, 2, 3 };

            // Act
            var result = euclideanDistance.ComputeEuclideanDistance(vectorA, vectorB);

            // Assert
            Assert.AreEqual(0, result, "Euclidean distance between identical vectors should be zero.");
        }
    }
}
