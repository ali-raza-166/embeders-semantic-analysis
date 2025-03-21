﻿using SemanticSimilarityAnalysis.Proj.Models;
namespace SemanticSimilarityAnalysis.Proj.Utils
{
    /// <summary>
    /// Provides utility methods for working with embeddings (vector data).
    /// </summary>
    public class EmbeddingUtils
    {
        /// <summary>
        /// Calculates the average embedding from a list of embeddings.
        /// </summary>
        /// <param name="embeddings">List of embeddings</param>
        /// <returns>List of floats representing the average embedding</returns>
        /// <exception cref="ArgumentException"></exception>
        public List<float> GetAverageEmbedding(List<Embedding> embeddings)
        {
            if (embeddings == null || embeddings.Count == 0)
            {
                throw new ArgumentException("The embeddings list cannot be null or empty.");
            }
            var numOfEmbeddings = embeddings.Count;
            var vectorLength = embeddings[0].Values.Count;
            var avgEmbedding = new List<float>(new float[vectorLength]);

            foreach (var embedding in embeddings)
            {
                for (var i = 0; i < vectorLength; i++)
                {
                    avgEmbedding[i] += embedding.Values[i];
                }
            }

            for (var i = 0; i < vectorLength; i++)
            {
                avgEmbedding[i] /= numOfEmbeddings;
            }

            return avgEmbedding;
        }
    }
}