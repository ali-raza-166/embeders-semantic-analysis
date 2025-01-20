using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticSimilarityAnalysis.Proj.Utils; 
using System.Collections.Generic;

namespace SemanticSimilarityAnalysis.Tests
{
    [TestClass]
    public class EuclideanDistanceTests
    {
        [TestMethod]
        public void ComputeEuclideanDistance_ValidInputs_ReturnsCorrectResult()
        {
            
            var euclideanDistance = new EuclideanDistance();
            var vectorA = new List<float> { 1, 2, 3 };
            var vectorB = new List<float> { 4, 5, 6 };

            
            var expected = 5.196152422;

            
            var result = euclideanDistance.ComputeEuclideanDistance(vectorA, vectorB);

            
            Assert.AreEqual(expected, result, 0.000001, "Euclidean distance calculation is incorrect.");
        }
    }
}
