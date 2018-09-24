using imbNLP.Toolkit.Documents.Analysis;
using imbSCI.Core.attributes;
using System;
using System.ComponentModel;

namespace imbNLP.Toolkit.Documents.HtmlAnalysis
{



    /// <summary>
    /// One entry in tag counter table
    /// </summary>
    public class HtmlTagCounterResult : MetricsBase
    {
        public HtmlTagCounterResult() : base()
        {

        }


        /// <summary> Path to the category </summary>
        [Category("Label")]
        [DisplayName("Path")]
        [Description("Path to the category")]
        [imb(imbAttributeName.reporting_columnWidth, 50)]
        [imb(imbAttributeName.reporting_escapeoff)] // allows URLs, when reported as HTML
        public String Path { get; set; } = default(String);



        /// <summary> Category name </summary>
        [Category("Label")]
        [DisplayName("Name")]
        [Description("Category name")]
        [imb(imbAttributeName.reporting_escapeoff)] // allows URLs, when reported as HTML
        public String Name { get; set; } = default(String);


        /// <summary> Description </summary>
        [Category("Label")]
        [DisplayName("Description")]
        [Description("Description on the category")]
        [imb(imbAttributeName.reporting_escapeoff)] // allows URLs, when reported as HTML
        public String Description { get; set; } = default(String);



        /// <summary> Absolute score of the tag category </summary>
        [Category("Count")]
        [DisplayName("Score")]
        [Description("Absolute score of the tag category")]
        [imb(imbAttributeName.measure_letter, "C")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_valueformat, "#,###")]
        [imb(imbAttributeName.reporting_columnWidth, 20)]
        // [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        // [imb(imbAttributeName.basicColor, ColorWorks.ColorRed)]
        public Int32 Score { get; set; } = default(Int32);



        /// <summary> Ratio </summary>
        [Category("Ratio")]
        [DisplayName("WeightedScore")]
        [Description("Ratio")]
        [imb(imbAttributeName.measure_letter, "P")]
        [imb(imbAttributeName.measure_setUnit, "w")]
        [imb(imbAttributeName.reporting_columnWidth, 20)]
        [imb(imbAttributeName.reporting_valueformat, "F4")] // sets 0.00% display format for the property
        public Double WeightedScore { get; set; } = default(Double);



        /// <summary> Tags </summary>
        [Category("Label")]
        [DisplayName("Tags")]
        [Description("Tags")]
        [imb(imbAttributeName.reporting_escapeoff)] // allows URLs, when reported as HTML
        public String Tags { get; set; } = default(String);



    }

}