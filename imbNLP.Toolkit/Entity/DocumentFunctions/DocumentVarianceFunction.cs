using imbNLP.Toolkit.Documents;
using System;

namespace imbNLP.Toolkit.Entity.DocumentFunctions
{

    /// <summary>
    /// Variance of distinct textual tokens' frequencies
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Entity.DocumentFunctions.DocumentStatisticFunction" />
    public class DocumentVarianceFunction : DocumentStatisticFunction
    {
        public override double Compute(TextDocumentLayerCollection document, String documentSet)
        {
            return stats[documentSet][document.name].varianceFreq;
        }

        public override double Compute(TextDocumentLayerCollection document, string parentID)
        {
            throw new NotImplementedException();
        }
    }

}