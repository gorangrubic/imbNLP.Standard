using imbNLP.Toolkit.Documents.FeatureAnalytics.Core;
using imbNLP.Toolkit.Documents.FeatureAnalytics.Data;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Reporting;
using imbSCI.Core.files.folders;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using imbSCI.Data.collection.nested;
using imbSCI.DataComplex.extensions.data.modify;
using imbSCI.DataComplex.tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics
{
    public class FeatureCWPAnalysisEntryReport : FeatureCWPDictionary<FeatureCWPAnalysisSiteMetrics>
    {
        //  public Dictionary<String, FeatureCWPAnalysisSiteMetrics> terms = new Dictionary<string, FeatureCWPAnalysisSiteMetrics>();

        public WeightDictionary EntryDictionary { get; set; }

        public aceDictionarySet<FeatureCWPTermClass, String> featuresByClass { get; protected set; } = new aceDictionarySet<FeatureCWPTermClass, string>();

        public FeatureCWPAnalysisEntryReport(String __name, String description, folderNode _folder)
        {
            name = __name;
            //EntryDictionary = new WeightDictionary("DictionaryFor" + CategoryID, "Dictionary with term metrics for category " + CategoryID);
            EntryDictionary = new WeightDictionary("DictionaryFor" + name, description);
            EntryDictionary.nDimensions = fields().Count;
            folder = _folder;
        }

        private List<String> _fields = new List<string>();

        public List<String> fields()
        {
            if (!_fields.Any())
            {
                FeatureCWPAnalysisSiteMetrics m = new FeatureCWPAnalysisSiteMetrics();

                _fields.AddRange(m.GetFields(true, true));
            }

            return _fields;
        }

        /// <summary>
        /// Gets the metric value.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Property not recognized - property</exception>
        public Double GetMetricValue(String term, String property)
        {
            Int32 dim = fields().IndexOf(property);

            if (dim == -1)
            {
                throw new ArgumentOutOfRangeException("Property not recognized", nameof(property));
            }

            if (EntryDictionary.ContainsKey(term))
            {
                return EntryDictionary.GetValue(term, dim);
            }
            else
            {
                return 0;
            }
        }

        public void PostMerge()
        {
            foreach (var m in this)
            {
                m.Value.varianceCoeficient = m.Value.varianceCoeficient.GetRatio(m.Value.SourceCount);
                m.Value.score = m.Value.score.GetRatio(m.Value.SourceCount);
                m.Value.mean_score = m.Value.mean_score.GetRatio(m.Value.SourceCount);
                Append(m.Value, false);
            }
        }

        public void AddMerge(FeatureCWPAnalysisSiteMetrics metrics, Boolean SubLevelCall = false)
        {
            FeatureCWPAnalysisSiteMetrics existing = new FeatureCWPAnalysisSiteMetrics(metrics.term);
            var terms = this;

            if (terms.ContainsKey(metrics.term))
            {
                existing = terms[metrics.term];
            }

            existing.score += metrics.score;
            existing.mean_score += metrics.mean_score;

            existing.varianceCoeficient += metrics.varianceCoeficient;

            existing.particularity_score = Math.Max(existing.particularity_score, metrics.particularity_score);

            existing.commonality_score = Math.Max(existing.commonality_score, metrics.commonality_score);

            existing.max_score = Math.Max(existing.max_score, metrics.max_score);

            existing.min_score = Math.Min(existing.min_score, metrics.min_score);

            existing.Count += metrics.Count;

            existing.SourceCount++; ///= metrics.SourceCount;

            if (!terms.ContainsKey(existing.term))
            {
                terms.Add(existing.term, existing);
            }
            else
            {
                terms[existing.term] = existing;
            }
        }

        public void Append(FeatureCWPAnalysisSiteMetrics metrics, Boolean SubLevelCall)
        {
            AddEntry(metrics);

            featuresByClass.Add(metrics.featureClass, metrics.term);

            List<Double> scores = metrics.GetVectors(true, true);

            EntryDictionary.AddEntry(metrics.term, scores.ToArray(), false);

        }

        public static void SaveFeatures(folderNode _folder, aceDictionarySet<FeatureCWPTermClass, String> dict)
        {
            if (_folder != null)
            {
                foreach (FeatureCWPTermClass key in dict.Keys)
                {
                    String p = _folder.pathFor("features_" + key.ToString() + ".txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of features extracted as [" + key + "]");
                    StringBuilder sb = new StringBuilder();
                    if (dict[key].Count > 0)
                    {
                        foreach (String feature in dict[key])
                        {
                            sb.Append(feature + " ");
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

            EntryDictionary.Save(folder, log, name);

            if (!SubLevelCall)
            {
                var flds = fields();

                DataSet dataSet = new DataSet("rep_" + name);

                // List<DataTableForStatistics> rdt_list = new List<DataTableForStatistics>();
                foreach (String n in flds)
                {
                    DataTable dt = EntryDictionary.MakeTable(n + "_" + name, EntryDictionary.description,
                        flds
                        , 500, flds.IndexOf(n));

                    dt.AddStringLine("Report for " + name);

                    dataSet.Tables.Add(dt);
                    //DataTableForStatistics rdt = dt.GetReportTableVersion();
                    //rdt_list.Add(rdt);
                }

                DataSetForStatistics report = dataSet.GetReportAndSave(folder, null, "cwp_" + name);

                var keys = Keys.ToList();
                keys.Sort();

                DataTableTypeExtended<FeatureCWPAnalysisSiteMetrics> metrics = new DataTableTypeExtended<FeatureCWPAnalysisSiteMetrics>();
                Int32 c = 0;
                foreach (var key in keys)
                {
                    metrics.AddRow(this[key]);
                    c++;
                    if (c > 2000) break;
                }

                metrics.GetReportAndSave(folder, null, "cwp_" + name + "metrics");

                //foreach (var pair in entryReport)
                //{
                //    folderNode fn = folder.Add(pair.Key, pair.Key, "Sub entry report");
                //    pair.Value.Save(log, true);
                //}
            }
        }

        public override void DisposeExtraInfo()
        {
            //terms.Clear();
        }
    }
}