using imbNLP.Toolkit.Documents.WebExtensions;
using imbSCI.Core.files;
using imbSCI.Core.files.folders;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace imbNLP.Toolkit.Documents.HtmlAnalysis
{
    /// <summary>
    /// Dataset size reduction engine
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Documents.HtmlAnalysis.IReductionEngine{imbNLP.Toolkit.Documents.HtmlAnalysis.WebSiteDataSetReductionSettings}" />
    public class WebSiteDataSetReductionEngine : IReductionEngine<WebSiteDataSetReductionSettings>
    {

        protected HtmlDocumentReductionEngine htmlEngine { get; set; } = new HtmlDocumentReductionEngine();



        public long logOutputStart { get; set; } = 0;

        public Double reductionScore { get; set; }


        public void SaveReport(ILogBuilder logger, String folderPath, WebSiteDataSetReductionSettings settings)
        {
            String logOutput = logger.GetContent(logOutputStart);

            folderNode folder = folderNode.GetFolderNodeForPath(folderPath);

            String p_log = folder.pathFor("reduction_log.txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "Log of dataset reduction process");
            File.WriteAllText(p_log, logOutput);


            String p_settings = folder.pathFor("reduction_setup.xml", imbSCI.Data.enums.getWritableFileMode.overwrite, "Settings employed for reduction process");

            objectSerialization.saveObjectToXML(settings, p_settings);
        }


        /// <summary>
        /// Reduces the dataset, returns total reduction score (%)
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public double ReduceDataset(IEnumerable<WebSiteDocumentsSet> dataSet, WebSiteDataSetReductionSettings settings, ILogBuilder logger)
        {
            logOutputStart = logger.Length;

            if (!dataSet.Any())
            {
                throw new ArgumentException("The specified dataset is empty!", nameof(dataSet));
            }

            List<Double> reductions = new List<double>();

            List<Double> html_reductions = new List<double>();

            Int32 total_input = 0; // dataSet.CountDocumentsTotal();
            Int32 total_output = 0;

            foreach (WebSiteDocumentsSet category in dataSet)
            {
                total_input += category.CountDocumentsTotal();
                reductions.Add(ReduceDatasetCategory(category, settings, logger));

                html_reductions.Add(htmlEngine.ReduceDatasetCategory(category, settings.HtmlDocumentReduction, logger));

                total_output += category.CountDocumentsTotal();
                // reductions.Add(ReduceDatasetCategory(category, settings, logger));
            }

            Double average = reductions.Average();
            Double reduction = total_output.GetRatio(total_input);
            Double average_html = html_reductions.Average();

            reductionScore = (average_html * reduction);

            logger.log("Dataset document count reduced: " + reduction.ToString("P2"));
            logger.log("Dataset document size reduced (avg): " + average_html.ToString("P2"));

            logger.log("Total reduction score: " + reductionScore.ToString("P2"));

            return reductionScore;
        }

        /// <summary>
        /// Reduces the dataset category.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public double ReduceDatasetCategory(WebSiteDocumentsSet dataSet, WebSiteDataSetReductionSettings settings, ILogBuilder logger)
        {
            //List<Double> reductions = new List<double>();

            Int32 total_input = dataSet.CountDocumentsTotal();

            List<WebSiteGraphDiagnosticMark> marks = new List<WebSiteGraphDiagnosticMark>();

            if (settings.marksToRemove != WebSiteGraphDiagnosticMark.none)
            {
                marks = settings.marksToRemove.getEnumListFromFlags<WebSiteGraphDiagnosticMark>();
            }

            List<WebSiteDocuments> toRemove = new List<WebSiteDocuments>();

            foreach (WebSiteDocuments site in dataSet)
            {
                if (settings.marksToRemove != WebSiteGraphDiagnosticMark.none)
                {
                    if (site.extensions.graph == null)
                    {
                        if (settings.logSiteLevel)
                        {
                            logger.log("Site _ [" + site.domain + "] _ flaged for removal because not having graph declared");
                        }
                    }
                    else
                    {
                        foreach (WebSiteGraphDiagnosticMark mark in marks)
                        {
                            if (site.extensions.graph.diagnosticResults.HasFlag(mark))
                            {
                                if (settings.logSiteLevel)
                                {
                                    logger.log("Site _ [" + site.domain + "] _ flaged for removal because of [" + mark.ToString() + "] web graph diagnostic mark");
                                }

                                toRemove.Add(site);
                            }
                        }
                    }
                }
            }

            foreach (WebSiteDocuments site in toRemove)
            {
                if (dataSet.Contains(site))
                {

                    dataSet.Remove(site);
                }
            }

            dataSet.RemoveEmptyDocuments(logger, settings.LimitSettings.minPageLimit, settings.LimitSettings.maxPageLimit);

            Int32 total_output = dataSet.CountDocumentsTotal();

            Double average = total_output.GetRatio(total_input);

            if (settings.logCategoryLevel) logger.log("Document count in _ [" + dataSet.name + "] _ reduced to: " + average.ToString("P2"));

            return average;
        }


    }
}