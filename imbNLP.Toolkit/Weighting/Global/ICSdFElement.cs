using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting.Data;
using imbSCI.Core.reporting;
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
        //protected Dictionary<String, Double> index { get; set; } = new Dictionary<string, double>();

        //   protected 

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

            Double score = 1 + index[term];
            if (!DistinctReturns.ContainsKey(score))
            {
                DistinctReturns.Add(score, term);
            }


            return score;

        }

        public override void LoadModelData(WeightingModelData data)
        {
            LoadModelDataBase(data);
        }

        public override WeightingModelData SaveModelData()
        {
            return SaveModelDataBase();
        }

        public override void PrepareTheModel(SpaceModel space, ILogBuilder log)
        {
            if (!IsEnabled) return;

            index.Clear();

            var labels = space.labels;

            Dictionary<String, Dictionary<SpaceLabel, Double>> TermClassDensity = new Dictionary<string, Dictionary<SpaceLabel, double>>();

            //    Dictionary<String, List<SpaceLabel>> TermToLabelIndex = new Dictionary<string, List<SpaceLabel>>();

            var terms = space.GetTokens(true, false);

            foreach (String term in terms)
            {
                Dictionary<SpaceLabel, Double> ClassDensity = new Dictionary<SpaceLabel, double>();
                foreach (SpaceLabel label in labels)
                {
                    ClassDensity.Add(label, 0);
                }

                TermClassDensity.Add(term, ClassDensity);
                index.Add(term, 0);
            }



            foreach (SpaceLabel label in labels)
            {
                List<SpaceDocumentModel> documents = space.GetDocumentsOfLabel(label.name); // .LabelToDocumentLinks.GetAllLinked(label);

                Int32 doc_N = documents.Count;
                foreach (String term in terms)
                {
                    Int32 doc_t = documents.Count(x => x.Contains(term));
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
                    if (TermClassDensity.ContainsKey(term))
                    {
                        if (TermClassDensity[term][label] > 0)
                        {
                            CS = CS + TermClassDensity[term][label];
                        }
                    }
                }
                if (CS > 0)
                {
                    if (index.ContainsKey(term))
                    {
                        index[term] = Math.Log(C / CS);
                    }
                }

            }

            //foreach (KeyValuePair<string, double> pair in index)
            //{
            //    if (index.ContainsKey(pair.Key)) index[pair.Key] = 
            //}
        }

        public override void DeploySettings(GlobalFunctionSettings settings)
        {

        }
    }
}