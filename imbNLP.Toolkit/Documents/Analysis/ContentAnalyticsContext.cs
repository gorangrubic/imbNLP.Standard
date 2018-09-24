using imbNLP.Toolkit.Documents.HtmlAnalysis;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbSCI.Core.extensions.table;
using imbSCI.DataComplex.tables;
using System;
using System.Collections.Generic;
using System.Data;

namespace imbNLP.Toolkit.Documents.Analysis
{
    public class ContentAnalyticsContext
    {
        public ContentAnalyticsContext()
        {

        }


        public HtmlTagCategoryTree GlobalCategoryTree { get; set; }



        public List<ContentMetrics> ClassMetrics { get; set; } = new List<ContentMetrics>();

        public ContentMetrics sumMetrics { get; set; } = new ContentMetrics();

        public ContentMetrics avgMetrics { get; set; } = new ContentMetrics();

        public List<String> labels = new List<string>();

        public List<String> domains = new List<string>();

        public Dictionary<String, TokenDictionary> categoryNameVsTerms = new Dictionary<string, TokenDictionary>();
        public Dictionary<String, List<SpaceDocumentModel>> categoryNameVsDocumentModel = new Dictionary<string, List<SpaceDocumentModel>>();
        public Dictionary<String, List<TextDocument>> categoryNameVsDocumentText = new Dictionary<String, List<TextDocument>>();

        public Dictionary<String, HtmlTagCategoryTree> categoryNameVsHtmlTag = new Dictionary<string, HtmlTagCategoryTree>();


        public TokenDictionary terms { get; set; } = new TokenDictionary();

        public void MakeGraphics()
        {
            //erms.GetRankedTokenFrequency().
        }


        public DataTable GetDataTable(String name)
        {
            DataTableTypeExtended<ContentMetrics> output = new DataTableTypeExtended<ContentMetrics>(name);
            foreach (var c in ClassMetrics)
            {
                output.AddRow(c);
            }
            output.AddRow(sumMetrics);
            output.AddRow(avgMetrics);

            output.SetAdditionalInfoEntry("Terms", terms.Count, "Number of distinct terms in the dataset");
            output.SetAdditionalInfoEntry("MaxTF", terms.GetMaxFrequency(), "Max. frequency in the corpus");
            output.SetAdditionalInfoEntry("SumTF", terms.GetSumFrequency(), "Sum of frequencies in the corpus");

            return output;
        }
    }

}