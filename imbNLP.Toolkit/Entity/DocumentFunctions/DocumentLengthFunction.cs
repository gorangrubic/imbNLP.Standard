using System;

namespace imbNLP.Toolkit.Entity.DocumentFunctions
{

    /// <summary>
    /// Total count of non-distinct textual tokens, extracted from the document
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Entity.DocumentFunctions.DocumentStatisticFunction" />
    public class DocumentLengthFunction : DocumentStatisticFunction
    {
        public override double Compute(TextDocumentLayerCollection document, String documentSet)
        {
            return stats[documentSet][document.name].TotalScore;
        }
    }

}