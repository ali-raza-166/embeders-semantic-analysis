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
            Console.WriteLine("Loading Word2Vec model...");
            var totalLines = File.ReadLines(filePath).Count();
            int lineCount = 0;
            int linesToPrintProgress = 10000; 

            foreach (var line in File.ReadLines(filePath))
            {
                lineCount++;
                if (lineCount % linesToPrintProgress == 0)
                {
                    Console.WriteLine($"Processed {lineCount} lines out of {totalLines}...");
                }

                var parts = line.Split(' ');
                if (parts.Length < 2) continue;

                string word = parts[0];
                float[] vector = parts.Skip(1).Select(float.Parse).ToArray();
                _wordVectors[word] = vector;
            }

            Console.WriteLine("Word2Vec model loaded successfully!");
        }
        
        public float[]? GetWordVector(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                throw new ArgumentException("Word cannot be null or empty.", nameof(word));
            word = word.ToLower();
            if (_wordVectors == null)
                throw new InvalidOperationException("Word vectors dictionary is not initialized.");

            return _wordVectors.GetValueOrDefault(word);
        }
        public float[] GetPhraseVector(string phrase)
        {
            phrase = phrase.ToLower();
            var words = phrase.Split(' ');
            var vectors = new List<float[]>();

            foreach (var word in words)
            {
                var vector = GetWordVector(word);
                if (vector != null)
                {
                    vectors.Add(vector);
                }
            }

            if (vectors.Count == 0)
            {
                return null!; // If no valid vectors were found for the phrase
            }

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
