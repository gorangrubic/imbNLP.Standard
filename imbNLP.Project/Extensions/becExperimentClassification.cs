using imbNLP.Toolkit.ExperimentModel.CrossValidation;
using imbSCI.Core.math.classificationMetrics;

namespace imbNLP.Project.Extensions
{
    public class becExperimentClassification
    {
        public becExperimentClassification()
        {
        }

        public classificationMetricComputation averagingMethod { get; set; } = classificationMetricComputation.macroAveraging;
    }
}