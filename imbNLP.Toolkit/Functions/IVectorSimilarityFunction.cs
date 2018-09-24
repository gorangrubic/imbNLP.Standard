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
        Double ComputeSimilarity(IEnumerable<WeightDictionaryEntry> vectorA, IEnumerable<WeightDictionaryEntry> vectorB);
    }

}