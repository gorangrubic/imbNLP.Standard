using imbSCI.Core.extensions.text;
using imbSCI.DataComplex.special;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Entity.DocumentFunctions
{

    /// <summary>
    /// Variance of distinct textual tokens' frequencies
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Entity.DocumentFunctions.DocumentStatisticFunction" />
    public class DocumentVarianceFunction : DocumentStatisticFunction
    {
        public override double Compute(TextDocumentLayerCollection document, TextDocumentSet documentSet)
        {
            return stats[documentSet.name][document.name].varianceFreq;
        }
    }

}