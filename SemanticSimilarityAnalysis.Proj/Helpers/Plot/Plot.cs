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
public async Task<List<RecordSimilarityData>> prepareDataForPlotting(
    List<string> records,
    List<string> inputs,
    List<string> inputDescriptions,
    string attribute,
    string recordsJsonFilePath)
{
    var recordEmbeddings = await loadRecordEmbeddingsFromFile(recordsJsonFilePath, attribute);
    var inputEmbeddings = await embeddingService.createEmbeddingsAsync(inputs);
    var similarityResults = new List<RecordSimilarityData>();

    foreach (var recordEmbedding in recordEmbeddings)
    {
        var similarities = new List<float>();

        foreach (var inputEmbedding in inputEmbeddings)
        {
            var similarity = computeCosineSimilarity(recordEmbedding.Values, inputEmbedding.Values);
            similarities.Add(similarity);
        }

        var recordData = new RecordSimilarityData
        {
            RecordId = recordEmbedding.Id,
            X = similarities[0],
            Y = similarities[1],
            Z = similarities[2],
            Similarity1 = similarities[0],
            Similarity2 = similarities[1],
            Similarity3 = similarities[2]
        };

        similarityResults.Add(recordData);
    }

    return similarityResults;
}
