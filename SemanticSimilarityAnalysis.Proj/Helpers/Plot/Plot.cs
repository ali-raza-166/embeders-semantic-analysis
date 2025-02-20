public class RecordSimilarityData
{
    public string RecordId { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float Similarity1 { get; set; }
    public float Similarity2 { get; set; }
    public float Similarity3 { get; set; }
}
public async Task<List<RecordEmbedding>> loadRecordEmbeddingsFromFile(string filePath, string attribute)
{
    using var reader = new StreamReader(filePath);
    var json = await reader.ReadToEndAsync();
    var records = JsonConvert.DeserializeObject<List<RecordEmbedding>>(json);

    return records.Where(r => r.Attribute == attribute).ToList();
}
public float computeCosineSimilarity(float[] vectorA, float[] vectorB)
{
    float dotProduct = 0, magnitudeA = 0, magnitudeB = 0;

    for (int i = 0; i < vectorA.Length; i++)
    {
        dotProduct += vectorA[i] * vectorB[i];
        magnitudeA += vectorA[i] * vectorA[i];
        magnitudeB += vectorB[i] * vectorB[i];
    }

    magnitudeA = (float)Math.Sqrt(magnitudeA);
    magnitudeB = (float)Math.Sqrt(magnitudeB);

    return magnitudeA == 0 || magnitudeB == 0 ? 0 : dotProduct / (magnitudeA * magnitudeB);
}
