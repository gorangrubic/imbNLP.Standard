using imbNLP.Toolkit.Documents.HtmlAnalysis;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Reporting;
using imbNLP.Toolkit.Space;
using imbSCI.Core.extensions.table;
using imbSCI.Core.files.folders;
using imbSCI.DataComplex.tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

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

        public Dictionary<String, List<String>> StemToTokens { get; set; } = new Dictionary<string, List<string>>();
        public List<String> DictinctTokens { get; set; } = new List<string>();
        public List<String> DictinctStems { get; set; } = new List<string>();

        public List<String> labels = new List<string>();

        public List<String> domains = new List<string>();

        public List<String> pages = new List<string>();

        public Dictionary<String, TokenDictionary> categoryNameVsTerms = new Dictionary<string, TokenDictionary>();
        public Dictionary<String, List<SpaceDocumentModel>> categoryNameVsDocumentModel = new Dictionary<string, List<SpaceDocumentModel>>();
        public Dictionary<String, List<TextDocument>> categoryNameVsDocumentText = new Dictionary<String, List<TextDocument>>();

        public Dictionary<String, HtmlTagCategoryTree> categoryNameVsHtmlTag = new Dictionary<string, HtmlTagCategoryTree>();

        public TokenDictionary terms { get; set; } = new TokenDictionary();

        public void MakeGraphics()
        {
            //erms.GetRankedTokenFrequency().
        }

        public void ReportHTMLTags(folderNode folder, String name)
        {
            foreach (var pair in categoryNameVsHtmlTag)
            {
                pair.Value.GetDataTable(name + "_" + pair.Key + "_htmlTag", pair.Value.description).GetReportAndSave(folder, new imbSCI.Core.data.aceAuthorNotation());
                //       dataSetASummary.Merge(pair.Value);
            }

            GlobalCategoryTree.GetDataTable(name + "_htmlTag_all", "Aggregate HTML tags statistics").GetReportAndSave(folder, new imbSCI.Core.data.aceAuthorNotation());
        }

        public void ReportSample(folderNode folder, String name, Int32 limit)
        {
            //String tknA_p = folder.pathFor("tokens_" + name + ".txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of all unique terms in the dataset [" + name + "]");

            //File.WriteAllText(tknA_p, terms.GetTokens().toCsvInLine(","));
            List<String> allDocumentIDS = new List<string>();

            foreach (KeyValuePair<string, List<TextDocument>> pair in categoryNameVsDocumentText)
            {
                String tkn_c = folder.pathFor("documents_" + name + "_" + pair.Key + ".txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of documents in category [" + pair.Key + "]");

                var names = pair.Value.Select(x => x.name).ToList();

                File.WriteAllLines(tkn_c, names);
                allDocumentIDS.AddRange(names);
            }

            String dp = folder.pathFor("documents_" + name + "_All" + ".txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of documents in dataset");

            File.WriteAllLines(dp, allDocumentIDS);

            dp = folder.pathFor("webpages_" + name + ".txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of documents in dataset");

            File.WriteAllLines(dp, pages);

            dp = folder.pathFor("webdomains_" + name + ".txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of domains in dataset");

            File.WriteAllLines(dp, domains);
        }

        public void ReportTokens(folderNode folder, String name, Int32 limit)
        {
            foreach (KeyValuePair<string, TokenDictionary> pair in categoryNameVsTerms)
            {
                String tkn_c = folder.pathFor("stems_" + name + "_" + pair.Key + "_top" + limit + ".txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of all unique terms in the dataset [" + name + "]");

                pair.Value.MakeRankedList("stems_" + name, "Ranked list of features in [" + pair.Key + "] for [" + name + "]", limit, tkn_c);

                var rt = pair.Value.MakeTable("stems_" + name + "_" + pair.Key, "Category [" + pair.Key + "] of [" + name + "] data set", limit);

                rt.GetReportAndSave(folder, new imbSCI.Core.data.aceAuthorNotation());
            }

            var allTable = terms.MakeTable("stems_" + name + "_All", "stems of [" + name + "] data set", limit);

            allTable.GetReportAndSave(folder, new imbSCI.Core.data.aceAuthorNotation());

            String tkn_all = folder.pathFor("stems_" + name + "_All" + limit + ".txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "Ranked list of all unique terms in the dataset [" + name + "]");

            terms.MakeRankedList("stems_" + name + "_All", "Ranked list of features in for [" + name + "]", limit, tkn_all);
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

            output.SetAdditionalInfoEntry("Stems", DictinctStems.Count(), "Number of distinct stems");
            output.SetAdditionalInfoEntry("Tokens", DictinctTokens.Count(), "Number of distinct tokens");

            return output;
        }
    }
}