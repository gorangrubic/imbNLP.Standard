using imbNLP.Toolkit.Vectors;
using System;

namespace imbNLP.Toolkit.Feature
{

    public abstract class FeatureSpaceDimensionBase
    {

        /// <summary>
        /// Computes the dimension value for the given vector
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns></returns>
        public abstract double ComputeDimension(IVector vector, Int32 d = 0);

    }

}