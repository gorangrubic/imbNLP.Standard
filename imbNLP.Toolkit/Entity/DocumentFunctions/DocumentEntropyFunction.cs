namespace imbNLP.Toolkit.Entity.DocumentFunctions
{





    /// <summary>
    /// Entropy of distinct textual tokens' frequencies
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Entity.DocumentFunctions.DocumentStatisticFunction" />
    public class DocumentEntropyFunction : DocumentStatisticFunction
    {
        /// <summary>
        /// Computes the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="documentSet">The document set.</param>
        /// <returns></returns>
        public override double Compute(TextDocumentLayerCollection document, TextDocumentSet documentSet)
        {
            return stats[documentSet.name][document.name].entropyFreq;
        }
    }

}