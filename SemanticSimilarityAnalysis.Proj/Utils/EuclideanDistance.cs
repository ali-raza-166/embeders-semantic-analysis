namespace SemanticSimilarityAnalysis.Proj.Utils
{
    public class EuclideanDistance
    {
       
        public double ComputeEuclideanDistance(List<float> vectorA, List<float> vectorB)
        {
            if (vectorA.Count != vectorB.Count)
            {
                throw new ArgumentException("Vectors must have the same length.");
            }
            double sumOfSquares = 0.0;
            for (var i = 0; i < vectorA.Count; i++)
            {
                sumOfSquares += Math.Pow(vectorA[i] - vectorB[i], 2);
            }
            return Math.Sqrt(sumOfSquares);
        }
    }
}