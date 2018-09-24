using imbNLP.Toolkit.Processing;
using imbSCI.Core.collection;
using imbSCI.Core.extensions.data;
using imbSCI.Core.files.folders;
using imbSCI.DataComplex.tables;
using System;
using System.Collections.Generic;
using System.IO;

namespace imbNLP.Toolkit.Documents.Analysis
{

    public class DataSetComparison
    {
        public List<String> DocumentSetsUniqueForA { get; set; } = new List<string>();
        public List<String> DocumentSetsInCommonByName { get; set; } = new List<string>();
        public List<String> DocumentSetsUniqueForB { get; set; } = new List<string>();

        public TokenDictionary TermsUniqueForA { get; set; } = new TokenDictionary();
        public TokenDictionary TermsUniqueForB { get; set; } = new TokenDictionary();
        public TokenDictionary TermsInCommon { get; set; } = new TokenDictionary();

        public List<string> tknA;
        public List<string> tknB;

        public List<string> tknC;

        public List<String> tknA_u;
        public List<String> tknB_u;

        public ContentAnalyticsContext analyticA;

        public ContentAnalyticsContext analyticB;

        public void FullReport(folderNode folder, String datasetA_name = "A", String datasetB_name = "B", String runName = "DataSets")
        {
            folder = folder.Add(runName, runName, "Reports on datasets [" + datasetA_name + "," + datasetB_name + "]");

            #region --------------- TERM LISTS ------------------------------
            String tknA_p = folder.pathFor("tokens_" + datasetA_name + ".txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of all unique terms in the dataset [" + datasetA_name + "]");
            String tknB_p = folder.pathFor("tokens_" + datasetB_name + ".txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of all unique terms in the dataset [" + datasetB_name + "]");
            String tknC_p = folder.pathFor("tokens_common.txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of all unique terms, existing in both datasets");

            String tknA_u_p = folder.pathFor("tokens_" + datasetA_name + "_specific.txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of all unique terms, being spcific to the dataset [" + datasetA_name + "], and not part of the other dataset");
            String tknB_u_p = folder.pathFor("tokens_" + datasetB_name + "_specific.txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of all unique terms, being spcific to the dataset [" + datasetB_name + "], and not part of the other dataset");

            File.WriteAllText(tknA_p, tknA.toCsvInLine(","));
            File.WriteAllText(tknB_p, tknB.toCsvInLine(","));
            File.WriteAllText(tknC_p, tknC.toCsvInLine(","));

            File.WriteAllText(tknA_u_p, tknA_u.toCsvInLine(","));
            File.WriteAllText(tknB_u_p, tknB_u.toCsvInLine(","));
            #endregion

            #region ------------------ domain list
            String docSetUA_p = folder.pathFor("domains_" + datasetA_name + "_specific.txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of all domains, being unique for the dataset [" + datasetA_name + "]");
            String docSetUB_p = folder.pathFor("domains_" + datasetB_name + "_specific.txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of all domains, being unique for the dataset [" + datasetB_name + "]");
            String docSetUC_p = folder.pathFor("domains_common.txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of all domains, being common to both datasets");
            String docSetA_p = folder.pathFor("domains_" + datasetA_name + ".txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of all domains in the dataset");
            String docSetB_p = folder.pathFor("domains_" + datasetB_name + ".txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of all domains in the dataset");

            File.WriteAllLines(docSetUA_p, DocumentSetsUniqueForA);
            File.WriteAllLines(docSetUB_p, DocumentSetsUniqueForB);
            File.WriteAllLines(docSetUC_p, DocumentSetsInCommonByName);

            File.WriteAllLines(docSetA_p, analyticA.domains);
            File.WriteAllLines(docSetB_p, analyticB.domains);
            #endregion

            analyticA.GetDataTable(datasetA_name + "_contentAnalysis").GetReportAndSave(folder, new imbSCI.Core.data.aceAuthorNotation());
            analyticB.GetDataTable(datasetB_name + "_contentAnalysis").GetReportAndSave(folder, new imbSCI.Core.data.aceAuthorNotation());

            //  HtmlTagCategoryTree dataSetASummary = new HtmlTagCategoryTree(datasetA_name, "HTML Tags statistics");
            foreach (var pair in analyticA.categoryNameVsHtmlTag)
            {
                pair.Value.GetDataTable(datasetA_name + "_" + pair.Key + "_htmlTag", pair.Value.description).GetReportAndSave(folder, new imbSCI.Core.data.aceAuthorNotation());
                //       dataSetASummary.Merge(pair.Value);
            }


            analyticA.GlobalCategoryTree.GetDataTable(datasetA_name + "_htmlTag_all", "Aggregate HTML tags statistics").GetReportAndSave(folder, new imbSCI.Core.data.aceAuthorNotation());

            //  HtmlTagCategoryTree dataSetBSummary = new HtmlTagCategoryTree(datasetB_name, "HTML Tags statistics");
            foreach (var pair in analyticB.categoryNameVsHtmlTag)
            {
                pair.Value.GetDataTable(datasetB_name + "_" + pair.Key + "_htmlTag", pair.Value.description).GetReportAndSave(folder, new imbSCI.Core.data.aceAuthorNotation());
                //  dataSetBSummary.Merge(pair.Value);
            }
            //   dataSetBSummary.GetDataTable(datasetB_name + "_htmlTag_all", "Aggregate HTML tags statistics").GetReportAndSave(folder, new imbSCI.Core.data.aceAuthorNotation());

            analyticB.GlobalCategoryTree.GetDataTable(datasetB_name + "_htmlTag_all", "Aggregate HTML tags statistics").GetReportAndSave(folder, new imbSCI.Core.data.aceAuthorNotation());

            GetPCE().getDataTable().Save(folder, new imbSCI.Core.data.aceAuthorNotation(), "ComparisonMetrics");
            folder.generateReadmeFiles(new imbSCI.Core.data.aceAuthorNotation());
        }

        public DataSetComparison()
        {

        }

        public PropertyCollectionExtended GetPCE()
        {
            PropertyCollectionExtended output = new PropertyCollectionExtended();

            output.Add(nameof(DocumentSetsUniqueForA), DocumentSetsUniqueForA.Count, "Unique for A", "Number of document sets that are unique for dataset A");
            output.Add(nameof(DocumentSetsUniqueForB), DocumentSetsUniqueForB.Count, "Unique for B", "Number of document sets that are unique for dataset B");
            output.Add(nameof(DocumentSetsInCommonByName), DocumentSetsInCommonByName.Count, "In common", "Number of document sets that in common for datasets");

            output.Add(nameof(TermsUniqueForA), TermsUniqueForA.Count, "Terms unique for A", "Number of terms that are unique for dataset A");
            output.Add(nameof(TermsUniqueForB), TermsUniqueForB.Count, "Terms unique for B", "Number of terms that are unique for dataset B");
            output.Add(nameof(TermsInCommon), TermsInCommon.Count, "In common", "Number of terms that in common for datasets");

            return output;
        }
    }

}