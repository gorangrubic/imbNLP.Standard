using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Documents.FeatureAnalytics;
using imbNLP.Toolkit.Documents.FeatureAnalytics.Core;
using imbNLP.Toolkit.Documents.FeatureAnalytics.Data;
using imbNLP.Toolkit.Planes;
using imbNLP.Toolkit.Weighting;
using System.Collections.Generic;

namespace imbNLP.Project.Operations.Setups
{



    public class SetupFeatureCWPAnalysis : ProcedureSetupBase
    {


        public List<FeatureWeightModel> WeightModels { get; set; } = new List<FeatureWeightModel>();

        public List<FeatureFilter> FilterModels { get; set; } = new List<FeatureFilter>();



        public CWPAnalysisReportsEnum tasks { get; set; } = CWPAnalysisReportsEnum.all;

        private CorpusPlaneMethodSettings _corpusForEvaluation = new CorpusPlaneMethodSettings();

        private FeaturePlaneMethodSettings _featureMethod = new FeaturePlaneMethodSettings();
        public FeaturePlaneMethodSettings featureMethod
        {
            get { return _featureMethod; }
            set
            {
                _featureMethod = value;
                OnPropertyChange(nameof(featureMethod));
            }
        }


        public CorpusPlaneMethodSettings corpusForEvaluation
        {
            get { return _corpusForEvaluation; }
            set
            {
                _corpusForEvaluation = value;
                OnPropertyChange(nameof(corpusForEvaluation));
            }
        }

        private EntityPlaneMethodSettings _renderForEvaluation = new EntityPlaneMethodSettings();
        private FeatureCWPAnalysisSettings _analysisSettings = new FeatureCWPAnalysisSettings();

        public EntityPlaneMethodSettings renderForEvaluation
        {
            get { return _renderForEvaluation; }
            set
            {
                _renderForEvaluation = value;
                OnPropertyChange(nameof(renderForEvaluation));
            }
        }

        public FeatureCWPAnalysisSettings analysisSettings
        {
            get { return _analysisSettings; }
            set
            {
                _analysisSettings = value;
                OnPropertyChange(nameof(analysisSettings));
            }
        }

    }
}