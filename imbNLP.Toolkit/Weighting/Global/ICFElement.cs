using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting.Data;
using imbSCI.Core.reporting;
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
        //public Dictionary<String, Double> index { get; set; } = new Dictionary<string, double>();

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

            var labels = space.labels;

            Dictionary<String, List<SpaceLabel>> TermToLabelIndex = new Dictionary<string, List<SpaceLabel>>();

            var terms = space.GetTokens(true, false);

            foreach (String term in terms)
            {
                TermToLabelIndex.Add(term, new List<SpaceLabel>());
            }

            foreach (SpaceLabel label in labels)
            {
                List<SpaceDocumentModel> documents = space.GetDocumentsOfLabel(label.name); //.//LabelToDocumentLinks.GetAllLinked(label);
                foreach (SpaceDocumentModel document in documents)
                {
                    var termsInDocument = document.GetTerms(true, true).GetTokens();
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
                if (TermToLabelIndex.ContainsKey(term))
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
                    index.Add(term, ICF_t);
                }
                else
                {
                    index.Add(term, 0);
                }
            }
        }

        public override void LoadModelData(WeightingModelData data)
        {
            LoadModelDataBase(data);
        }

        public override WeightingModelData SaveModelData()
        {
            return SaveModelDataBase();
        }

        public override void DeploySettings(GlobalFunctionSettings settings)
        {

        }
    }
}