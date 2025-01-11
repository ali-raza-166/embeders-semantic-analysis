
namespace SemanticSimilarityAnalysis.Proj.Services
{
    public class CsvExporter
    {
        /// <summary>
        /// Exports a list of strings to a CSV file.
        /// </summary>
        /// <param name="data">The data to export.</param>
        /// <param name="filePath">The path where the CSV file will be saved.</param>
        public static async Task ExportToCsvAsync(List<string> data, string filePath)
        {
            try
            {
                await using var writer = new StreamWriter(filePath);
                foreach (var line in data)
                {
                    await writer.WriteLineAsync(line);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting to CSV: {ex.Message}");
            }
        }
    }
}