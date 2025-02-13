using SemanticSimilarityAnalysis.Proj.Models;

namespace SemanticSimilarityAnalysis.Proj.Helpers.JsonHelper
{
    public class JsonHelper
    {
        // Method to read and deserialize JSON from a file
        public static MultiEmbeddingRecord GetRecordFromJson(string filePath)
        {
            // Read the JSON string from the file
            string jsonString = File.ReadAllText(filePath);

            // Deserialize the JSON string into a YourRecordType object
            var record = Newtonsoft.Json.JsonConvert.DeserializeObject<MultiEmbeddingRecord>(jsonString);

            if (record == null)
            {
                throw new InvalidOperationException("Failed to deserialize JSON into MultiEmbeddingRecord.");
            }

            return record;
        }
    }
}
