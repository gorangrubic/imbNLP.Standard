using imbNLP.Toolkit.Space;
using imbSCI.Core.files.folders;
using imbSCI.DataComplex.converters;
using imbSCI.DataComplex.tables;
using System;
using System.Linq;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics.Data
{
    public class FeatureCWPFrequencyDictionary : FeatureCWPDictionary<FeatureCWPFrequencies>
    {
        // public String name { get; set; } = "Freq";

        // public Dictionary<String, FeatureCWPFrequencies> terms { get; set; } = new Dictionary<string, FeatureCWPFrequencies>();

        public FeatureCWPFrequencyDictionary()
        {
        }

        /// <summary>
        /// Publishes the table blocks.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="blockSize">Size of the block.</param>
        /// <param name="blockCount">The block count.</param>
        /// <returns></returns>
        public DataTableTypeExtended<FeatureCWPFrequencies> PublishTableBlocks(folderNode folder, Int32 blockSize = 500, Int32 blockCount = 10)
        {
            if (!this.Any())
            {
                return null;
            }

            foreach (var pair in this)
            {
                pair.Value.Compute();
            }

            Int32 b = 0;

            for (int i = 0; i < blockCount; i++)
            {
                var p = this.First();

                String dt_n = name + "_" + i.ToString();

                DataTableTypeExtended<FeatureCWPFrequencies> cwpMetrics = new DataTableTypeExtended<FeatureCWPFrequencies>(dt_n, "Collected metrics");
                // DataTableTypeExtended<FeatureCWPFrequencies> cwpFrequencies = new DataTableTypeExtended<FeatureCWPFrequencies>(dt_n + "_freq", "frequency metrics");

                // p.Value.SetDataTable(datatable);
                //     DataColumn nm = datatable.Columns.Add("Name");

                Int32 c = 0;

                foreach (var pair in this)
                {
                    if (c > (i * blockSize) && c < (i + 1) * blockSize)
                    {
                        cwpMetrics.AddRow(pair.Value);

                        // cwpFrequencies.AddRow(term_finders.)
                    }

                    c++;
                }

                if (cwpMetrics.Rows.Count > 0)
                {
                    DataTableConverterASCII dataTableConverterASCII = new DataTableConverterASCII();

                    dataTableConverterASCII.ConvertToFile(cwpMetrics, folder, dt_n);

                    DataTableForStatistics report = cwpMetrics.GetReportAndSave(folder, null, dt_n);
                }
            }

            DataTableTypeExtended<FeatureCWPFrequencies> output = new DataTableTypeExtended<FeatureCWPFrequencies>();

            foreach (var p in this)
            {
                output.AddRow(p.Value);
            }

            output.Save(folder, null, "stats_frequencies");

            return output;
        }

        public DataTableTypeExtended<FeatureCWPFrequencies> GetDataTable()
        {
            DataTableTypeExtended<FeatureCWPFrequencies> output = new DataTableTypeExtended<FeatureCWPFrequencies>(name);

            foreach (var p in this)
            {
                output.AddRow(p.Value);
            }

            return output;
        }

        public void Deploy(SpaceDocumentStatsModel stats)
        {
            name = stats.name;

            var tokens = stats.terms.GetTokens();

            foreach (String tkn in tokens)
            {
                if (!this.ContainsKey(tkn))
                {
                    var freq = stats.QueryForTermFrequencies(tkn);

                    this.Add(tkn, freq);
                }
            }
        }

        public override void DisposeExtraInfo()
        {
        }
    }
}