# Getting started

## Project Structure 
```
📂 SemanticSimilarityAnalysis
│
├── 📂 SemanticSimilarityAnalysis.Proj/        (Main project)
│   ├── 📂 Dependencies/                      (Dependencies)
│   ├── 📂 Datasets/                          (Ignored in version control)
│   ├── 📂 Extensions/                        (Extension methods)
│   ├── 📂 Helpers/                           (Helper classes)
│   ├── 📂 Interfaces/                        (Interface defs)
│   ├── 📂 Models/                            (Data models)
│   ├── 📂 Outputs/                           (Output files)
│   ├── 📂 Pipelines/                         (Data pipelines)
│   ├── 📂 Services/                          (Service classes)
│   ├── 📂 Utils/                             (Utility functions)
│   ├── 📜 Accord.dll.config                  (Accord.NET config)
│   ├── 📜 appsettings.json                   (Ignored in version control)
│   └── 📜 Program.cs                         (Main entry point)
│
└── 📂 SemanticSimilarityAnalysis.Test/        (Test project)
```

**Figure 1. Solution structure diagram showing the separation of concerns between main project components and test infrastructure.**

## Installation
1. Clone the repository
```
git clone https://github.com/ali-raza-166/embeders-semantic-analysis.git
```
2. Create an appsettings.json file in the correct location within the project folder structure:

```
SemanticSimilarityAnalysis/SemanticSimilarityAnalysis.Proj/appsettings.json
```

3. Add configuration details (e.g., API keys, model names) by using the following template: 
```
{
  "OpenAI": {
    "Model": "your-openai-model-name",
    "ChatModel": "your-openai-chat-model-name",
    "ApiKey": "your-openai-api-key"
  },
  "Pinecone": {
    "ApiKey": "your-pinecone-api-key"
  }
}
```

### Examples for the Model and Chat Model
✅ Example 1: Using gpt-4o and text-embedding-3-small
```
{
  "OpenAI": {
    "Model": "text-embedding-3-small",
    "ChatModel": "gpt-4o",
    "ApiKey": "your-openai-api-key"
  },
  "Pinecone": {
    "ApiKey": "your-pinecone-api-key"
  }
}
```

✅ Example 2: Using gpt-3.5-turbo and text-embedding-ada-002
```
{
  "OpenAI": {
    "Model": "text-embedding-ada-002",
    "ChatModel": "gpt-3.5-turbo",
    "ApiKey": "your-openai-api-key
  },
  "Pinecone": {
    "ApiKey": "your-pinecone-api-key"
  }
}
```

[View some embedding models and chat models that can be used for the program in OpenaiModels.md](./OpenaiModels.md)

4. Navigate the project directory
```
cd ./SemanticSimilarityAnalysis.Proj/
```

5. Build the project by one of the following ways:
   
   a. Build -> Build Solution or Ctrl + Shift + B (Windows).

   b. Enter the following command from the CMD.
```
dotnet build
```

## Two Main Parts of the Project

This project consists of two main parts:

1. **Command Line Helper for Semantic Similarity Analysis**
   The CommandLineHelper class is a central component of the Semantic Similarity Analysis tool, designed to handle command-line interactions and execute various commands for analyzing   
   semantic similarity. This helper class processes user input, performs the required analysis, and exports results to CSV files. It supports commands for comparing words, PDFs, and 
   datasets, making it a versatile tool for semantic analysis tasks. [See Semantic Similarity Analysis in CommandLine.md](./CommandLine.md)

3. **Retrieval-Augmented Generation (RAG) Implementation**  
   The second part focuses on the RAG implementation in this project. It covers concepts from general RAG principles to **mRAG** (multi-lingual RAG) and chatbot integration using a 
   customized dataset. [See more about RAG in DimReductionPipelinesAndRAG.md](./DimReductionPipelinesAndRAG.md)



