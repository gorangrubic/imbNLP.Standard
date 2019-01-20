using imbNLP.Toolkit.Processing;
using System;

namespace imbNLP.Toolkit.Functions
{

    /// <summary>
    /// Cosine similarity
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Functions.SimilarityFunctionBase" />
    public class CosineSimilarityFunction : SimilarityFunctionBase, IVectorSimilarityFunction
    {
        public CosineSimilarityFunction() { }


        public override double ComputeSimilarity(WeightDictionary vectorA, WeightDictionary vectorB)
        {
            WeightDictionaryEntryPairs termPairs = new WeightDictionaryEntryPairs(vectorA, vectorB);
            if (termPairs.Count == 0) return 0;
            return Compute(termPairs);
        }


        private Double Compute(WeightDictionaryEntryPairs termPairs)
        {
            Double above = 0;
            Double belowA = 0;
            Double belowB = 0;

            foreach (WeightDictionaryEntryPair pair in termPairs)
            {
                above += pair.weight_A * pair.weight_B;
                belowA += pair.weight_A_sq;
                belowB += pair.weight_B_sq;
            }

            belowA = Math.Sqrt(belowA);
            belowB = Math.Sqrt(belowB);



            return above / (belowA * belowB);
        }


        ///// <summary>
        ///// Computes the similarity.
        ///// </summary>
        ///// <param name="vectorA">The vector a.</param>
        ///// <param name="vectorB">The vector b.</param>
        ///// <returns></returns>
        //public override double ComputeSimilarity(IEnumerable<WeightDictionaryEntry> vectorA, IEnumerable<WeightDictionaryEntry> vectorB)
        //{

        //    WeightDictionaryEntryPairs termPairs = new WeightDictionaryEntryPairs(vectorA, vectorB);
        //    if (termPairs.Count == 0) return 0;


        //}
    }

}