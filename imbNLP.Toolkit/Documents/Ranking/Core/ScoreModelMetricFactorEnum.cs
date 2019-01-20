using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Space;
using imbSCI.Core.extensions.text;
using imbSCI.Core.reporting;
using imbSCI.DataComplex.special;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Documents.Ranking.Core
{

public enum ScoreModelMetricFactorEnum
    {
        varianceFreq,
        TotalScore,
        standardDeviation,
        entropyFreq,
        avgFreq,
        Count,
        /// <summary>
        /// It can be used as pseudo random selector for testing disabled document selection method
        /// </summary>
        Ordinal,
    }
}