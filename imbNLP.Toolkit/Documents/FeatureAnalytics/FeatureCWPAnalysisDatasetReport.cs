using imbNLP.Toolkit.Documents.FeatureAnalytics.Core;
using imbNLP.Toolkit.Documents.FeatureAnalytics.Data;
using imbSCI.Core.files.folders;
using imbSCI.Core.math.range.finder;
using imbSCI.Core.reporting;
using imbSCI.Data.collection.nested;
using imbSCI.DataComplex.converters;
using imbSCI.DataComplex.tables;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics
{
    public class FeatureCWPAnalysisDatasetReport : FeatureCWPDictionary<FeatureCWPMetrics>
    {
        public List<FeatureCWPMetrics> terms = new List<FeatureCWPMetrics>();
        public List<rangeFinderCollectionForMetrics<FeatureCWPAnalysisSiteMetrics>> term_finders = new List<rangeFinderCollectionForMetrics<FeatureCWPAnalysisSiteMetrics>>();

        public aceDictionarySet<FeatureCWPTermClass, FeatureCWPMetrics> featuresByClass { get; protected set; } = new aceDictionarySet<FeatureCWPTermClass, FeatureCWPMetrics>();

        public folderNode folder { get; set; }

        public FeatureCWPAnalysisDatasetReport(String __name, String description, folderNode _folder)
        {
            name = __name;
            //EntryDictionary = new WeightDictionary("DictionaryFor" + CategoryID, "Dictionary with term metrics for category " + CategoryID);

            folder = _folder;
        }

        public FeatureCWPAnalysisDatasetReport(String __name, String __description, folderNode _folder, List<FeatureCWPAnalysisEntryReport> categoryReports)
        {
            name = __name;
            description = __description;

            folder = _folder;

            Dictionary<string, List<FeatureCWPAnalysisSiteMetrics>> alligned = new Dictionary<string, List<FeatureCWPAnalysisSiteMetrics>>();

            List<String> keys = new List<string>();

            //categoryReports.Values.Select(x => x.terms.Values); //.GetAllignedByName(x=>x.)

            foreach (var cr in categoryReports)
            {
                foreach (FeatureCWPAnalysisSiteMetrics m in cr.Values)
                {
                    if (!alligned.ContainsKey(m.term))
                    {
                        alligned.Add(m.term, new List<FeatureCWPAnalysisSiteMetrics>());
                    }

                    alligned[m.term].Add(m);
                }
            }

            keys = alligned.Keys.ToList();
            keys.Sort();

            rangeFinderCollectionForMetrics<FeatureCWPAnalysisSiteMetrics> finder = new rangeFinderCollectionForMetrics<FeatureCWPAnalysisSiteMetrics>();

            foreach (String key in keys)
            {
                var term_finder = new rangeFinderCollectionForMetrics<FeatureCWPAnalysisSiteMetrics>();
                term_finder.name = key;
                term_finder.Learn(alligned[key]);

                term_finders.Add(term_finder);

                FeatureCWPMetrics fCWP = new FeatureCWPMetrics();
                fCWP.Deploy(term_finder);
                fCWP.term = key;

                terms.Add(fCWP);

                Add(fCWP.term, fCWP);
            }
        }

        public static void SaveFeatures(folderNode _folder, aceDictionarySet<FeatureCWPTermClass, FeatureCWPMetrics> dict)
        {
            if (_folder != null)
            {
                foreach (FeatureCWPTermClass key in dict.Keys)
                {
                    String p = _folder.pathFor("features_" + key.ToString() + ".txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of features extracted as [" + key + "]");
                    StringBuilder sb = new StringBuilder();

                    if (dict[key].Count > 0)
                    {
                        foreach (FeatureCWPMetrics m in dict[key])
                        {
                            m.ToString(sb);
                        }

                        File.WriteAllText(p, sb.ToString());
                    }
                }
            }
        }

        public void Save(ILogBuilder log, Boolean SubLevelCall = false)
        {
            //if (folder == null) folder = notes.folder_corpus;

            if (folder == null) return;

            SaveFeatures(folder.Add("Features", "Features", "Features"), featuresByClass);

            term_finders.BuildDataTableSplits(10, "FeatureStats_" + name, "Full range statistics").GetReportAndSave(folder, null, "rangeFinders", new DataTableConverterASCII());

            terms.BuildDataTableSplits(10, "FeatureTerms_" + name, "Full range statistics").GetReportAndSave(folder, null, "Metrics", new DataTableConverterASCII());
        }

        public override void DisposeExtraInfo()
        {
            terms.Clear();
            term_finders.Clear();
        }
    }
}