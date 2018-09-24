using imbNLP.Toolkit.Processing;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Functions
{

    /// <summary>
    /// Base class for similarity computation functions
    /// </summary>
    public abstract class SimilarityFunctionBase : IVectorSimilarityFunction
    {
        protected SimilarityFunctionBase()
        {

        }

        /// <summary>
        /// Computes the similarity.
        /// </summary>
        /// <param name="vectorA">The vector a.</param>
        /// <param name="vectorB">The vector b.</param>
        /// <returns></returns>
        public abstract Double ComputeSimilarity(IEnumerable<WeightDictionaryEntry> vectorA, IEnumerable<WeightDictionaryEntry> vectorB);

    }


}
