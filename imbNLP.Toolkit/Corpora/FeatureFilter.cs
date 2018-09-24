using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting.Global;
using imbSCI.Core.enums;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Corpora
{




    /// <summary>
    /// Feature filter
    /// </summary>
    public class FeatureFilter : IDescribe
    {
        public void Describe(ILogBuilder logger)
        {
            logger.AppendLine("Feature Selection");
            logger.AppendPair("Function", function.shortName, true, "\t\t\t");
            logger.AppendPair("Limit", limit, true, "\t\t\t");

            logger.AppendLine("Ranking method for n-dimensional Feature Weights: " + nVectorValueSelectionOperation.ToString());
        }

        public void Deploy(ILogBuilder logger)
        {
            function = functionSettings.GetFunction(logger);


        }

        /// <summary>
        /// Selects the top <see cref="limit"/> terms, ranked by <see cref="function"/>
        /// </summary>
        /// <param name="space">The space.</param>
        /// <returns></returns>
        public List<KeyValuePair<string, double>> SelectFeatures(SpaceModel space)
        {
            Dictionary<String, Double> rank = new Dictionary<string, double>();

            if (limit == -1)
            {
                List<KeyValuePair<string, double>> outAll = new List<KeyValuePair<string, double>>();
                var tokens = space.terms.GetTokens();
                foreach (String tkn in tokens)
                {
                    outAll.Add(new KeyValuePair<string, double>(tkn, 1));
                }
                return outAll;
            }

            function.PrepareTheModel(space);

            WeightDictionary featureScores = function.GetElementFactors(space.terms.GetTokens(), space);



            if (featureScores.nDimensions > 1)
            {
                foreach (WeightDictionaryEntry en in featureScores.entries)
                {
                    rank.Add(en.name, en.CompressNumericVector(nVectorValueSelectionOperation));
                }
            }
            else
            {
                foreach (WeightDictionaryEntry en in featureScores.entries)
                {
                    rank.Add(en.name, en.weight);
                }
            }

            var rankSorted = rank.OrderByDescending(x => x.Value).ToList();
            List<KeyValuePair<string, double>> top = rankSorted.Take(Math.Min(limit, rankSorted.Count)).ToList();
            return top;
        }



        public FeatureFilter()
        {

        }

        public String GetSignature()
        {
            String output = functionSettings.GetSignature();
            if (limit > 0) output += limit.ToString();
            return output;
        }

        [XmlIgnore]
        public IGlobalElement function { get; set; }

        public Int32 limit { get; set; } = -1;

        public operation nVectorValueSelectionOperation { get; set; } = operation.max;

        public GlobalFunctionSettings functionSettings { get; set; } = new GlobalFunctionSettings();
    }


}