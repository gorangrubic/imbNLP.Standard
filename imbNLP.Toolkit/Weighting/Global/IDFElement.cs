using imbNLP.Toolkit.Space;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Weighting.Global
{
    /// <summary>
    /// Inverse Document Frequency and mIDF
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Weighting.Elements.ModelElementBase" />
    public class IDFElement : GlobalElementBase
    {
        public IDFElement()
        {
            shortName = "IDF";
        }

        /// <summary>
        /// Gets or sets the idf computation.
        /// </summary>
        /// <value>
        /// The idf computation.
        /// </value>
        public IDFComputation Computation { get; set; } = IDFComputation.logPlus;

        /// <summary>
        /// Term vs IDF factor
        /// </summary>
        /// <value>
        /// The index of the idf.
        /// </value>
        public Dictionary<String, Double> IDF_index { get; set; } = new Dictionary<string, double>();

        public override double GetElementFactor(string term, SpaceModel space, SpaceLabel label = null)
        {
            if (!IsEnabled) return 1;
            if (!IDF_index.ContainsKey(term)) return 0;
            return IDF_index[term];
        }

        public override void PrepareTheModel(SpaceModel space)
        {
            if (!IsEnabled) return;

            if (Computation == IDFComputation.DF)
            {
                shortName = "DF";
            }

            Dictionary<String, List<SpaceDocumentModel>> TermToDocumentIndex = new Dictionary<string, List<SpaceDocumentModel>>();
            var terms = space.terms.GetTokens();
            foreach (String term in terms)
            {
                TermToDocumentIndex.Add(term, new List<SpaceDocumentModel>());
            }

            foreach (SpaceDocumentModel document in space.documents)
            {
                var termsInDocument = document.terms.GetTokens();
                foreach (String termInDocument in termsInDocument)
                {
                    TermToDocumentIndex[termInDocument].Add(document);
                }
            }

            Double N = space.documents.Count;

            foreach (String term in terms)
            {
                Double DF_t = TermToDocumentIndex[term].Count;
                Double IDF_t = 0;

                switch (Computation)
                {
                    case IDFComputation.logPlus:
                        IDF_t = Math.Log(N / DF_t) + 1;
                        break;

                    case IDFComputation.modified:
                        IDF_t = Math.Log((N * N) - (N - DF_t) + N);
                        break;
                    case IDFComputation.DF:
                        IDF_t = DF_t / N;
                        break;
                }

                IDF_index.Add(term, IDF_t);
            }
        }

        /// <summary>
        /// Method of IDF computation
        /// </summary>
        public enum IDFComputation
        {
            /// <summary>
            /// The log idf plus one: log(N/DF_t) + 1
            /// </summary>
            logPlus,

            /// <summary>
            /// mIDF computation as proposed by mTF-mIDF research
            /// </summary>
            /// <remarks>
            /// Sabbah, Thabit, Ali Selamat, Md Hafiz Selamat, Fawaz S. Al-Anzi, Enrique Herrera Viedma, Ondrej Krejcar, and Hamido Fujita. 2017. “Modified Frequency-Based Term Weighting Schemes for Text Classification.” Applied Soft Computing Journal 58. Elsevier B.V.: 193–206. doi:10.1016/j.asoc.2017.04.069.
            /// </remarks>
            modified,

            /// <summary>
            /// Non-inversed Document Frequency score
            /// </summary>
            DF,
        }
    }
}