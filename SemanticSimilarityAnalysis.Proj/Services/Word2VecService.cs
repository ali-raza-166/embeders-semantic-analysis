using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UglyToad.PdfPig.Content;

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
        
        public float[]? GetVector(string word)
        {
            word = word.ToLower();
            return _wordVectors.TryGetValue(word, out var vector) ? vector : null;
        }
        public float[] GetPhraseVector(string phrase)
        {
            phrase = phrase.ToLower();
            var words = phrase.Split(' ');
            var vectors = new List<float[]>();

            foreach (var word in words)
            {
                var vector = GetVector(word);
                if (vector != null)
                {
                    vectors.Add(vector);
                }
            }

            if (vectors.Count == 0)
            {
                return null; // If no valid vectors were found for the phrase
            }

            // Average the word vectors
            int vectorLength = vectors[0].Length;
            float[] phraseVector = new float[vectorLength];

            foreach (var vector in vectors)
            {
                for (int i = 0; i < vectorLength; i++)
                {
                    phraseVector[i] += vector[i];
                }
            }

            // Normalize the phrase vector by the number of words
            for (int i = 0; i < vectorLength; i++)
            {
                phraseVector[i] /= vectors.Count;
            }

            return phraseVector;
        }
    }
}
