using imbSCI.Core.extensions.text;
using imbSCI.DataComplex.special;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Entity.DocumentFunctions
{

    /// <summary>
    /// Total count of non-distinct textual tokens, extracted from the document
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Entity.DocumentFunctions.DocumentStatisticFunction" />
    public class DocumentLengthFunction : DocumentStatisticFunction
    {
        public override double Compute(TextDocumentLayerCollection document, TextDocumentSet documentSet)
        {
            return stats[documentSet.name][document.name].TotalScore;
        }
    }

}