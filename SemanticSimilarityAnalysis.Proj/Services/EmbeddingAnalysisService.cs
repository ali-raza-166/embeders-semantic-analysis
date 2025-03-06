using SemanticSimilarityAnalysis.Proj.Helpers.Csv;
using SemanticSimilarityAnalysis.Proj.Helpers.Json;
using SemanticSimilarityAnalysis.Proj.Helpers.Pdf;
using SemanticSimilarityAnalysis.Proj.Models;
using SemanticSimilarityAnalysis.Proj.Utils;
using System.Data;

namespace SemanticSimilarityAnalysis.Proj.Services
{
    /// <summary>
    /// Provides services for analyzing and comparing text embeddings using cosine similarity and Euclidean distance.
    /// This class supports tasks such as comparing text/document embeddings, processing dataset embeddings from CSV files,
    /// and analyzing similarity between input texts and precomputed embeddings stored in JSON files.
    /// </summary>
    public class EmbeddingAnalysisService
    {
        private readonly OpenAiEmbeddingService _embeddingService;
        private readonly CosineSimilarity _similarityCalculator;
        private readonly EuclideanDistance _euclideanDistCalc;
        private readonly EmbeddingUtils _embeddingUtils;
        private readonly PdfHelper _pdfHelper;
        private readonly CSVHelper _csvHelper;
        private readonly JsonHelper _jsonHelper;

        public EmbeddingAnalysisService(
            OpenAiEmbeddingService embeddingService,
            CosineSimilarity similarityCalculator,
            EuclideanDistance euclideanDistCalc,
            EmbeddingUtils embeddingUtils,
            PdfHelper pdfHelper,
            CSVHelper csvHelper,
            JsonHelper jsonHelper
        )
        {
            _embeddingService = embeddingService;
            _similarityCalculator = similarityCalculator;
            _euclideanDistCalc = euclideanDistCalc;
            _embeddingUtils = embeddingUtils;
            _pdfHelper = pdfHelper;
            _csvHelper = csvHelper;
            _jsonHelper = jsonHelper;
        }


        /// <summary>
        /// Compares the similarity between all words in the first list and all words in the second list.
        /// </summary>
        /// <param name="words1">The first list of words to compare.</param>
        /// <param name="words2">The second list of words to compare.</param>
        /// <returns>A list of SimilarityPlotPoint objects, organized in a table-like format.</returns>
        public async Task<List<SimilarityPlotPoint>> CompareWordsVsWords(List<string> words1, List<string> words2)
        {
            if (words1 == null || words1.Count == 0)
            {
                throw new ArgumentException("The first list of words cannot be null or empty.", nameof(words1));
            }

            if (words2 == null || words2.Count == 0)
            {
                throw new ArgumentException("The second list of words cannot be null or empty.", nameof(words2));
            }

            // Generate embeddings for the first list of words
            var embeddings1 = await _embeddingService.CreateEmbeddingsAsync(words1);

            // Generate embeddings for the second list of words
            var embeddings2 = await _embeddingService.CreateEmbeddingsAsync(words2);

            var similarityResults = new List<SimilarityPlotPoint>();

            // Compare each word in the first list with each word in the second list
            for (int i = 0; i < words1.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(words1[i]))
                {
                    Console.WriteLine($"Warning: Skipping comparison for an empty or null word in the first list at index {i}.");
                    continue;
                }

                // Create a dictionary to store similarity scores for this word (row)
                var similarities = new Dictionary<string, double>();

                for (int j = 0; j < words2.Count; j++)
                {
                    if (string.IsNullOrWhiteSpace(words2[j]))
                    {
                        Console.WriteLine($"Warning: Skipping comparison for an empty or null word in the second list at index {j}.");
                        continue;
                    }

                    // Compute cosine similarity between the two word embeddings
                    double similarity = _similarityCalculator.ComputeCosineSimilarity(embeddings1[i].Values, embeddings2[j].Values);

                    // Add the similarity score to the dictionary
                    similarities.Add(words2[j], similarity);
                }

                // Create a SimilarityPlotPoint for this row (word from words1)
                similarityResults.Add(new SimilarityPlotPoint(words1[i], similarities));
            }
            return similarityResults;
        }


        /// <summary>
        /// Compares the similarity between a list of words and the content of PDF files in a folder.
        /// </summary>
        /// <param name="words">The list of words to compare.</param>
        /// <param name="pdfFolderPath">The path to the folder containing PDF files.</param>
        /// <param name="chunkType">The type of chunks to extract from the PDF (paragraphs, sentences, or none).</param>
        /// <returns>A list of SimilarityPlotPoint objects, where each row represents a PDF file and columns represent words.</returns>
        public async Task<List<SimilarityPlotPoint>> ComparePdfsvsWords(List<string> words, string pdfFolderPath = @"../../Datasets/PDFs", PdfHelper.ChunkType chunkType = PdfHelper.ChunkType.None)
        {
            if (words == null || words.Count == 0)
            {
                throw new ArgumentException("The list of words cannot be null or empty.", nameof(words));
            }

            if (string.IsNullOrWhiteSpace(pdfFolderPath))
            {
                throw new ArgumentException("The PDF folder path cannot be null or empty.", nameof(pdfFolderPath));
            }

            if (!Directory.Exists(pdfFolderPath))
            {
                throw new DirectoryNotFoundException($"The folder '{pdfFolderPath}' does not exist.");
            }

            var similarityResults = new List<SimilarityPlotPoint>();

            // Get all PDF files in the folder
            var pdfFiles = Directory.GetFiles(pdfFolderPath, "*.pdf");

            if (pdfFiles.Length == 0)
            {
                throw new FileNotFoundException("No PDF files found in the specified folder.");
            }

            // Generate embeddings for all words
            var wordEmbeddings = await _embeddingService.CreateEmbeddingsAsync(words);

            foreach (var pdfFile in pdfFiles)
            {
                try
                {
                    // Extract text chunks from the PDF using PdfHelper
                    var textChunks = _pdfHelper.ExtractTextChunks(pdfFile, chunkType);

                    if (textChunks.Count == 0)
                    {
                        Console.WriteLine($"Warning: No text extracted from PDF file '{Path.GetFileName(pdfFile)}'.");
                        continue;
                    }

                    // Generate embeddings for the PDF text chunks
                    var documentEmbeddings = await _embeddingService.CreateEmbeddingsAsync(textChunks);

                    // Compute the average embedding for the document
                    var documentVector = _embeddingUtils.GetAverageEmbedding(documentEmbeddings);

                    // Create a dictionary to store similarity scores for this PDF file
                    var similarities = new Dictionary<string, double>();

                    // Compare each word with the PDF content
                    for (int i = 0; i < words.Count; i++)
                    {
                        if (string.IsNullOrWhiteSpace(words[i]))
                        {
                            Console.WriteLine($"Warning: Skipping comparison for an empty or null word at index {i}.");
                            continue;
                        }

                        // Compute cosine similarity between the word and the document
                        double similarity = _similarityCalculator.ComputeCosineSimilarity(wordEmbeddings[i].Values, documentVector);

                        // Add the similarity score to the dictionary
                        similarities.Add(words[i], similarity);
                    }

                    // Create a SimilarityPlotPoint for this PDF file
                    similarityResults.Add(new SimilarityPlotPoint(Path.GetFileName(pdfFile), similarities));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing PDF file '{Path.GetFileName(pdfFile)}': {ex.Message}");
                }
            }

            return similarityResults;
        }


        /// <summary>
        /// Compares the cosine similarity of all PDFs in the specified folder.
        /// This method extracts text from each PDF, generates embeddings for the extracted text chunks,
        /// computes the average embedding for each PDF, and then calculates the cosine similarity between all unique document pairs.
        /// The similarity results are returned as a list of SimilarityPlotPoint objects containing the label and similarity values.
        /// </summary>
        /// <param name="inputDir">The directory containing the PDF files to be compared.</param>
        /// <param name="chunkType">The type of chunking to apply when extracting text from PDFs (default is None).</param>
        /// <returns>Returns a list of SimilarityPlotPoint objects</returns>
        public async Task<List<SimilarityPlotPoint>> CompareAllPdfDocuments(
             string inputDir = @"../../Datasets/PDFs",
             PdfHelper.ChunkType chunkType = PdfHelper.ChunkType.None
        )
        {
            // Get all PDF file paths in the directory
            var pdfFiles = Directory.GetFiles(inputDir, "*.pdf");

            if (pdfFiles.Length < 2)
            {
                throw new InvalidOperationException("At least two PDF files are required for comparison.");
            }

            Console.WriteLine($"Extracting text from {pdfFiles.Length} PDF files...");

            // Dictionary to store average embeddings for each PDF (using file paths as keys)
            var averageEmbeddingsDict = new Dictionary<string, List<float>>();

            // Process each PDF file to generate embeddings
            foreach (var pdfFile in pdfFiles)
            {
                string fileName = Path.GetFileName(pdfFile);

                // Extract text chunks from the PDF
                var textChunks = _pdfHelper.ExtractTextChunks(pdfFile, chunkType);

                if (textChunks.Count == 0)
                {
                    Console.WriteLine($"Warning: No extractable text in {fileName}.");
                    continue;
                }

                // Generate embeddings for the text chunks and compute the average embedding
                var embeddings = await _embeddingService.CreateEmbeddingsAsync(textChunks);
                averageEmbeddingsDict[pdfFile] = _embeddingUtils.GetAverageEmbedding(embeddings);
            }

            Console.WriteLine("Computing similarity metrics for all document pairs...");

            // List to store similarity plot points
            var similarityPlotPoints = new List<SimilarityPlotPoint>();

            for (int i = 0; i < pdfFiles.Length; i++)
            {
                for (int j = i + 1; j < pdfFiles.Length; j++)
                {
                    string filePath1 = pdfFiles[i];
                    string filePath2 = pdfFiles[j];

                    string fileName1 = Path.GetFileName(filePath1);
                    string fileName2 = Path.GetFileName(filePath2);

                    // Check if embeddings are available for both files
                    if (!averageEmbeddingsDict.TryGetValue(filePath1, out var avgEmbedding1) ||
                        !averageEmbeddingsDict.TryGetValue(filePath2, out var avgEmbedding2))
                    {
                        Console.WriteLine($"Warning: Skipping comparison for {fileName1} and {fileName2} due to missing embeddings.");
                        continue;
                    }

                    double cosineSimilarity = _similarityCalculator.ComputeCosineSimilarity(avgEmbedding1, avgEmbedding2);

                    similarityPlotPoints.Add(new SimilarityPlotPoint(fileName1, new Dictionary<string, double> { { fileName2, cosineSimilarity } }));

                    Console.WriteLine($"Cosine Similarity between {fileName1} and {fileName2}: {cosineSimilarity}");
                }
            }
            return similarityPlotPoints;
        }


        /// <summary>
        /// Analyzes similarity between input text embeddings and dataset embeddings. 
        /// </summary>
        /// <param name="jsonFileName">The name of the JSON file containing dataset embeddings.</param>
        /// <param name="attributeKeyForLabel">The key in the JSON file representing the label.</param>
        /// <param name="attributeKeyForEmbedding">The key in the JSON file representing the embedding which will be compared with the input embeddings.</param>
        /// <param name="inputStrings">A list of input text strings to analyze.</param>
        /// <param name="inputDir">The directory containing the JSON file. Default is @"../../../Outputs".</param>
        /// <returns>A list of SimilarityPlotPoint objects for plotting.</returns>
        public async Task<List<SimilarityPlotPoint>> compareDataSetVsWords(
            string jsonFileName,
            string attributeKeyForLabel,
            string attributeKeyForEmbedding,
            List<string> inputStrings,
            string inputDir = @"../../../Outputs")
        {
            var jsonFilePath = Path.Combine(inputDir, jsonFileName);

            // Check if JSON file exists
            if (!File.Exists(jsonFilePath))
                throw new FileNotFoundException($"JSON file not found: {jsonFilePath}");

            // Load records from JSON
            var records = _jsonHelper.GetRecordFromJson(jsonFilePath);

            // Generate input embeddings from the input strings
            var inputEmbeddings = await _embeddingService.CreateEmbeddingsAsync(inputStrings);
            var inputVectors = inputEmbeddings.Select(e => e.Values).ToList();

            var similarityResults = new List<SimilarityPlotPoint>();

            foreach (var record in records)
            {
                // Ensure the attribute key exists in the record
                if (!record.Attributes.TryGetValue(attributeKeyForLabel, out var recordLabel))
                {
                    throw new KeyNotFoundException($"Attribute key '{attributeKeyForLabel}' not found in record attributes.");
                }

                // Ensure the record has embeddings for the specified attribute key
                if (!record.Vectors.TryGetValue(attributeKeyForEmbedding, out var vectors) || vectors.Count == 0)
                {
                    throw new KeyNotFoundException($"No vector data found for attribute key '{attributeKeyForEmbedding}' in record.");
                }

                var attributeEmbedding = vectors.First().Values; // vectorList[0].Values

                // Calculate similarities dynamically for all input vectors
                var similarities = new Dictionary<string, double>();
                Console.WriteLine($"\nAnalysis for '{recordLabel}': ");
                for (int i = 0; i < inputVectors.Count; i++)
                {
                    var similarity = _similarityCalculator.ComputeCosineSimilarity(attributeEmbedding, inputVectors[i]);
                    var inputKey = inputStrings[i]; // Use the original input string as the key
                    similarities[inputKey] = similarity;
                    Console.WriteLine($"Similarity with '{inputKey}': {similarity}");
                }

                // Create a SimilarityPlotPoint object
                var similarityPlotPoint = new SimilarityPlotPoint(recordLabel, similarities);
                similarityResults.Add(similarityPlotPoint);
            }

            // Return the list of SimilarityPlotPoint objects, ordered by the first input's similarity in descending order 
            //return similarityResults
            //    .OrderByDescending(p => p.Similarities.Values.FirstOrDefault())
            //    .ToList();

            return similarityResults;
        }


        /// <summary>
        /// Processes dataset embeddings from a CSV file and saves them as JSON.
        /// </summary>
        /// <param name="fields">List of fields to extract from the CSV file.</param>
        /// <param name="csvFileName">Name of the dataset file.</param>
        /// <param name="processedRows">Number of rows to process from the CSV file (default is -1, which means use 20 rows).</param>
        /// <param name="inputDir">Directory containing the CSV file.</param>
        /// <param name="outputDir">Directory to save the JSON file.</param>
        /// <returns>A list of MultiEmbeddingRecord objects.</returns>
        public async Task<List<MultiEmbeddingRecord>> CreateDataSetEmbeddingsAsync(
            List<string> fields,
            string csvFileName,
            int processedRows = -1, // Sentinel value to indicate no user input
            string inputDir = @"../../../Datasets",
            string outputDir = @"../../../Outputs"
        )
        {
            // Validate that the fields list is not null or empty
            if (fields == null || fields.Count == 0)
            {
                throw new ArgumentException("The fields list cannot be null or empty.");
            }

            // Construct the dataset file path
            string datasetPath = Path.Combine(inputDir, csvFileName);
            Console.WriteLine($"Extracting records from: {datasetPath}");

            // Check if the CSV file exists
            if (!File.Exists(datasetPath))
            {
                throw new FileNotFoundException($"CSV file not found: {datasetPath}");
            }

            // Extract all records from the CSV file
            var allRecords = _csvHelper.ExtractRecordsFromCsv(fields, datasetPath);

            // Determine the number of rows to process
            int rowsToProcess = _csvHelper.DetermineRowsToProcess(allRecords, processedRows);
            Console.WriteLine($"Processing {rowsToProcess} rows.");

            // Take only the specified number of rows
            var records = allRecords.Take(rowsToProcess).ToList();

            // Process embeddings for the selected rows
            foreach (var record in records)
            {
                foreach (var attribute in record.Attributes)
                {
                    var attributeName = attribute.Key;
                    var attributeValue = attribute.Value;

                    if (string.IsNullOrWhiteSpace(attributeValue)) continue;

                    // Generate embedding for the attribute value
                    var attributeEmbedding = await _embeddingService.CreateEmbeddingsAsync(new List<string> { attributeValue });

                    // Add the generated attribute embedding to the record
                    record.AddEmbedding(attributeName, new VectorData(attributeEmbedding[0].Values));
                }
            }

            // Ensure the 'Outputs' directory exists
            Directory.CreateDirectory(outputDir);

            // Remove .csv extension from the file name
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(csvFileName);

            // Save the embeddings as a JSON file
            string jsonFilePath = Path.Combine(outputDir, $"{fileNameWithoutExtension}_Embeddings.json");

            await _jsonHelper.SaveRecordToJson(records, jsonFilePath);

            Console.WriteLine($"Embeddings saved to: {jsonFilePath}");

            return records;
        }
    }
}