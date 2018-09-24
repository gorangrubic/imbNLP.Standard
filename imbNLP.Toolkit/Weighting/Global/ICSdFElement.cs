using imbNLP.Toolkit.Space;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Weighting.Global
{
    /// <summary>
    /// Provides Inverse class space density frequency
    /// </summary>
    /// <remarks>
    /// <para>
    /// Related paper: Ren, Fuji, and Mohammad Golam Sohrab. 2013. “Class-Indexing-Based Term Weighting for Automatic Text Classification.” Information Sciences 236. Elsevier Inc.: 109–25. doi:10.1016/j.ins.2013.02.029.
    /// </para>
    /// </remarks>
    /// <seealso cref="imbNLP.Toolkit.Weighting.Elements.ModelElementBase" />
    public class ICSdFElement : GlobalElementBase
    {
        public ICSdFElement()
        {
            shortName = "ICSdF";
            description = "Inverse class space density frequency, Related paper: Ren, Fuji, and Mohammad Golam Sohrab. 2013. “Class-Indexing-Based Term Weighting for Automatic Text Classification.” Information Sciences 236. Elsevier Inc.: 109–25. doi:10.1016/j.ins.2013.02.029.";
        }


        /// <summary>
        /// Term vs IDF factor
        /// </summary>
        /// <value>
        /// The index of the idf.
        /// </value>
        protected Dictionary<String, Double> ICSdF_index { get; set; } = new Dictionary<string, double>();

        protected Dictionary<String, Dictionary<SpaceLabel, Double>> TermClassDensity { get; set; } = new Dictionary<string, Dictionary<SpaceLabel, double>>();

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
            if (!ICSdF_index.ContainsKey(term)) return 0;
            return 1 + ICSdF_index[term];
        }

        public override void PrepareTheModel(SpaceModel space)
        {
            if (!IsEnabled) return;

            var labels = space.labels;

            //    Dictionary<String, List<SpaceLabel>> TermToLabelIndex = new Dictionary<string, List<SpaceLabel>>();

            var terms = space.terms.GetTokens();

            foreach (String term in terms)
            {
                Dictionary<SpaceLabel, Double> ClassDensity = new Dictionary<SpaceLabel, double>();
                foreach (SpaceLabel label in labels)
                {
                    ClassDensity.Add(label, 0);
                }

                TermClassDensity.Add(term, ClassDensity);
                ICSdF_index.Add(term, 0);
            }



            foreach (SpaceLabel label in labels)
            {
                List<SpaceDocumentModel> documents = space.LabelToDocumentLinks.GetAllLinked(label);

                Int32 doc_N = documents.Count;
                foreach (String term in terms)
                {
                    Int32 doc_t = documents.Count(x => x.terms.Contains(term));
                    if (doc_t > 0)
                    {
                        Double f = Convert.ToDouble(doc_t) / Convert.ToDouble(doc_N);
                        if (f > 0)
                        {
                            TermClassDensity[term][label] = f;
                        }
                    }
                }
            }

            Double C = labels.Count;

            foreach (String term in terms)
            {
                Double CS = 0;
                foreach (SpaceLabel label in labels)
                {
                    if (TermClassDensity[term][label] > 0)
                    {
                        CS = CS + TermClassDensity[term][label];
                    }
                }
                if (CS > 0)
                {
                    ICSdF_index[term] = Math.Log(C / CS);
                }

            }
        }
    }
}