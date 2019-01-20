using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Documents.Ranking;
using imbNLP.Toolkit.Planes;
using System;

namespace imbNLP.Project.Operations.Setups
{
    public class SetupProjectionWeightTableConstruction : ProcedureSetupBase
    {

        public SetupProjectionWeightTableConstruction() { }

        private EntityPlaneMethodSettings _primaryRender = new EntityPlaneMethodSettings();

        private SetupDocumentSelection _secondaryModel = new SetupDocumentSelection();

        private EntityPlaneMethodSettings _secondaryRender = new EntityPlaneMethodSettings();

        public EntityPlaneMethodSettings primaryRender
        {
            get { return _primaryRender; }
            set
            {
                _primaryRender = value;
                OnPropertyChange(nameof(primaryRender));
            }
        }

        public EntityPlaneMethodSettings secondaryRender
        {
            get { return _secondaryRender; }
            set { _secondaryRender = value; }
        }

        public SetupDocumentSelection secondaryModel
        {
            get { return _secondaryModel; }
            set
            {
                _secondaryModel = value;
                OnPropertyChange(nameof(secondaryModel));
            }
        }


        //  private EntityPlaneMethodSettings _secondaryRender = new EntityPlaneMethodSettings();
        //  private CorpusPlaneMethodSettings _secondaryCorpus = new CorpusPlaneMethodSettings();
        //private DocumentRankingMethod _ranking = new DocumentRankingMethod();

        //public EntityPlaneMethodSettings secondaryRender
        //{
        //    get { return _secondaryRender; }
        //    set { _secondaryRender = value;
        //        OnPropertyChange(nameof(secondaryRender));
        //    }
        //}

        /*
        public CorpusPlaneMethodSettings secondaryCorpus
        {
            get { return _secondaryCorpus; }
            set {
                _secondaryCorpus = value;
                OnPropertyChange(nameof(secondaryCorpus));
            }
        }*/

        //public DocumentRankingMethod ranking
        //{
        //    get { return _ranking; }
        //    set
        //    {
        //        _ranking = value;
        //        OnPropertyChange(nameof(ranking));
        //    }
        //}



    }
}