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
        //Console.WriteLine(filePath);

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
    public List<string> CleanAndSplitText(string text)
    {
        // Remove unwanted whitespace and control characters
        text = Regex.Replace(text, @"[\t\r\n\v\f\b\0\u200B\u00A0\u2028\u2029]", "");

        // Split the text by commas
        List<string> segments = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(segment => segment.Trim()) // Remove leading/trailing whitespace
                                     .ToList();

        // Clean each segment (remove punctuation, convert to lowercase, etc.)
        var cleanedSegments = new List<string>();
        foreach (var segment in segments)
        {
            // Remove punctuation and special characters using regex
            string cleanedText = Regex.Replace(segment, @"[^\w\s]", "");

            // Convert text to lowercase
            cleanedText = cleanedText.ToLower();

            cleanedSegments.Add(cleanedText);
        }

        return cleanedSegments;
    }

    /// <summary>
    /// Checks if the input is a valid file path and has a .txt extension.
    /// </summary>
    /// <param name="input"></param>
    /// <returns>True if the input is a valid file path and has a .txt extension; otherwise, false.</returns>
    public bool IsTextFilePath(string input)
    {
        return Path.GetExtension(input).Equals(".txt", StringComparison.OrdinalIgnoreCase);
    }
}