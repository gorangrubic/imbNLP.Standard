using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.ExperimentModel.Settings;
using imbNLP.Toolkit.Planes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace imbNLP.Project.Extensions.Setups
{
    public class SetupDataSetReporting : ProcedureSetupBase
    {

        public SetupDataSetReporting()
        {

        }

        public becDataSetSettings dataSetSource { get; set; } = new becDataSetSettings();

        public EntityPlaneMethodSettings render { get; set; } = new EntityPlaneMethodSettings();

        public CorpusPlaneMethodSettings corpus { get; set; } = new CorpusPlaneMethodSettings();

        public FeaturePlaneMethodSettings vector { get; set; } = new FeaturePlaneMethodSettings();

    }
}
