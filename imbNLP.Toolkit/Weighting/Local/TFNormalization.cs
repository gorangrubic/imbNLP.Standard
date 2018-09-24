using imbNLP.Toolkit.Space;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Weighting.Local
{

    /// <summary>
    /// Method of TF in document normalization
    /// </summary>
    public enum TFNormalization
    {
        /// <summary>
        /// Maximum raw term frequency
        /// </summary>
        divisionByMaxTF,

        /// <summary>
        /// The square root of summation of square frequencies
        /// </summary>
        squareRootOfSquareSum,
    }

}