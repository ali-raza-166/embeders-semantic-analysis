# ProcessorAli Documentation

## Overview
`ProcessorAli` is a C# class that utilizes various services to perform semantic similarity analysis, embedding generation, dimensionality reduction, chatbot interactions, and retrieval-augmented generation (RAG) evaluations.

## Prerequisites
Ensure you have the following dependencies installed in your C# project:

- `LanguageDetection`
- `Microsoft.Extensions.DependencyInjection`

## Usage
### Running `ProcessorAli`
1. Initialize the required services using dependency injection.
2. Call the `RunAsync()` method to execute various tasks.

Example:
```csharp
ProcessorAli processor = new(serviceProvider);
await processor.RunAsync();
```
## Notes
While testing, uncomment only the relevant code block under each feature and keep all other code blocks commented. This ensures that each functionality is tested in isolation without conflicts.
## Features
### 1. Creating Embeddings & Computing Cosine Similarity
- Generates embeddings using `OpenAiEmbeddingService`.
- Computes cosine similarity between generated embeddings.

```csharp
var embeddingService = _serviceProvider.GetRequiredService<OpenAiEmbeddingService>();
var cosineSimService = _serviceProvider.GetRequiredService<CosineSimilarity>();

var embeddings = await embeddingService.CreateEmbeddingsAsync(inputs);
var listofEmbeddingVectors = embeddings.Select(e => e.Values).ToList();

Console.WriteLine(cosineSimService.ComputeCosineSimilarity(listofEmbeddingVectors[0], listofEmbeddingVectors[1]));
```

### 2. Dimensionality Reduction Pipelines
- Runs OpenAI and Word2Vec embedding dimensionality reduction and visualization.

```csharp
var openAiEmbeddingsDimReductionAndPlotting = _serviceProvider.GetRequiredService<OpenAiEmbeddingsDimReductionAndPlotting>();
var word2VecEmbeddingsDimReductionAndPlotting = _serviceProvider.GetRequiredService<Word2VecEmbeddingsDimReductionAndPlotting>();

await openAiEmbeddingsDimReductionAndPlotting.RunPipelineAsync(inputs);
word2VecEmbeddingsDimReductionAndPlotting.RunPipeline(inputs);
```

### 3. Pinecone Vector Database Setup & Querying
#### Setting Up Pinecone
**Note:** The `namespace` name and `index` name should be all small letters and separated by a hyphen
```csharp
var pineconeSetupService = _serviceProvider.GetRequiredService<PineconeSetup>();
string namespaceName = "your-namespace-name";  
string indexName = "your-index-name"; 
await pineconeSetupService.RunAsync(inputs, indexName, namespaceName);
```

#### Querying Pinecone
**Note** 'QueryEmbeddingsAsync' has following args
- The `query` parameter specifies the search input.
- `indexName` refers to the Pinecone index being queried.
- `namespaceName` helps organize data within the index.
- `3` determines the number of top matching results to return.
- `"en"` specifies the language filter for embeddings that will be extracted from pinecone. Its default value is "en"
- While testing, uncomment this block and ensure other query-related blocks remain commented.
```csharp
var pineconeService = _serviceProvider.GetRequiredService<PineconeService>();
string query = "Who is Dr. Dobric?";
var pineconeTopKparagraphs = await pineconeService.QueryEmbeddingsAsync(query, indexName, namespaceName, 3, "en");
```

### 4. Chatbot Service
```csharp
var chatbotService = _serviceProvider.GetRequiredService<ChatbotService>();
await chatbotService.StartChatAsync(indexName, namespaceName);
```

### 5. RAG Pipeline Evaluation
- Retrieves relevant documents and evaluates accuracy using cosine similarity and ROUGE scores.
 **Notes**
- This code allows batch querying in Pinecone.
- A list of queries is passed as input.
- The response contains answers for all queries from the specified index and namespace in Pinecone.
- While testing, uncomment this block and ensure other Pinecone-related blocks remain commented.
```csharp
var ragPipeline = _serviceProvider.GetRequiredService<RagPipeline>();
List<string> inputQueries = new() { "your-first-query", "your-second-query", "your-third-query" };
List<string> generatedResponses = await ragPipeline.BatchRetrieveAndGenerateResponsesAsync(inputQueries, indexName, namespaceName, 3);

foreach (var response in generatedResponses)
{
    Console.WriteLine($"Generated Answer: {response}");
}
```

### 6. Word2Vec Similarity Testing
**Note**
- Download the dataset from https://www.kaggle.com/datasets/pkugoodspeed/nlpword2vecembeddingspretrained?resource=download&select=glove.6B.300d.txt and place inside Datasets directory with name `glove.6B.300d.txt`
```csharp
string filePath = Path.Combine("Datasets", "glove.6B.300d.txt");
var word2VecService = new Word2VecService(filePath);

var vector1 = word2VecService.GetPhraseVector("machine learning revolution");
var vector2 = word2VecService.GetPhraseVector("law and legal regulations");

var cosineSimilarity = new CosineSimilarity();
var similarityValue = cosineSimilarity.ComputeCosineSimilarity(vector1.ToList(), vector2.ToList());
Console.WriteLine($"Cosine Similarity using Word2Vec: {similarityValue}");
```

## Error Handling
Any exceptions encountered during execution are caught and logged to the console.

```csharp
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Contributing
To contribute, fork the repository, create a new branch, and submit a pull request.

## License
This project is licensed under [MIT License](LICENSE).
