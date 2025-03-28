# Introduction
This project introduces a generalized model for semantic similarity analysis using cosine similarity, dimensionality reduction (PCA and t-SNE), and the Retrieval-Augmented Generation (RAG) approach. Built around the PESTLE framework, the model supports 1D, 2D, and 3D visualizations to compare words with phrases, PDFs, and datasets. High-dimensional embeddings (1536 dimensions) are reduced for effective visualization via scatter plots and heatmaps. The RAG approach integrates Pinecone for efficient storage and retrieval of embeddings, enabling accurate and contextually relevant responses from a large language model (LLM). The model also supports multilingual analysis and extends to recommendation systems for personalized suggestions. This C# .NET implementation demonstrates effective retrieval, visualization, and recommendation generation based on semantic similarity.

## Getting started
To get started, please see <a href="https://github.com/ali-raza-166/embeders-semantic-analysis/blob/main/Documentation/gettingStarted.md">this document.</a>

# Online References
https://medium.com/researchify/exploring-cosine-similarity-how-sentence-embedding-models-measure-meaning-1b047675ef8a

https://www.baeldung.com/cs/ml-similarities-in-text

https://www.geeksforgeeks.org/cnn-introduction-to-pooling-layer/

https://medium.com/@siladityaghosh/a-deep-dive-into-openais-text-embedding-ada-002-the-power-of-semantic-understanding-7072c0386f83

https://en.wikipedia.org/wiki/Principal_component_analysis

https://code.google.com/archive/p/word2vec/

https://opentsne.readthedocs.io/en/stable/tsne_algorithm.html

https://www.kaggle.com/datasets/sugataghosh/google-word2vec

https://openai.com/index/new-and-improved-embedding-model/

https://platform.openai.com/docs/guides/embeddings#:~:text=By%20default%2C%20the%20length%20of,%2Dembedding%2D3%2Dlarge%20


# References 

D. Gunawan, C. A. Sembiring, and M. A. Budiman, “The Implementation of Cosine Similarity to Calculate Text Relevance between Two Documents,” J. Phys. Conf. Ser., vol. 978, no. 1, 2018, doi: 10.1088/1742-6596/978/1/012120.

S. P. Vaishali Ugale, “Pestle Based Event Detection and Classification,” Int. J. Res. Eng. Technol., vol. 04, no. 05, eISSN: 2319-1163, 2015, doi: 10.15623/ijret.2015.0405112.

L. Van Der Maaten and G. Hinton, “Visualizing data using t-SNE,” J. Mach. Learn. Res., vol. 9, pp. 2579–2605, 2008.

P. Lewis et al., “Retrieval-augmented generation for knowledge-intensive NLP tasks,” Adv. Neural Inf. Process. Syst., vol. 2021-April, 2021.

A. Vaswani et al., “Attention is all you need,” Adv. Neural Inf. Process. Syst., vol. 2023-August, 2023.

T. G. Dietterich, “Ensemble methods in machine learning,” Lect. Notes Comput. Sci., vol. 1857 LNCS, pp. 1–15, 2000, doi: 10.1007/3-540-45014-9_1.

L. Van Der Maaten, “Accelerating t-SNE using Tree-Based Algorithms,” J. Mach. Learn. Res., vol. 15, pp. 3221–3245, 2014.

C. Y. Lin, “Rouge: A package for automatic evaluation of summaries,” Proc. Work. text Summ. branches out (WAS 2004), 2004.

N. Chirkova et al., “Retrieval-augmented generation in multilingual settings,” no. KnowLLM, pp. 177–188, 2024, [Online]. Available: http://arxiv.org/abs/2407.01463.

word2vec Project, Google Code Archive, 2013. [Online].Available: https://code.google.com/archive/p/word2vec/source, unpublished.

J. Xue, Y. C. Wang, C. Wei, and C. C. Jay Kuo, “Word embedding dimension reduction via weakly-supervised feature selection,” APSIPA Transactions on Signal and Information Processing, 2024, doi: 10.1561/116.20240046.
