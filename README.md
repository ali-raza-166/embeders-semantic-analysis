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

# References 
D. Gunawan, C. A. Sembiring, and M. A. Budiman, "The implementation of cosine similarity to calculate text relevance between two documents," in 2nd International Conference on Computing and Applied Informatics 2017, J. Phys.: Conf. Ser., vol. 978, Art. no. 012120, 2018, doi: 10.1088/1742-6596/978/1/012120.

V. Ugale and S. Pawar, “PESTLE based event detection and classification,” International Journal of Research in Engineering and Technology (IJRET), vol. 4, no. 5, pp. 611–616, May 2015. [Online]. Available: http://www.ijret.org.

L. van der Maaten and G. Hinton, “Visualizing data using t-SNE,” Journal of Machine Learning Research, vol. 9, pp. 2579–2605, 2008.

P. Lewis et al., “Retrieval-augmented generation for knowledge-intensive NLP tasks,” arXiv preprint arXiv:2005.11401v4 [cs.CL], 12 Apr. 2021.

A. Vaswani et al., “Attention is all you need,” in Advances in Neural Information Processing Systems (NeurIPS), vol. 30, 2017, pp. 5998–6008. [Online]. Available: https://proceedings.neurips.cc/paper/2017/file/3f5ee243547dee91fbd053c1c4a845aa-Paper.pdf.

T. G. Dietterich, “Ensemble methods in machine learning,” in Multiple Classifier Systems, J. Kittler and F. Roli, Eds., Lecture Notes in Computer Science, vol. 1857, Springer, 2000, pp. 1–15. [Online]. Available: https://doi.org/10.1007/3-540-45014-9_1.

L. van der Maaten, “Accelerating t-SNE using tree-based algorithms,” J. Mach. Learn. Res., vol. 15, pp. 3221–3245, Oct. 2014.  

C.-Y. Lin, "ROUGE: A Package for Automatic Evaluation of Summaries," in Text Summarization Branches Out, Barcelona, Spain, 2004.

N. Chirkova et al., "Retrieval-Augmented Generation in Multilingual Settings," in Proceedings of the 1st Workshop on Towards Knowledgeable Language Models (KnowLLM 2024), pp. 177–188, Aug. 16, 2024. ©2024 Association for Computational Linguistics. [Online]. Available: https://arxiv.org/abs/2407.01463

word2vec Project, Google Code Archive, 2013. [Online].Available: https://code.google.com/archive/p/word2vec/source

J. Xue, Y.-C. Wang, C. Wei, and C.-C. J. Kuo, “Word embedding dimension reduction via weakly-supervised feature selection,” unpublished.