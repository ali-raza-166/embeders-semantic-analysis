using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SemanticSimilarityAnalysis.Proj.Services

{
    public class Word2VecService
    {
        private readonly Dictionary<string, float[]> _wordVectors = new Dictionary<string, float[]>();

        public Word2VecService(string filePath)
        {
            LoadWord2Vec(filePath);
        }

        private void LoadWord2Vec(string filePath)
        {
            foreach (var line in File.ReadLines(filePath))
            {
                var parts = line.Split(' ');
                if (parts.Length < 2) continue;

                string word = parts[0];
                float[] vector = parts.Skip(1).Select(float.Parse).ToArray();
                _wordVectors[word] = vector;
            }
        }

        public float[] GetVector(string word)
        {
            return _wordVectors.TryGetValue(word, out var vector) ? vector : null;
        }
    }
}
