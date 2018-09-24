using imbNLP.Toolkit.Space;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Weighting.Local
{

    public enum TFComputation
    {
        normal,

        squareRooted,

        glasgow,

        /// <summary>
        /// The modified tf: Sabbah, Thabit, Ali Selamat, Md Hafiz Selamat, Fawaz S. Al-Anzi, Enrique Herrera Viedma, Ondrej Krejcar, and Hamido Fujita. 2017. “Modified Frequency-Based Term Weighting Schemes for Text Classification.” Applied Soft Computing Journal 58. Elsevier B.V.: 193–206. doi:10.1016/j.asoc.2017.04.069.
        /// </summary>
        modifiedTF,
    }

}