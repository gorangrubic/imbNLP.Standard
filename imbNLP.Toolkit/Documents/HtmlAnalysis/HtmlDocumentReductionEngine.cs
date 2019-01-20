using HtmlAgilityPack;
using imbSCI.Core.extensions.table;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Documents.HtmlAnalysis
{
    /// <summary>
    /// Performs HTML document reduction, dataset page count and contectivity reduction
    /// </summary>
    public class HtmlDocumentReductionEngine : IReductionEngine<HtmlDocumentReductionSettings>
    {
        public HtmlDocumentReductionEngine()
        {

        }

        //public becDataSetSettings InputDataSet { get; set; } = new becDataSetSettings();






        /// <summary>
        /// Reduces the dataset.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public Double ReduceDataset(IEnumerable<WebSiteDocumentsSet> dataSet, HtmlDocumentReductionSettings settings, ILogBuilder logger)
        {

            List<Double> reductions = new List<double>();

            foreach (WebSiteDocumentsSet category in dataSet)
            {
                if (category.Count > 0)
                {
                    reductions.Add(ReduceDatasetCategory(category, settings, logger));
                }
            }

            Double average = reductions.Average();

            logger.log("Dataset reduced to (avg): " + average.ToString("P2"));

            return average;

        }



        /// <summary>
        /// Reduces the dataset.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public Double ReduceDatasetCategory(WebSiteDocumentsSet dataSet, HtmlDocumentReductionSettings settings, ILogBuilder logger)
        {
            if (dataSet.Count == 0)
            {
                return 1;
            }
            List<Double> reductions = new List<double>();

            foreach (WebSiteDocuments site in dataSet)
            {
                reductions.Add(ReduceDocumentSet(site, settings, logger));
            }

            Double average = reductions.Average();

            if (settings.logCategoryLevel) logger.log("_ [" + dataSet.name + "] _ reduced to (avg): " + average.ToString("P2"));

            return average;

        }


        /// <summary>
        /// Reduces the document set.
        /// </summary>
        /// <param name="docSet">The document set - web site.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>Rate of reduction</returns>
        public Double ReduceDocumentSet(WebSiteDocuments docSet, HtmlDocumentReductionSettings settings, ILogBuilder logger)
        {

            Int32 input = 0;
            Int32 output = 0;

            foreach (WebSiteDocument document in docSet.documents)
            {

                input += document.HTMLSource.Length;

                String newHtml = ReduceDocument(document.HTMLSource, settings, logger);

                output += newHtml.Length;


                document.HTMLSource = newHtml;

            }

            Double reduction = output.GetRatio(input);

            if (settings.logSiteLevel) logger.AppendLine("[" + docSet.domain + "] reduced to: " + reduction.ToString("P2"));

            return reduction;

        }


        /// <summary>
        /// Reduces the document.
        /// </summary>
        /// <param name="htmlInput">The HTML input.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public String ReduceDocument(String htmlInput, HtmlDocumentReductionSettings settings, ILogBuilder logger)
        {

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlInput);

            List<HtmlNode> htmlNodes = htmlDocument.DocumentNode.ChildNodes.ToList();

            List<HtmlNode> toRemove = new List<HtmlNode>();


            while (htmlNodes.Any())
            {

                List<HtmlNode> nextIteration = new List<HtmlNode>();

                foreach (HtmlNode node in htmlNodes)
                {
                    if (settings.tagsToRemove.Contains(node.Name))
                    {
                        node.Remove();
                    }
                    else
                    {
                        nextIteration.Add(node);

                        foreach (var attribute in node.Attributes.ToList())
                        {
                            if (settings.attributesToRemove.Contains(attribute.Name))
                            {
                                node.Attributes.Remove(attribute);
                            }
                        }
                    }
                }

                htmlNodes = new List<HtmlNode>();
                foreach (HtmlNode node in nextIteration)
                {
                    htmlNodes.AddRange(node.ChildNodes.ToList());
                }
            }

            return htmlDocument.DocumentNode.OuterHtml;
        }
    }
}