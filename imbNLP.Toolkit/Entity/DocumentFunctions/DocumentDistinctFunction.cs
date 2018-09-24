using imbSCI.Core.extensions.text;
using imbSCI.DataComplex.special;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Entity.DocumentFunctions
{

    /// <summary>
    /// Number of distinct textual tokens, extracted from the document
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Entity.DocumentFunctions.DocumentStatisticFunction" />
    public class DocumentDistinctFunction : DocumentStatisticFunction
    {
        public override double Compute(TextDocumentLayerCollection document, TextDocumentSet documentSet)
        {
            return stats[documentSet.name][document.name].Count;
        }
    }

}