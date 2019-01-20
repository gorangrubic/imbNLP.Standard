using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Documents.FeatureAnalytics;
using imbNLP.Toolkit.Documents.Ranking;
using imbNLP.Toolkit.Planes;

namespace imbNLP.Project.Operations.Setups
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Core.ProcedureSetupBase" />
    public class SetupDocumentSelection : ProcedureSetupBase
    {


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

        private EntityPlaneMethodSettings _renderForEvaluation = new EntityPlaneMethodSettings();
        private DocumentRankingMethod _ranking = new DocumentRankingMethod();

        public SetupDocumentSelection()
        {

        }

        private CorpusPlaneMethodSettings _corpusForEvaluation = new CorpusPlaneMethodSettings();

        public CorpusPlaneMethodSettings corpusForEvaluation
        {
            get { return _corpusForEvaluation; }
            set
            {
                _corpusForEvaluation = value;
                OnPropertyChange(nameof(corpusForEvaluation));
            }
        }


        public EntityPlaneMethodSettings renderForEvaluation
        {
            get { return _renderForEvaluation; }
            set
            {
                _renderForEvaluation = value;
                OnPropertyChange(nameof(renderForEvaluation));
            }
        }

        public DocumentRankingMethod ranking
        {
            get { return _ranking; }
            set
            {
                _ranking = value;
                OnPropertyChange(nameof(ranking));
            }
        }

        public bool DoAdvancedFeatureAnalysis { get; set; } = true;
    }
}
