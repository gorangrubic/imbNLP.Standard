using imbSCI.Core.attributes;
using imbSCI.Core.math;
using imbSCI.Core.math.measurement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics.Data
{


    public class FeatureCWPAnalysisSiteMetrics : FeatureCWPInfoItemBase
    {

        public FeatureCWPAnalysisSiteMetrics() : base()
        {

        }




        [imb(imbAttributeName.reporting_hide)]
        [XmlIgnore]
        public Dictionary<string, Double> TermFrequencyRatios { get; set; } = new Dictionary<string, double>();


        [Category("Counters")]
        [DisplayName("Count")]
        [Description("Number of times the term was detected at target and sub target scope")]
        [imb(imbAttributeName.reporting_columnWidth, 7)]
        [imb(imbAttributeName.measure_letter, "t")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        public Int32 Count { get; set; } = 0;

        [Category("Counters")]
        [DisplayName("Sources")]
        [Description("Number of master categories agregated")]
        [imb(imbAttributeName.measure_setUnit, "S_in")]
        [imb(imbAttributeName.measure_letter, "|C|")]
        [imb(imbAttributeName.reporting_columnWidth, 4)]
        public Int32 SourceCount { get; set; } = 0;



        [Category("Master factors")]
        [DisplayName("Particularity")]
        [imb(imbAttributeName.reporting_columnWidth, 15)]
        [imb(imbAttributeName.reporting_valueformat, "F5")]
        public Double particularity_score { get; set; }


        [Category("Master factors")]
        [DisplayName("Commonality")]
        [imb(imbAttributeName.reporting_columnWidth, 15)]
        [imb(imbAttributeName.reporting_valueformat, "F5")]
        public Double commonality_score { get; set; }


        [Category("Frequency density score")]
        [DisplayName("Max in S_in")]
        [Description("Highest WSd within S_in")]
        [imb(imbAttributeName.measure_letter, "max(WSd)")]
        [imb(imbAttributeName.reporting_columnWidth, 15)]
        [imb(imbAttributeName.reporting_valueformat, "F5")]
        public Double max_score { get; set; } = 0;

        [Category("Frequency density score")]
        [DisplayName("Min in S_in")]
        [imb(imbAttributeName.measure_letter, "min(WSd)")]
        [imb(imbAttributeName.reporting_columnWidth, 15)]
        [imb(imbAttributeName.reporting_valueformat, "F5")]
        public Double min_score { get; set; } = 1;

        [Category("Frequency density score")]
        [DisplayName("Mean frequency density")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.measure_letter, "WSd / S_in")]
        [imb(imbAttributeName.reporting_columnWidth, 15)]
        [imb(imbAttributeName.reporting_valueformat, "F5")]
        public Double mean_score { get; set; }

        [Category("Frequency density score")]
        [DisplayName("Std Deviation")]
        [imb(imbAttributeName.measure_letter, "Std(WSd in S_in)")]
        [imb(imbAttributeName.reporting_columnWidth, 20)]
        [imb(imbAttributeName.reporting_valueformat, "F5")]
        public Double varianceCoeficient { get; set; }

        [imb(imbAttributeName.reporting_hide)]
        public List<String> max_score_entries { get; set; } = new List<string>();





        /// <summary> Ratio </summary>
        //[Category("Ratio")]
        //[DisplayName("myProperty")]
        //[Description("Ratio")]
        //[imb(imbAttributeName.measure_letter, "P")]
        //[imb(imbAttributeName.measure_setUnit, "%")]
        //[imb(imbAttributeName.reporting_valueformat, "P2")] // sets 0.00% display format for the property
        //  public Double myProperty { get; set; } = default(Double);



        // ---------------







        public void Compute(Int32 C)
        {
            max_score = TermFrequencyRatios.Values.Max();
            min_score = TermFrequencyRatios.Values.Min();

            score = TermFrequencyRatios.Values.Sum();

            particularity_score = max_score.GetRatio(score);

            commonality_score = max_score * min_score;

            mean_score = TermFrequencyRatios.Values.Average();

            varianceCoeficient = TermFrequencyRatios.Values.GetStdDeviation(false);

            foreach (var pair in TermFrequencyRatios)
            {
                if (pair.Value == max_score)
                {
                    max_score_entries.Add(pair.Key);
                }
            }

            Count = C;

        }

        ///// <summary>
        ///// Shows how much the term is particular for a / category or site -- for 1: the term is perfectly particular for one cateogry
        ///// </summary>
        ///// <returns></returns>
        //public Double GetParticularityScore()
        //{

        //    Double max_score = TermFrequencyRatios.Values.Max();
        //    return max_score.GetRatio(score);
        //}

        //public Double GetMeanScore()
        //{
        //    return TermFrequencyRatios.Values.Average();
        //}
        [Category("Frequency density score")]
        [DisplayName("Score")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.measure_letter, "WSd")]
        [imb(imbAttributeName.reporting_columnWidth, 15)]
        [imb(imbAttributeName.reporting_valueformat, "F5")]
        public Double score { get; set; } = 0;

        public void Add(String categoryOrSiteId, Double __score)
        {
            TermFrequencyRatios.Add(categoryOrSiteId, __score);

        }



        public FeatureCWPAnalysisSiteMetrics(String _term)
        {
            term = _term;

        }

    }
}