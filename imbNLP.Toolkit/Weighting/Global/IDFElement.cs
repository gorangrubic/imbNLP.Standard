using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting.Data;
using imbSCI.Core.extensions.enumworks;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

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
        // public Dictionary<String, Double> index { get; set; } = new Dictionary<string, double>();

        public override void LoadModelData(WeightingModelData data)
        {
            LoadModelDataBase(data);
        }

        public override WeightingModelData SaveModelData()
        {
            return SaveModelDataBase();
        }

        protected Int32 DocumentN { get; set; } = 0;

        public override double GetElementFactor(string term, SpaceModel space, SpaceLabel label = null)
        {
            if (!IsEnabled) return 1;
            if (!index.ContainsKey(term)) return 0;
            Double score = index[term];
            if (!DistinctReturns.ContainsKey(score))
            {
                DistinctReturns.Add(score, term);
            }


            return score;

        }

        public override void PrepareTheModel(SpaceModel space, ILogBuilder log)
        {
            if (!IsEnabled) return;

            index.Clear();

            if (Computation == IDFComputation.DF)
            {
                shortName = "DF";
            }

            Dictionary<String, List<SpaceDocumentModel>> TermToDocumentIndex = new Dictionary<string, List<SpaceDocumentModel>>();

            List<SpaceLabel> labels = space.labels.ToList();

            var terms = space.GetTokens(true, true);

            foreach (String term in terms)
            {
                TermToDocumentIndex.Add(term, new List<SpaceDocumentModel>());
            }

            Double N = 0;

            foreach (SpaceLabel label in labels)
            {
                foreach (SpaceDocumentModel document in space.LabelToDocumentLinks.GetAllLinked(label))
                {
                    var termsInDocument = document.GetTokens(terms); //.GetTerms(true, true, true).GetTokens();

                    for (int i = 0; i < termsInDocument.Count; i++)
                    {
                        if (TermToDocumentIndex.ContainsKey(termsInDocument[i]))
                        {
                            TermToDocumentIndex[termsInDocument[i]].Add(document);
                        }
                    }

                    DocumentN++;
                }

            }

            N = DocumentN;

            foreach (String term in terms)
            {
                Double DF_t = TermToDocumentIndex[term].Count;
                Double IDF_t = 0;

                if (DF_t != 0)
                {
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
                }

                index.Add(term, IDF_t);
            }
        }

        public override void DeploySettings(GlobalFunctionSettings settings)
        {
            Computation = imbEnumExtendBase.GetEnumFromStringFlags<IDFComputation>(settings.flags, Computation).FirstOrDefault();
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