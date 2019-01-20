using imbNLP.Toolkit.Processing;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Functions
{

    /// <summary>
    /// 
    /// </summary>
    public interface IVectorSimilarityFunction
    {
        /// <summary>
        /// Higher performance method that utilizes index hashtable within <see cref="WeightDictionary"/> instances to perform computation faster
        /// </summary>
        /// <param name="vectorA">The vector a.</param>
        /// <param name="vectorB">The vector b.</param>
        /// <returns></returns>
        Double ComputeSimilarity(WeightDictionary vectorA, WeightDictionary vectorB);

        /// <summary>
        /// Lower-level method with lower performances
        /// </summary>
        /// <param name="vectorA">The vector a.</param>
        /// <param name="vectorB">The vector b.</param>
        /// <returns></returns>
      //  Double ComputeSimilarity(IEnumerable<WeightDictionaryEntry> vectorA, IEnumerable<WeightDictionaryEntry> vectorB);
    }

}