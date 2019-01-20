using imbSCI.Core.attributes;
using imbSCI.Core.math;
using imbSCI.Core.math.range.finder;
using System;
using System.ComponentModel;
using System.Text;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics.Data
{
    public class FeatureCWPMetrics : FeatureCWPInfoItemBase
    {
        public FeatureCWPMetrics()
        {
        }

        /// <summary> Ratio </summary>
        [Category("Ratio")]
        [DisplayName("MaxParticularity")]
        [Description("Ratio")]
        [imb(imbAttributeName.measure_letter, "P")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")] // sets 0.00% display format for the property
        public Double MaxParticularity { get; set; } = default(Double);

        [Category("Ratio")]
        [DisplayName("MinCommonality")]
        [Description("Ratio")]
        [imb(imbAttributeName.measure_letter, "P")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")] // sets 0.00% display format for the property
        public Double MinCommonality { get; set; } = default(Double);

        /// <summary> Ratio </summary>
        [Category("Ratio")]
        [DisplayName("Score_MeanStdDev")]
        [Description("Cross category mean of StdDeviation")]
        [imb(imbAttributeName.measure_letter, "P")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")] // sets 0.00% display format for the property
        public Double Score_MeanStdDev { get; set; } = default(Double);

        [imb(imbAttributeName.measure_letter, "P")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        public Double Score_Mean { get; set; } = default(Double);

        [imb(imbAttributeName.measure_letter, "P")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        public Double MacroParticularity { get; set; } = default(Double);

        // public Double MacroCommonality { get; set; } = default(Double);

        public void Deploy(rangeFinderCollectionForMetrics<FeatureCWPAnalysisSiteMetrics> __finder)
        {
            finder = __finder;

            var dict = finder.GetDictionary();

            Double max_score = (Double)dict[nameof(FeatureCWPAnalysisSiteMetrics.mean_score) + "_" + nameof(rangeFinder.Maximum)];

            Double score = (Double)dict[nameof(FeatureCWPAnalysisSiteMetrics.mean_score) + "_" + nameof(rangeFinder.Sum)];

            MacroParticularity = max_score.GetRatio(score);

            MaxParticularity = (Double)dict[nameof(FeatureCWPAnalysisSiteMetrics.particularity_score) + "_" + nameof(rangeFinder.Maximum)];

            MinCommonality = (Double)dict[nameof(FeatureCWPAnalysisSiteMetrics.commonality_score) + "_" + nameof(rangeFinder.Maximum)];

            Score_MeanStdDev = (Double)dict[nameof(FeatureCWPAnalysisSiteMetrics.varianceCoeficient) + "_" + nameof(rangeFinder.Average)];

            Score_Mean = (Double)dict[nameof(FeatureCWPAnalysisSiteMetrics.mean_score) + "_" + nameof(rangeFinder.Average)];
        }

        private rangeFinderCollectionForMetrics<FeatureCWPAnalysisSiteMetrics> finder { get; set; }

        public String ToString(StringBuilder sb)
        {
            var dict = finder.GetDictionary();

            sb.Append(term + "\t \t \t");

            foreach (var p in dict)
            {
                sb.Append(" \t " + p.Key + " = " + p.Value.ToString() + "\t \t");
            }

            return sb.ToString();
        }
    }
}