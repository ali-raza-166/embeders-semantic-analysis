using System.Diagnostics;

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
public void plot3D(List<RecordSimilarityData> similarityResults, List<string> inputDescriptions)
{
    var xValues = new List<float>();
    var yValues = new List<float>();
    var zValues = new List<float>();
    var hoverInfo = new List<string>();

    foreach (var recordData in similarityResults)
    {
        xValues.Add(recordData.X);
        yValues.Add(recordData.Y);
        zValues.Add(recordData.Z);

        hoverInfo.Add($"{recordData.RecordId}<br>{inputDescriptions[0]}: {recordData.Similarity1:F2}<br>" +
                      $"{inputDescriptions[1]}: {recordData.Similarity2:F2}<br>" +
                      $"{inputDescriptions[2]}: {recordData.Similarity3:F2}");
    }

    var scatter3D = new Scatter3d
    {
        Mode = "markers",
        X = xValues,
        Y = yValues,
        Z = zValues,
        Text = hoverInfo,
        Marker = new Marker
        {
            Size = 6,
            Color = xValues,
            Colorscale = "Viridis",
            Showscale = true
        }
    };

    var layout = new Layout
    {
        Title = "3D Similarity Plot",
        Scene = new Scene
        {
            Xaxis = new Xaxis { Title = inputDescriptions[0] },
            Yaxis = new Yaxis { Title = inputDescriptions[1] },
            Zaxis = new Zaxis { Title = inputDescriptions[2] }
        }
    };

    var plot = new PlotlyChart
    {
        Data = new List<Trace> { scatter3D },
        Layout = layout
    };

    plot.Show();
}
public async Task generateAndPlotData()
{
    List<string> records = new List<string> { "The Shawshank Redemption", "The Godfather", "The Dark Knight", "Inception", "Pulp Fiction" };
    List<string> inputs = new List<string> { "redemption", "justice", "family" };
    List<string> inputDescriptions = new List<string> { "Drama", "Action", "Thriller" };
    string attribute = "Overview";
    string recordsJsonFilePath = "path/to/your/records.json";

    var similarityResults = await prepareDataForPlotting(records, inputs, inputDescriptions, attribute, recordsJsonFilePath);
    plot3D(similarityResults, inputDescriptions);
}
