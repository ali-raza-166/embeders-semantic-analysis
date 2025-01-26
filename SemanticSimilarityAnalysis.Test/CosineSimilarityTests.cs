using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticSimilarityAnalysis.Proj.Utils; // Namespace of the CosineSimilarity class
using System.Collections.Generic;

namespace SemanticSimilarityAnalysis.Tests
{
    [TestClass] // Marks this class as containing unit tests
    public class CosineSimilarityTests
    {
        [TestMethod] // Marks this method as a unit test
        public void ComputeCosineSimilarity_ValidInputs_ReturnsCorrectResult()
        {
            // Arrange: Create an instance of the class and define test vectors
            var cosineSimilarity = new CosineSimilarity();
            var vectorA = new List<float> { 1, 2, 3 };
            var vectorB = new List<float> { 4, 5, 6 };

            // Expected result manually calculated
            var expected = 0.974631846;

            // Act: Call the method with test vectors
            var result = cosineSimilarity.ComputeCosineSimilarity(vectorA, vectorB);

            // Assert: Check if the result is within a small delta of the expected value
            Assert.AreEqual(expected, result, 0.000001, "Cosine similarity calculation is incorrect.");
        }

        [TestMethod]
        public void ComputeCosineSimilarity_DifferentLengths_ReturnsZero()
        {
            // Arrange
            var cosineSimilarity = new CosineSimilarity();
            var vectorA = new List<float> { 1, 2, 3 };
            var vectorB = new List<float> { 4, 5 }; // Different length

            // Act
            var result = cosineSimilarity.ComputeCosineSimilarity(vectorA, vectorB);

            // Assert
            Assert.AreEqual(0, result, "Method should return 0 for vectors of different lengths.");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void ComputeCosineSimilarity_ZeroMagnitude_ThrowsException()
        {
            // Arrange
            var cosineSimilarity = new CosineSimilarity();
            var vectorA = new List<float> { 0, 0, 0 };
            var vectorB = new List<float> { 4, 5, 6 };

            // Act
            cosineSimilarity.ComputeCosineSimilarity(vectorA, vectorB);
        }
    }
}
