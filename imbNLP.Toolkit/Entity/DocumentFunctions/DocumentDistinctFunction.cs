using System;

namespace imbNLP.Toolkit.Entity.DocumentFunctions
{

    /// <summary>
    /// Number of distinct textual tokens, extracted from the document
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Entity.DocumentFunctions.DocumentStatisticFunction" />
    public class DocumentDistinctFunction : DocumentStatisticFunction
    {
        public override double Compute(TextDocumentLayerCollection document, String documentSet)
        {
            return stats[documentSet][document.name].Count;
        }
    }

}