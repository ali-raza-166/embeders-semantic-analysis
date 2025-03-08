using System.Text.RegularExpressions;

public class TextHelper
{
    /// <summary>
    /// Extracts text from a .txt file and returns a list of words.
    /// </summary>
    /// <param name="fileName">The name of the .txt file.</param>
    /// <param name="inputDir">The path to the directory containing the .txt file. Default is "../../Datasets/TXTs".</param>
    /// <returns>A list of words extracted from the file.</returns>
    public List<string> ExtractWordsFromTextFile(string fileName, string inputDir = "../../../Datasets/TXTs")
    {
        var filePath = Path.Combine(inputDir, fileName);
        Console.WriteLine(filePath);

        // Check if the file exists
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file '{filePath}' does not exist.");
        }

        // Read the entire file content
        string fileContent = File.ReadAllText(filePath);

        // Clean and split the text into words
        List<string> words = CleanAndSplitText(fileContent);

        return words;
    }

    /// <summary>
    /// Cleans the text by removing punctuation, converting to lowercase, and splitting into words.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <returns>A list of cleaned words.</returns>
    private List<string> CleanAndSplitText(string text)
    {
        // Remove punctuation and special characters using regex
        string cleanedText = Regex.Replace(text, @"[^\w\s]", "");

        // Convert text to lowercase
        cleanedText = cleanedText.ToLower();

        // Split the text into words
        List<string> words = cleanedText.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        // Remove stopwords (optional)
        words = RemoveStopWords(words);

        return words;
    }

    /// <summary>
    /// Removes common stopwords from the list of words.
    /// </summary>
    /// <param name="words">The list of words.</param>
    /// <returns>A list of words without stopwords.</returns>
    private List<string> RemoveStopWords(List<string> words)
    {
        // Define a list of common stopwords
        var stopWords = new HashSet<string>
        {
            "a", "an", "the", "and", "or", "but", "if", "in", "on", "with", "as", "by", "for", "of", "at", "to", "from", "up", "down", "out", "over", "under", "again", "further", "then", "once"
        };

        // Filter out stopwords
        return words.Where(word => !stopWords.Contains(word)).ToList();
    }
}