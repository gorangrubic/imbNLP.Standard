using imbNLP.Toolkit.Space;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Weighting.Global
{
    /// <summary>
    /// Provides Inverse Class Frequency for a term
    /// </summary>
    /// <remarks>
    /// <para>
    /// Related paper: Ren, Fuji, and Mohammad Golam Sohrab. 2013. “Class-Indexing-Based Term Weighting for Automatic Text Classification.” Information Sciences 236. Elsevier Inc.: 109–25. doi:10.1016/j.ins.2013.02.029.
    /// </para>
    /// </remarks>
    /// <seealso cref="imbNLP.Toolkit.Weighting.Elements.ModelElementBase" />
    public class ICFElement : GlobalElementBase
    {
        public ICFElement()
        {
            shortName = "ICF";
            description = "Inverted Class Frequency, Related paper: Ren, Fuji, and Mohammad Golam Sohrab. 2013. “Class-Indexing-Based Term Weighting for Automatic Text Classification.” Information Sciences 236. Elsevier Inc.: 109–25. doi:10.1016/j.ins.2013.02.029.";
        }

        /// <summary>
        /// Term vs IDF factor
        /// </summary>
        /// <value>
        /// The index of the idf.
        /// </value>
        public Dictionary<String, Double> ICF_index { get; set; } = new Dictionary<string, double>();

        /// <summary>
        /// Returns Inverse Class Frequency - ICF
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="document">The document.</param>
        /// <param name="space">The space.</param>
        /// <returns></returns>
        public override double GetElementFactor(string term, SpaceModel space, SpaceLabel label = null)
        {
            if (!IsEnabled) return 1;
            if (!ICF_index.ContainsKey(term)) return 0;

            return ICF_index[term];
        }

        public override void PrepareTheModel(SpaceModel space)
        {
            if (!IsEnabled) return;

            var labels = space.labels;

            Dictionary<String, List<SpaceLabel>> TermToLabelIndex = new Dictionary<string, List<SpaceLabel>>();

            var terms = space.terms.GetTokens();

            foreach (String term in terms)
            {
                TermToLabelIndex.Add(term, new List<SpaceLabel>());
            }

            foreach (SpaceLabel label in labels)
            {
                List<SpaceDocumentModel> documents = space.LabelToDocumentLinks.GetAllLinked(label);

                foreach (SpaceDocumentModel document in documents)
                {
                    var termsInDocument = document.terms.GetTokens();
                    foreach (String termInDocument in termsInDocument)
                    {
                        if (!TermToLabelIndex[termInDocument].Contains(label))
                        {
                            TermToLabelIndex[termInDocument].Add(label);
                        }
                    }
                }
            }

            Double N = labels.Count;

            foreach (String term in terms)
            {
                Double CF_t = TermToLabelIndex[term].Count;
                Double ICF_t = 0;
                if (CF_t == 0)
                {

                }
                else
                {
                    ICF_t = Math.Log(1 + (N / CF_t));
                }
                ICF_index.Add(term, ICF_t);
            }
        }
    }
}