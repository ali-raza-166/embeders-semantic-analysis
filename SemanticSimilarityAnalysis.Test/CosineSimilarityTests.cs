using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticSimilarityAnalysis.Proj.Utils; 
using System.Collections.Generic;

namespace SemanticSimilarityAnalysis.Tests
{
    [TestClass] 
    public class CosineSimilarityTests
    {
        [TestMethod] 
        public void ComputeCosineSimilarity_ValidInputs_ReturnsCorrectResult()
        {
            
            var cosineSimilarity = new CosineSimilarity();
            var vectorA = new List<float> { 1, 2, 3 };
            var vectorB = new List<float> { 4, 5, 6 };

            
            var expected = 0.974631846;

            
            var result = cosineSimilarity.ComputeCosineSimilarity(vectorA, vectorB);

            
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
            
            var cosineSimilarity = new CosineSimilarity();
            var vectorA = new List<float> { 0, 0, 0 };
            var vectorB = new List<float> { 4, 5, 6 };

            
            cosineSimilarity.ComputeCosineSimilarity(vectorA, vectorB);
        }
    }
}
