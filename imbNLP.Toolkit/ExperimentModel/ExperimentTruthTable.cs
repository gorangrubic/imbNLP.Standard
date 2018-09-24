using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Space;
using imbSCI.Core.math.classificationMetrics;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.ExperimentModel
{


    /// <summary>
    /// Index of document-2-class associations, used for confusion matrix, i.e. classification performances evaluation
    /// </summary>
    public class ExperimentTruthTable
    {
        public ExperimentTruthTable()
        {

        }


        /// <summary>
        /// Groups the test result entries by their real categories
        /// </summary>
        /// <param name="testResults">The test results.</param>
        /// <returns></returns>
        public Dictionary<String, List<FeatureVectorWithLabelID>> GroupByTrueCategory(List<FeatureVectorWithLabelID> testResults)
        {
            Dictionary<String, List<FeatureVectorWithLabelID>> output = new Dictionary<string, List<FeatureVectorWithLabelID>>();
            foreach (FeatureVectorWithLabelID label in testResults)
            {
                String catName = siteToLabel[label.vector.name];
                if (!output.ContainsKey(catName)) output.Add(catName, new List<FeatureVectorWithLabelID>());
                output[catName].Add(label);
            }
            return output;
        }


        /// <summary>
        /// Evaluates the test results and returns te metric set
        /// </summary>
        /// <param name="testResults">Set of test results.</param>
        /// <param name="_testName">Descriptive name to be attached at results report.</param>
        /// <param name="logger">The logger - to log any problems, if occourred.</param>
        /// <returns></returns>
        public classificationEvalMetricSet EvaluateTestResultsToMetricSet(List<FeatureVectorWithLabelID> testResults, String _testName, ILogBuilder logger)
        {
            classificationEvalMetricSet metric = new classificationEvalMetricSet(_testName, labels_without_unknown);

            foreach (FeatureVectorWithLabelID test_item in testResults)
            {
                String test_response = labels_without_unknown[test_item.labelID];

                String test_truth = siteToLabel[test_item.vector.name];

                metric.AddRecord(test_response, test_truth);
            }
            return metric;
        }

        /// <summary>
        /// Evaluates the test results.
        /// </summary>
        /// <param name="testResults">Set of test results.</param>
        /// <param name="_testName">Descriptive name to be attached at results report.</param>
        /// <param name="logger">The logger - to log any problems, if occourred.</param>
        /// <param name="averagingMethod">The averaging method.</param>
        /// <returns></returns>
        public classificationReport EvaluateTestResults(List<FeatureVectorWithLabelID> testResults, String _testName, ILogBuilder logger, classificationMetricComputation averagingMethod = classificationMetricComputation.macroAveraging)
        {
            //classificationReport report = new classificationReport()

            classificationReport report = new classificationReport(_testName);

            classificationEvalMetricSet metric = EvaluateTestResultsToMetricSet(testResults, _testName, logger);

            report.GetSetMetrics(metric);
            report.AddValues(metric, averagingMethod);

            return report;


        }





        /// <summary>
        /// Deploys the specified dataset.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <param name="logger">The logger.</param>
        public void Deploy(List<WebSiteDocumentsSet> dataset, ILogBuilder logger)
        {
            //      documentToLabelID = new Dictionary<string, int>();

            // ------------------ creation of truth table and label index ------- //
            label_index = new List<string>();
            label_index.Add(SpaceLabel.UNKNOWN);
            foreach (var fold in dataset)
            {
                label_index.Add(fold.name);
                labels_without_unknown.Add(fold.name);
            }

            // creating indexes
            index_to_label = new Dictionary<int, string>();
            for (int i = 0; i < label_index.Count; i++)
            {
                index_to_label.Add(i, label_index[i]);
            }




            // populating the truth table
            foreach (WebSiteDocumentsSet set in dataset)
            {

                Int32 c = 0;
                foreach (WebSiteDocuments entity in set)
                {
                    siteToLabel.Add(entity.domain, set.name);
                    //if (!documentToLabelID.ContainsKey(entity.domain))
                    //{
                    //    c++;
                    //    documentToLabelID.Add(entity.domain, l_id);
                    //}

                }

                //   logger.log("Truth table entries [" + c + "] for class [" + l_id + "]:[" + set.name + "] created.");
            }
        }

        #region GLOBAL INFORMATION 

        public Dictionary<String, String> siteToLabel { get; set; } = new Dictionary<string, string>();

        public List<String> labels_without_unknown { get; set; } = new List<string>();

        public Dictionary<Int32, String> index_to_label { get; set; } = new Dictionary<int, string>();

        public List<String> label_index { get; set; } = new List<String>();

        //   public Dictionary<String, Int32> documentToLabelID { get; set; } = new Dictionary<string, int>();

        #endregion
    }

}