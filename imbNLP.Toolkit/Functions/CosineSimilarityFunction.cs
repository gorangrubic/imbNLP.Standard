using imbNLP.Toolkit.Processing;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Functions
{

    /// <summary>
    /// Cosine similarity
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Functions.SimilarityFunctionBase" />
    public class CosineSimilarityFunction : SimilarityFunctionBase, IVectorSimilarityFunction
    {
        public CosineSimilarityFunction() { }

        /// <summary>
        /// Computes the similarity.
        /// </summary>
        /// <param name="vectorA">The vector a.</param>
        /// <param name="vectorB">The vector b.</param>
        /// <returns></returns>
        public override double ComputeSimilarity(IEnumerable<WeightDictionaryEntry> vectorA, IEnumerable<WeightDictionaryEntry> vectorB)
        {

            WeightDictionaryEntryPairs termPairs = new WeightDictionaryEntryPairs(vectorA, vectorB);
            if (termPairs.Count == 0) return 0;

            Double above = 0;
            Double belowA = 0;
            Double belowB = 0;

            foreach (WeightDictionaryEntryPair pair in termPairs)
            {
                above += pair.weight_A * pair.weight_B;
                belowA += pair.weight_A * pair.weight_A;
                belowB += pair.weight_B * pair.weight_B;
            }

            belowA = Math.Sqrt(belowA);
            belowB = Math.Sqrt(belowB);

            return above / (belowA * belowB);
        }
    }

}