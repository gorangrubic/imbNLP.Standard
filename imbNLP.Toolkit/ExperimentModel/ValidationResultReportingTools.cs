using imbNLP.Toolkit.Classifiers.Core;
using imbNLP.Toolkit.ExperimentModel.CrossValidation;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Weighting;
using imbSCI.Core.math;
using imbSCI.Core.math.classificationMetrics;
using imbSCI.Core.reporting;
using System;

namespace imbNLP.Toolkit.ExperimentModel
{

    public static class ValidationResultReportingTools
    {



        public static void SetReportDataFields(this classificationReport report, WeightDictionary selected)
        {
            // report.data.Add(nameof(ReportDataFieldEnum.PagePerSite), classifier.GetSignature(), "Signature of the classification algorithm");
            report.data.Add(nameof(ReportDataFieldEnum.SelectedFeatures), selected.Count.ToString(), "Number of selected features");
            //  report.data.Add(nameof(ReportDataFieldEnum.FeatureWeighting), featureWeight.GetSignature(), "Signature of feature weight model");

        }


        public static void SetReportDataFields(this classificationReport report, IClassifier classifier, FeatureFilter filter, FeatureWeightModel featureWeight)
        {
            report.data.Add(nameof(ReportDataFieldEnum.Classifier), classifier.GetSignature(), "Signature of the classification algorithm");
            report.data.Add(nameof(ReportDataFieldEnum.FeatureSelection), filter.GetSignature(), "Signature of feature selection filter model");
            report.data.Add(nameof(ReportDataFieldEnum.FeatureWeighting), featureWeight.GetSignature(), "Signature of feature weight model");

        }

        public static void SetReportDataFields(this classificationReport report, CrossValidationModel crossValidationModel, ExperimentModelExecutionContext mainContext)
        {
            report.data.Add(nameof(ReportDataFieldEnum.DataSetName), mainContext.dataset.name, "Name of dataset used in the experiment");
            report.data.Add(nameof(ReportDataFieldEnum.ValidationK), crossValidationModel.GetShortSignature(), "Cross validation model signature");

            double testPerFold = 0;
            double trainingPerFold = 0;
            Int32 c = 0;
            foreach (var frep in mainContext.testSummaries)
            {
                testPerFold += frep.Targets;
                c++;
            }

            trainingPerFold = testPerFold;
            testPerFold = testPerFold.GetRatio(c);
            trainingPerFold = trainingPerFold - testPerFold;

            report.data.Add(nameof(ReportDataFieldEnum.TestSetCount), testPerFold.ToString("F2"), "Average number of test instances per fold");
            report.data.Add(nameof(ReportDataFieldEnum.TrainingSetCount), trainingPerFold.ToString("F2"), "Average number of training instances per fold");
        }


        /// <summary>
        /// Reports to log.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="logger">The logger.</param>
        public static void ReportToLog(this IClassificationReport report, ILogBuilder logger)
        {

            logger.AppendHeading(report.Name);
            logger.AppendPair("Classifier", report.Classifier);
            logger.AppendPair("F1-Measure", report.F1measure.ToString("F5"), true, "\t\t");

            logger.AppendPair("Precission", report.Precision.ToString("F5"), true, "\t\t");
            logger.AppendPair("Recall", report.Recall.ToString("F5"), true, "\t\t");

            logger.AppendPair("Total tests", report.Targets, true, "\t\t");
            logger.AppendPair("Correct", report.Correct, true, "\t\t");

        }

    }

}