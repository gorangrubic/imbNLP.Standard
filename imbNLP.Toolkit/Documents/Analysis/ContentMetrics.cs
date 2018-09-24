using imbSCI.Core.attributes;
using System;
using System.ComponentModel;

namespace imbNLP.Toolkit.Documents.Analysis
{

    public class ContentMetrics : MetricsBase
    {

        public ContentMetrics()
        {

        }

        public ContentMetrics(String _name)
        {
            Name = _name;
        }

        /// <summary> name of the entry </summary>
        [Category("Label")]
        [DisplayName("Name")]
        [Description("name of the entry")]
        [imb(imbAttributeName.reporting_escapeoff)] // allows URLs, when reported as HTML
        public String Name { get; set; } = default(String);



        /// <summary> Number of classes in the dataset </summary>
        [Category("Count")]
        [DisplayName("Class")]
        [Description("Number of classes in the dataset")]
        [imb(imbAttributeName.measure_letter, "C")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_valueformat, "#,###")]
        // [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        // [imb(imbAttributeName.basicColor, ColorWorks.ColorRed)]
        public Int32 Class { get; set; } = default(Int32);


        /// <summary> Number of document sets in the dataset </summary>
        [Category("Count")]
        [DisplayName("DocumentSets")]
        [Description("Number of document sets in the dataset")]
        [imb(imbAttributeName.measure_letter, "C")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_valueformat, "#,###")]
        // [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        // [imb(imbAttributeName.basicColor, ColorWorks.ColorRed)]
        public Int32 DocumentSets { get; set; } = default(Int32);


        /// <summary> Number of documents in the data set </summary>
        [Category("Count")]
        [DisplayName("Documents")]
        [Description("Number of documents in the data set")]
        [imb(imbAttributeName.measure_letter, "C")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_valueformat, "#,###")]
        // [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        // [imb(imbAttributeName.basicColor, ColorWorks.ColorRed)]
        public Int32 Documents { get; set; } = default(Int32);


        /// <summary> Number of tokens in the documents </summary>
        [Category("Count")]
        [DisplayName("TokensDoc")]
        [Description("Number of tokens in the documents - i.e. summed length of all documents")]
        [imb(imbAttributeName.measure_letter, "C")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_valueformat, "#,###")]
        // [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        // [imb(imbAttributeName.basicColor, ColorWorks.ColorRed)]
        public Int32 TokensDoc { get; set; } = default(Int32);


        /// <summary> Number of distinct tokens in the dataset, before stemming </summary>
        [Category("Count")]
        [DisplayName("UniqueTokensDoc")]
        [Description("Number of distinct tokens in the document, summed on document level, before stemming")]
        [imb(imbAttributeName.measure_letter, "C")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_valueformat, "#,###")]
        // [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        // [imb(imbAttributeName.basicColor, ColorWorks.ColorRed)]
        public Int32 UniqueTokensDoc { get; set; } = default(Int32);



        /// <summary> Number of stemmed terms, in the dataset </summary>
        [Category("Count")]
        [DisplayName("StemmedTokensDoc")]
        [Description("Number of unique stemmed terms, summed from document level")]
        [imb(imbAttributeName.measure_letter, "C")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_valueformat, "#,###")]
        // [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        // [imb(imbAttributeName.basicColor, ColorWorks.ColorRed)]
        public Int32 StemmedTokensDoc { get; set; } = default(Int32);




        /// <summary> Distinct terms in the class, summed on category level (after stemming) </summary>
        [Category("Count")]
        [DisplayName("Terms")]
        [Description("Distinct terms in the class, summed on category level (after stemming)")]
        [imb(imbAttributeName.measure_letter, "C")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_valueformat, "#,###")]
        // [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        // [imb(imbAttributeName.basicColor, ColorWorks.ColorRed)]
        public Int32 Terms { get; set; } = default(Int32);



        /// <summary> Total number of characters in source codes of the documents </summary>
        [Category("Count")]
        [DisplayName("SourceLength")]
        [Description("Total number of characters in source codes of the documents")]
        [imb(imbAttributeName.measure_letter, "C")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_valueformat, "#,###")]
        // [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        // [imb(imbAttributeName.basicColor, ColorWorks.ColorRed)]
        public Int32 SourceLength { get; set; } = default(Int32);



        /// <summary> Total number of characters in rendered textural representation </summary>
        [Category("Count")]
        [DisplayName("RenderLength")]
        [Description("Total number of characters in rendered textural representation")]
        [imb(imbAttributeName.measure_letter, "C")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_valueformat, "#,###")]
        // [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        // [imb(imbAttributeName.basicColor, ColorWorks.ColorRed)]
        public Int32 RenderLength { get; set; } = default(Int32);




    }
}
