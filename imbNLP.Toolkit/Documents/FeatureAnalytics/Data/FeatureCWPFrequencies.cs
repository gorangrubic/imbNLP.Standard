using imbNLP.Toolkit.Documents.FeatureAnalytics.Core;
using imbNLP.Toolkit.Entity;
using imbSCI.Core.attributes;
using imbSCI.Core.math.measurement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics.Data
{
    /// <summary>
    /// Base class for feature metrics
    /// </summary>
    /// <seealso cref="imbSCI.Core.math.measurement.MetricsBase" />
    public abstract class FeatureCWPInfoItemBase : MetricsBase
    {
        protected FeatureCWPInfoItemBase() : base()
        {
        }

        [imb(imbAttributeName.reporting_columnWidth, 15)]
        [Category("Term")]
        [DisplayName("Feature ID")]
        public String term { get; set; } = "";

        [imb(imbAttributeName.reporting_columnWidth, 15)]
        [Category("Term")]
        [DisplayName("Designation")]
        public FeatureCWPTermClass featureClass { get; set; } = FeatureCWPTermClass.unevaluated;
    }

    /// <summary>
    /// Absolute frequences per scope
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Documents.FeatureAnalytics.Data.FeatureCWPInfoItemBase" />
    public class FeatureCWPFrequencies : FeatureCWPInfoItemBase
    {
        [imb(imbAttributeName.reporting_hide)]
        [XmlIgnore]
        public Dictionary<DocumentBlenderFunctionOptions, Double> TermFrequencyByScope { get; set; } = new Dictionary<DocumentBlenderFunctionOptions, Double>();

        // frequencies
        [Category("Raw frequency")]
        [DisplayName("Block")]
        [imb(imbAttributeName.reporting_columnWidth, 10)]
        [imb(imbAttributeName.reporting_valueformat, "F1")]
        public Double block_frequency { get; set; }

        [Category("Raw frequency")]
        [DisplayName("Layer")]
        [imb(imbAttributeName.reporting_columnWidth, 10)]
        [imb(imbAttributeName.reporting_valueformat, "F1")]
        public Double layer_frequency { get; set; }

        [Category("Raw frequency")]
        [DisplayName("Page")]
        [imb(imbAttributeName.reporting_columnWidth, 10)]
        [imb(imbAttributeName.reporting_valueformat, "F1")]
        public Double page_frequency { get; set; }

        [Category("Raw frequency")]
        [DisplayName("Site")]
        [imb(imbAttributeName.reporting_columnWidth, 10)]
        [imb(imbAttributeName.reporting_valueformat, "F1")]
        public Double site_frequency { get; set; }

        [Category("Raw frequency")]
        [DisplayName("Class")]
        [imb(imbAttributeName.reporting_columnWidth, 10)]
        [imb(imbAttributeName.reporting_valueformat, "F1")]
        public Double class_frequency { get; set; }

        [Category("Raw frequency")]
        [DisplayName("Total")]
        [imb(imbAttributeName.reporting_columnWidth, 10)]
        [imb(imbAttributeName.reporting_valueformat, "F1")]
        public Double total_frequency { get; set; }

        public void DirectCount(DocumentBlenderFunctionOptions scope)
        {
            switch (scope)
            {
                case DocumentBlenderFunctionOptions.none:
                    break;
                case DocumentBlenderFunctionOptions.binaryAggregation:
                    break;
                case DocumentBlenderFunctionOptions.weightedAggregation:
                    break;
                case DocumentBlenderFunctionOptions.pageLevel:
                    page_frequency++;
                    break;
                case DocumentBlenderFunctionOptions.blockLevel:
                    block_frequency++;
                    break;
                case DocumentBlenderFunctionOptions.sentenceLevel:

                    break;
                case DocumentBlenderFunctionOptions.siteLevel:
                    site_frequency++;
                    break;
                case DocumentBlenderFunctionOptions.uniqueContentUnitsOnly:
                    break;
                case DocumentBlenderFunctionOptions.separatePages:
                    break;
                case DocumentBlenderFunctionOptions.keepLayersInMemory:
                    break;
                case DocumentBlenderFunctionOptions.categoryLevel:
                    class_frequency++;
                    break;
                case DocumentBlenderFunctionOptions.datasetLevel:
                    total_frequency++;
                    break;
                case DocumentBlenderFunctionOptions.layerLevel:
                    layer_frequency++;
                    break;
                default:
                    break;
            }
        }

        public void Add(DocumentBlenderFunctionOptions scope, Double __score, Boolean sumValue = true)
        {
            if (!TermFrequencyByScope.ContainsKey(scope)) TermFrequencyByScope.Add(scope, 0);
            if (sumValue)
            {
                TermFrequencyByScope[scope] += __score;
            }
            else
            {
                TermFrequencyByScope[scope] = __score;
            }
        }

        public void Add(DocumentBlenderFunctionOptions scope, Boolean contained)
        {
            if (!TermFrequencyByScope.ContainsKey(scope)) TermFrequencyByScope.Add(scope, 0);
            if (contained) TermFrequencyByScope[scope]++;
        }

        public void Compute()
        {
            if (TermFrequencyByScope.ContainsKey(DocumentBlenderFunctionOptions.layerLevel)) layer_frequency = TermFrequencyByScope[DocumentBlenderFunctionOptions.layerLevel];
            if (TermFrequencyByScope.ContainsKey(DocumentBlenderFunctionOptions.blockLevel)) block_frequency = TermFrequencyByScope[DocumentBlenderFunctionOptions.blockLevel];
            if (TermFrequencyByScope.ContainsKey(DocumentBlenderFunctionOptions.pageLevel)) page_frequency = TermFrequencyByScope[DocumentBlenderFunctionOptions.pageLevel];
            if (TermFrequencyByScope.ContainsKey(DocumentBlenderFunctionOptions.siteLevel)) site_frequency = TermFrequencyByScope[DocumentBlenderFunctionOptions.siteLevel];
            if (TermFrequencyByScope.ContainsKey(DocumentBlenderFunctionOptions.categoryLevel)) class_frequency = TermFrequencyByScope[DocumentBlenderFunctionOptions.categoryLevel];
            if (TermFrequencyByScope.ContainsKey(DocumentBlenderFunctionOptions.datasetLevel)) total_frequency = TermFrequencyByScope[DocumentBlenderFunctionOptions.datasetLevel];
        }
    }
}