## Helpers

The `Helpers` folder contains utility classes designed to simplify common tasks in the project. Below is a brief overview of each helper and its functionality:

### 1. **CommandLineHelper**
- **Purpose**: Facilitates the execution of semantic similarity analysis tasks via command-line arguments.
- **Functionality**:
  - Parses and validates command-line inputs for various tasks (e.g., comparing words, PDFs, or datasets).
  - Handles user interaction for missing or invalid inputs.
  - Supports default directories for input and output files.
  - Executes commands for:
    - **Words vs. Words**: Compares two lists of words and exports similarity results to a CSV file.
    - **Words vs. PDFs**: Compares a list of words with text extracted from PDF documents and exports results to a CSV file.
    - **Words vs. Dataset**: Compares a list of words with embeddings generated from a dataset (e.g., CSV file) and exports results to a CSV file.
  - Provides a detailed help message for command usage and examples.

[See more in CommandLine.md](./CommandLine.md)

---

### 2. **CSVHelper**
- **Purpose**: Handles reading from and writing to CSV files, including extracting data, exporting similarity results, and managing reduced dimensionality data.
- **Functionality**:
  - **Read CSV Fields**: Reads the header row of a CSV file and returns a list of field names.
  - **Extract Records**: Extracts records from a CSV file based on specified fields, returning a list of `MultiEmbeddingRecord` objects.
  - **Export to CSV**: Exports similarity results (e.g., `SimilarityPlotPoint` objects) to a CSV file, with support for custom headers and data formatting.
  - **Export Reduced Dimensionality Data**: Exports reduced dimensionality data (e.g., PCA results) to a CSV file, with headers for dimensions and corresponding values.
  - **Determine Rows to Process**: Determines the number of rows to process from a CSV file based on user input or default values.
  - **Export All Phrases to CSV**: Exports phrase similarity data for multiple files into a single CSV file, separating each file's data with empty rows for clarity.
- **Key Classes**

  *1. **SimilarityPlotPoint***
    - **Purpose**: Represents a point in a similarity plot, used to store and organize similarity scores between embeddings for export to CSV files.
    - **Properties**:
      - `Label`: The label for the point (e.g., a word or phrase).
      - `Similarities`: A dictionary where:
        - Each key represents an input ID (e.g., "Input1") from the list of inputs (words)
        - Each value is the similarity score between that input and the `Label`.
    
    This structure simplifies organizing and exporting data to CSV files.
  
  *2. **MultiEmbeddingRecord***
    - **Purpose**: Represents a record that stores multiple embeddings (vector data) along with associated attributes. This class is designed to manage embeddings for different fields, where each field can have multiple vectors.
    - **Properties**:
      - **Attributes**: A dictionary of key-value pairs representing the record's attributes (e.g., fields like "title", "description").
      - **Vectors**: A dictionary mapping field names (e.g., "title", "description") to lists of `VectorData` objects (embeddings for that field).
    - **Methods**:
      - **AddEmbedding**: Adds an embedding (`VectorData`) for a specific field to the `Vectors` dictionary.

    - **Workflow**:
      1. **Before Extracting Embeddings**:
         - The `Attributes` property contains the raw data (e.g., text or metadata) for each field in the record.
         - The `Vectors` property is initialized as empty or contains pre-existing embeddings.
      
      2. **After Extracting Embeddings**:
         - Use the `AddEmbedding` method to add embeddings for a specific field (e.g., "title" or "description").
         - The `Vectors` dictionary is populated with embeddings generated from the selected attribute's value.
---

### 3. **JsonHelper**
- **Purpose**: Manages JSON data serialization and deserialization.
- **Functionality**:
  - Converts objects to JSON strings for storage or transmission.
  - Parses JSON strings into objects for easy data access and .

---

### 4. **PdfHelper**
- **Purpose**: Facilitates reading and extracting text from PDF files.
- **Functionality**:
  - Extracts text content from PDF documents.
  - Useful for processing documents in PDF format.

---

### 5. **TextHelper**
- **Purpose**: Provides utilities for text processing and manipulation.
- **Functionality**:
  - Cleans and preprocesses text (e.g., removing stopwords, punctuation).
  - Splits text into sentences or words.

---

### 6. **ScatterPlotHelper** *(scatter_plot.py)*
- **Purpose**: Generates scatter plots from CSV data for visualizing word embeddings in 2D space.
- **Functionality**:
  - Reads a CSV file containing word embeddings (e.g., `String`, `Dim1`, `Dim2`).
  - Creates a scatter plot with words annotated on their corresponding `Dim1` and `Dim2` coordinates.
  - Saves the plot as an image file (e.g., PNG) to the specified output path.
  - Automatically creates the output directory if it doesn't exist.
  - Handles errors gracefully, such as missing columns or invalid file paths.
