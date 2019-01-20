namespace imbNLP.Toolkit.Entity.DocumentFunctions
{
    /*

    /// <summary>
    /// Iterative function that selects page with highest number of yet unknown text tokens in each iteration, therefore maximizes number of tokens used for classification
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Entity.DocumentFunctions.DocumentFunctionBase" />
    public class DocumentTermGain : DocumentFunctionBase
    {
        public DocumentTermGain()
        {

        }


        protected Dictionary<String, Dictionary<Int32, List<String>>> knownTerms = new Dictionary<String, Dictionary<Int32, List<String>>>();

        /// <summary>
        /// Computes the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="documentSet">The document set.</param>
        /// <returns></returns>
        public override double Compute(TextDocumentLayerCollection document, TextDocumentSet documentSet)
        {
            if (!knownTerms[documentSet.name].ContainsKey(documentSet.Count))
            {
                knownTerms[documentSet.name].Add(documentSet.Count, new List<string>());

                foreach (TextDocumentLayerCollection tx in sets[documentSet.name])
                {
                    if (!documentSet.Any(x => x.name == tx.name))
                    {
                        knownTerms[documentSet.name][documentSet.Count].AddRange(setTokens[documentSet.name][tx], true);
                    }
                }
            }

            var known = knownTerms[documentSet.name][documentSet.Count];

            Int32 difCount = setTokens[documentSet.name][document].GetDifference(known).Count;

            return difCount;

        }

        public Dictionary<String, Dictionary<TextDocumentLayerCollection, List<String>>> setTokens { get; set; } = new Dictionary<string, Dictionary<TextDocumentLayerCollection, List<string>>>();

        public Dictionary<String, TextDocumentSet> sets { get; set; } = new Dictionary<string, TextDocumentSet>();

        public override void Learn(IEnumerable<TextDocumentSet> documentSets)
        {
            knownTerms.Clear();

            sets.Clear();
            foreach (TextDocumentSet s in documentSets)
            {
                sets.Add(s.name, s);
                knownTerms.Add(s.name, new Dictionary<Int32, List<string>>());
                setTokens.Add(s.name, new Dictionary<TextDocumentLayerCollection, List<string>>());
                foreach (TextDocumentLayerCollection d in s)
                {
                    setTokens[s.name].Add(d, d.ToString().getTokens(true, true, true, false, 4).Distinct().ToList());
                }
            }
        }
    }
    */
}