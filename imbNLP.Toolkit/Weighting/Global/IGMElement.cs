using imbNLP.Toolkit.Space;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Weighting.Global
{
    /// <summary>
    /// Inverse Gravity Moment
    /// </summary>
    /// <remarks>
    /// <para>
    /// Related paper: Chen, Kewen, Zuping Zhang, Jun Long, and Hao Zhang. 2016. “Turning from TF-IDF to TF-IGM for Term Weighting in Text Classification.” Expert Systems With Applications 66. Elsevier Ltd: 245–60. doi:10.1016/j.eswa.2016.09.009.
    /// </para>
    /// </remarks>
    /// <seealso cref="imbNLP.Toolkit.Weighting.Elements.ModelElementBase" />
    public class IGMElement : GlobalElementBase
    {
        protected Dictionary<String, Double> IGM_index { get; set; } = new Dictionary<string, double>();

        protected Dictionary<String, Dictionary<SpaceLabel, Int32>> TermClassFrequency { get; set; } = new Dictionary<string, Dictionary<SpaceLabel, int>>();

        protected Dictionary<String, List<KeyValuePair<SpaceLabel, Int32>>> TermClassRank { get; set; } = new Dictionary<String, List<KeyValuePair<SpaceLabel, Int32>>>();

        protected Dictionary<SpaceDocumentModel, SpaceLabel> DocumentVsLabel { get; set; } = new Dictionary<SpaceDocumentModel, SpaceLabel>();

        public Double l { get; set; } = 7;

        public IGMElement()
        {
            shortName = "IGM";
            description = "Inverted Gravity Moment, Related paper: Chen, Kewen, Zuping Zhang, Jun Long, and Hao Zhang. 2016. “Turning from TF-IDF to TF-IGM for Term Weighting in Text Classification.” Expert Systems With Applications 66. Elsevier Ltd: 245–60. doi:10.1016/j.eswa.2016.09.009.";

        }

        /// <summary>
        /// Returns Inverse Gravity Moment of the term
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="document">The document.</param>
        /// <param name="space">The space.</param>
        /// <returns></returns>
        public override double GetElementFactor(string term, SpaceModel space, SpaceLabel label = null)
        {
            if (!IsEnabled) return 1;
            if (!IGM_index.ContainsKey(term)) return 0;
            return 1 + (l * IGM_index[term]);
        }

        public override void Describe(ILogBuilder logger)
        {
            base.Describe(logger);
            logger.AppendPair("Landa (λ)", l.ToString("F3"), true, "\t\t\t");
        }


        /// <summary>
        /// Prepares the model - computes IGM for each term
        /// </summary>
        /// <param name="space">The space.</param>
        /// <exception cref="ArgumentException">A document is already assigned to a label! This model is not applicable for multi-label problem.</exception>
        public override void PrepareTheModel(SpaceModel space)
        {
            if (!IsEnabled) return;

            var labels = space.labels;

            var terms = space.terms.GetTokens();

            foreach (String term in terms)
            {
                Dictionary<SpaceLabel, Int32> ClassFrequency = new Dictionary<SpaceLabel, Int32>();
                foreach (SpaceLabel label in labels)
                {
                    ClassFrequency.Add(label, 0);
                }

                TermClassFrequency.Add(term, ClassFrequency);
                IGM_index.Add(term, 0);
            }

            foreach (SpaceLabel label in labels)
            {
                List<SpaceDocumentModel> documents = space.LabelToDocumentLinks.GetAllLinked(label);
                foreach (SpaceDocumentModel document in documents)
                {
                    if (DocumentVsLabel.ContainsKey(document))
                    {
                        throw new ArgumentException("A document is already assigned to a label! This model is not applicable for multi-label problem.");
                    }
                    DocumentVsLabel.Add(document, label);
                }

                foreach (SpaceDocumentModel document in documents)
                {
                    var doc_terms = document.terms.GetTokens();
                    foreach (String term in doc_terms)
                    {
                        TermClassFrequency[term][label] += document.terms.GetTokenFrequency(term);
                    }
                }
            }

            foreach (String term in terms)
            {
                TermClassRank.Add(term, TermClassFrequency[term].OrderByDescending(x => x.Value).ToList());

                Double igm_tk_below = 0;

                Double f_ki = TermClassRank[term].Max(x => x.Value);
                Double r = 1;
                foreach (KeyValuePair<SpaceLabel, int> ranked in TermClassRank[term])
                {
                    if (ranked.Value > 0)
                    {
                        igm_tk_below += (Convert.ToDouble(ranked.Value) / f_ki) * r;
                    }
                    r++;
                }
                IGM_index[term] = 1 / igm_tk_below;
            }
        }
    }
}