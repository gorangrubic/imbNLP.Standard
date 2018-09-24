using imbSCI.Core.math.classificationMetrics;
using imbSCI.Core.reporting;

namespace imbNLP.Toolkit.ExperimentModel
{

    public static class ValidationResultReportingTools
    {

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