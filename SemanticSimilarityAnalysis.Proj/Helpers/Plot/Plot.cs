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
