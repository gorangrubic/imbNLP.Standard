using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Planes;
using imbSCI.Core.math.classificationMetrics;
using System;

namespace imbNLP.Project.Operations.Setups
{


    public class SetupDocumentClassification : ProcedureSetupBase
    {
        private EntityPlaneMethodSettings _entityMethod = new EntityPlaneMethodSettings();
        private CorpusPlaneMethodSettings _corpusMethod = new CorpusPlaneMethodSettings();
        private FeaturePlaneMethodSettings _featureMethod = new FeaturePlaneMethodSettings();

        private classificationMetricComputation _averagingMethod = classificationMetricComputation.macroAveraging;


        public Boolean ExportEvaluationAsDocumentSelectionResult { get; set; } = false;
        public Double ExportEvaluationCorrectScore { get; set; } = 1;
        public Double ExportEvaluationIncorrectScore { get; set; } = 0;




        /// <summary>
        /// Gets or sets the export evaluation to filename - if * it will be replaced with <see cref="ProcedureSetupBase.OutputFilename"/>
        /// </summary>
        /// <value>
        /// The export evaluation to filename.
        /// </value>
        public String ExportEvaluationToFilename { get; set; } = "*";

        public ClassificationDatasetSeparationEnum dataSetMode { get; set; } = ClassificationDatasetSeparationEnum.TrainingLabeled_TestUnlabeled;


        public DocumentSelectQuery documentSelectQuery { get; set; } = new DocumentSelectQuery();

        public SetupDocumentClassification()
        {

        }


        /// <summary>
        /// Gets or sets the entity method settings
        /// </summary>
        /// <value>
        /// The entity method.
        /// </value>
        public EntityPlaneMethodSettings entityMethod
        {
            get { return _entityMethod; }
            set
            {
                _entityMethod = value;
                OnPropertyChange(nameof(entityMethod));
            }
        }

        /// <summary>
        /// Gets or sets the corpus method settings
        /// </summary>
        /// <value>
        /// The corpus method.
        /// </value>
        public CorpusPlaneMethodSettings corpusMethod
        {
            get { return _corpusMethod; }
            set
            {
                _corpusMethod = value;
                OnPropertyChange(nameof(corpusMethod));
            }
        }

        /// <summary>
        /// Gets or sets the feature method.
        /// </summary>
        /// <value>
        /// The feature method.
        /// </value>
        public FeaturePlaneMethodSettings featureMethod
        {
            get { return _featureMethod; }
            set
            {
                _featureMethod = value;
                OnPropertyChange(nameof(featureMethod));
            }
        }


        /// <summary>
        /// Gets or sets the averaging method.
        /// </summary>
        /// <value>
        /// The averaging method.
        /// </value>
        public classificationMetricComputation averagingMethod
        {
            get { return _averagingMethod; }
            set
            {
                _averagingMethod = value;
                OnPropertyChange(nameof(averagingMethod));
            }
        }
    }
}