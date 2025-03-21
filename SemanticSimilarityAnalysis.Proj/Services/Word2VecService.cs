namespace SemanticSimilarityAnalysis.Proj.Services
{
    /// <summary>
    /// Provides functionality to load and interact with a Word2Vec model.
    /// This service loads word vectors from a file, retrieves vector representations 
    /// of individual words, and computes averaged vectors for phrases.
    /// </summary>
    public class Word2VecService
    {
        private readonly Dictionary<string, float[]> _wordVectors = new Dictionary<string, float[]>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Word2VecService"/> class
        /// and loads the Word2Vec model from the specified file.
        /// </summary>
        /// <param name="filePath">The file path of the Word2Vec model.</param>
        public Word2VecService(string filePath)
        {
            LoadWord2Vec(filePath);
        }
        
        /// <summary>
        /// Loads the Word2Vec model from a file and stores word vectors in memory.
        /// </summary>
        /// <param name="filePath">The file path of the Word2Vec model.</param>
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
        
        /// <summary>
        /// Retrieves the vector representation of a given word.
        /// </summary>
        /// <param name="word">The word to retrieve the vector for.</param>
        /// <returns>The word vector as an array of floats, or null if not found.</returns>
        /// <exception cref="ArgumentException">Thrown if the input word is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the word vector dictionary is not initialized.</exception>
        public float[]? GetWordVector(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                throw new ArgumentException("Word cannot be null or empty.", nameof(word));
            word = word.ToLower();
            if (_wordVectors == null)
                throw new InvalidOperationException("Word vectors dictionary is not initialized.");

            return _wordVectors.GetValueOrDefault(word);
        }
        
        /// <summary>
        /// Computes an averaged vector representation for a given phrase.
        /// </summary>
        /// <param name="phrase">The phrase to compute the vector for.</param>
        /// <returns>
        /// The averaged phrase vector as an array of floats.
        /// Returns null if no valid word vectors are found.
        /// </returns>
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
