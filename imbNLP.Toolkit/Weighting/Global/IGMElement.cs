using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting.Data;
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
        public override void DeploySettings(GlobalFunctionSettings settings)
        {

        }


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
            if (!index.ContainsKey(term)) return 0;

            Double score = index[term];
            if (!DistinctReturns.ContainsKey(score))
            {
                DistinctReturns.Add(score, term);
            }


            return score;

        }

        public override void Describe(ILogBuilder logger)
        {
            base.Describe(logger);
            logger.AppendPair("Landa (λ)", l.ToString("F3"), true, "\t\t\t");
        }

        public override void LoadModelData(WeightingModelData data)
        {
            LoadModelDataBase(data);
        }

        public override WeightingModelData SaveModelData()
        {
            return SaveModelDataBase();
        }


        /// <summary>
        /// Prepares the model - computes IGM for each term
        /// </summary>
        /// <param name="space">The space.</param>
        /// <exception cref="ArgumentException">A document is already assigned to a label! This model is not applicable for multi-label problem.</exception>
        public override void PrepareTheModel(SpaceModel space, ILogBuilder log)
        {
            if (!IsEnabled) return;

            index.Clear();

            Dictionary<String, Dictionary<SpaceLabel, Int32>> TermClassFrequency = new Dictionary<string, Dictionary<SpaceLabel, int>>();
            Dictionary<String, List<KeyValuePair<SpaceLabel, Int32>>> TermClassRank = new Dictionary<String, List<KeyValuePair<SpaceLabel, Int32>>>();
            Dictionary<SpaceDocumentModel, SpaceLabel> DocumentVsLabel = new Dictionary<SpaceDocumentModel, SpaceLabel>();

            var labels = space.labels.ToList();

            var terms = space.GetTokens(true, false);

            foreach (String term in terms)
            {
                Dictionary<SpaceLabel, Int32> ClassFrequency = new Dictionary<SpaceLabel, Int32>();
                foreach (SpaceLabel label in labels)
                {
                    ClassFrequency.Add(label, 0);
                }

                TermClassFrequency.Add(term, ClassFrequency);
                index.Add(term, 0);
            }
            /*
            foreach (SpaceDocumentModel document in space.documents)
            {
                String lab = document.labels.First();
                var spaceLabel = space.labels.FirstOrDefault(x => x.name == lab);

                if (lab != SpaceLabel.UNKNOWN)
                {

                    if (DocumentVsLabel.ContainsKey(document))
                    {
                        throw new ArgumentException("A document [" + document.name + "] is already assigned to a label! This model is not applicable for multi-label problem.");
                    }

                    DocumentVsLabel.Add(document, spaceLabel);
                }
            }
            */

            foreach (SpaceLabel label in labels)
            {
                List<SpaceDocumentModel> documents = space.LabelToDocumentLinks.GetAllLinked(label);
                foreach (SpaceDocumentModel document in documents)
                {
                    if (DocumentVsLabel.ContainsKey(document))
                    {
                        throw new ArgumentException("A document [" + document.name + "] is already assigned to a label! This model is not applicable for multi-label problem.");
                    }
                    DocumentVsLabel.Add(document, label);
                }

                foreach (SpaceDocumentModel document in documents)
                {
                    var doc_terms_dict = document.GetTerms(true, true, true);
                    var doc_terms = doc_terms_dict.GetTokens();
                    foreach (String term in doc_terms)
                    {
                        TermClassFrequency[term][label] += doc_terms_dict.GetTokenFrequency(term);
                    }
                }
            }

            foreach (String term in terms)
            {
                TermClassRank.Add(term, TermClassFrequency[term].OrderByDescending(x => x.Value).ToList());

                Double igm_tk_below = 0;

                Double f_ki = TermClassRank[term].Max(x => x.Value);

                Double r = 1;

                var termRanks = TermClassRank[term];

                foreach (KeyValuePair<SpaceLabel, int> ranked in termRanks)
                {
                    if (ranked.Value > 0)
                    {
                        igm_tk_below += (Convert.ToDouble(ranked.Value) / f_ki) * r;
                    }
                    r++;
                }

                Double t = 0;

                if (igm_tk_below == 0)
                {
                    //index[term] = 0;
                }
                else
                {
                    t = 1 / igm_tk_below;
                }


                index[term] = 1 + (l * t);

            }



        }


    }
}