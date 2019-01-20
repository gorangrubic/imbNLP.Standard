using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Vectors;
using System;

namespace imbNLP.Toolkit.Feature
{

    public abstract class FeatureSpaceDimensionBase
    {

        public abstract double ComputeDimension(WeightDictionary vector, Int32 d = 0);

        /// <summary>
        /// Computes the dimension value for the given vector
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns></returns>
        public abstract double ComputeDimension(IVector vector, Int32 d = 0);

    }

}