using imbNLP.Toolkit.Documents.FeatureAnalytics.Core;
using imbNLP.Toolkit.Entity;
using imbNLP.Toolkit.Space;
using imbSCI.Core.attributes;
using imbSCI.Core.math.measurement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics.Data
{
/// <summary>
    /// UNDERDEVELOPMENT
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Documents.FeatureAnalytics.Data.FeatureCWPInfoItemBase" />
    public class FeatureCWPDensityInfo : FeatureCWPInfoItemBase
    {

        public enum CWPScoreComputation
        {
            InSdF,
            InF,
            nSdF,
            nF,

        }


        [imb(imbAttributeName.reporting_hide)]
        [XmlIgnore]
        public Dictionary<DocumentBlenderFunctionOptions, Dictionary<string, Double>> TermFrequencyRatios { get; set; } = new Dictionary<DocumentBlenderFunctionOptions, Dictionary<string, double>>();


        public void Add(String model_id, DocumentBlenderFunctionOptions scope, double C, double n_ck, double N_ck)
        {
            if (!TermFrequencyRatios.ContainsKey(scope))
            {
                TermFrequencyRatios.Add(scope, new Dictionary<string, double>());
            }

            if (TermFrequencyRatios[scope].ContainsKey(model_id))
            {
                throw new ArgumentException("Model with ID [" + model_id + "] is already queried for Density information");
            }

            throw new NotImplementedException();
        }

    }
}