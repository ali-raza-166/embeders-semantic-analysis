import os
import sys
from random import random

import pandas as pd # type: ignore
import matplotlib.pyplot as plt # type: ignore

def plot_scatter(csv_path, output_path):
    try:
        # Ensure the directory exists
        output_dir = os.path.dirname(output_path)
        if not os.path.exists(output_dir):
            os.makedirs(output_dir)  # Create the directory if it doesn't exist
            print(f"Created directory: {output_dir}")

        # Load CSV data
        df = pd.read_csv(csv_path)

        # Ensure there are at least three columns (words, Dim1, Dim2)
        if df.shape[1] < 3:
            raise ValueError("CSV file must have at least three columns: String, Dim1, Dim2.")

        # Extract words, Dim1, and Dim2 for plotting
        words = df['String']  # Column containing words
        dim1 = df['Dim1']     # Column containing Dim1 values
        dim2 = df['Dim2']     # Column containing Dim2 values


        # Generate scatter plot
        plt.figure(figsize=(10, 8))
        plt.scatter(dim1, dim2, alpha=0.7, color='blue', edgecolors='black')

        # Annotate points with words (first column values)
        for i, word in enumerate(words):
            plt.text(dim1[i], dim2[i], word, fontsize=8, ha='right', va='bottom')

        plt.xlabel("Dim1")
        plt.ylabel("Dim2")
        plt.title("Scatter Plot of Words Based on Dim1 and Dim2")

        # Save plot to output path
        plt.savefig(output_path, dpi=300)
        plt.close()

        print(f"Plot saved to: {output_path}")
    except Exception as e:
        print(f"Error during plotting: {e}")

# Main execution
if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python scatter_plot.py <input_csv_path> <output_image_path>")
        sys.exit(1)

    csv_file = sys.argv[1]
    output_file = sys.argv[2]

    if os.path.isdir(output_file):
        output_file = os.path.join(output_file, 'scatter_plot.png'+ random.randint(0,255) )
        print(f"Output path is a directory, adding default file name: {output_file}")

    plot_scatter(csv_file, output_file)
