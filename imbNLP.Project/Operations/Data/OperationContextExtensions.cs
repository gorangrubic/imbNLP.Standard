using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Documents.DatasetStructure;
using imbNLP.Toolkit.ExperimentModel;
using imbSCI.Core.math.classificationMetrics;
using imbSCI.Core.math.range.finder;
using imbSCI.Data;
using System;
using System.Linq;

namespace imbNLP.Project.Operations.Data
{
    public static class OperationContextExtensions
    {
      

        public static void SetReportDataFields(this classificationReport report, DatasetStructureReport context)
        {
            report.data.Add(nameof(ReportDataFieldEnum.PagePerSite), context.pagesPerSite.ToString("F2"), "Pages per web site instance");


        }

        public static void SetReportDataFields(this classificationReport report, OperationContext context, Boolean afterFeatureSelection = false)
        {
            if (!afterFeatureSelection)
            {
                report.data.Add(nameof(ReportDataFieldEnum.labeled_terms), context.spaceModel.terms_known_label.Count.ToString(), "Number of labeled input terms");
                report.data.Add(nameof(ReportDataFieldEnum.unlabeled_terms), context.spaceModel.terms_unknown_label.Count.ToString(), "Number of unlabeled input terms");
            }
            else
            {


                report.data.Add(nameof(ReportDataFieldEnum.labeled_selected_terms), context.spaceModel.terms_known_label.Count.ToString(), "Number of labeled selected terms");
                report.data.Add(nameof(ReportDataFieldEnum.unlabeled_selected_terms), context.spaceModel.terms_unknown_label.Count.ToString(), "Number of unlabeled selected terms");

                report.data.Add(nameof(ReportDataFieldEnum.SelectedFeatures), context.SelectedFeatures.Count.ToString(), "Number of selected features");

                rangeFinder ranger = new rangeFinder();

                foreach (var pair in context.SelectedFeatures.index)
                {
                    ranger.Learn(pair.Value.weight);

                }

                report.data.Add(nameof(ReportDataFieldEnum.SelectedFeatureMin), ranger.Minimum.ToString("F5"), "Smallest weight of a selected feature");
            }

        }
    }
}