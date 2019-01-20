using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Planes;
using System;

namespace imbNLP.Project.Operations.Setups
{


    public class SetupWeightTableConstruction : ProcedureSetupBase
    {
        private EntityPlaneMethodSettings _entityMethod = new EntityPlaneMethodSettings();
        private CorpusPlaneMethodSettings _corpusMethod = new CorpusPlaneMethodSettings();

        public SetupWeightTableConstruction()
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

    }
}