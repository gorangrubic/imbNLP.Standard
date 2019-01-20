using imbACE.Core.operations;
using imbACE.Services.consolePlugins;
using imbNLP.Project.Operations.Setups;
using imbSCI.Core.files.folders;

namespace imbNLP.Project.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbACE.Services.consolePlugins.instanceLoadSaveExtension{imbNLP.Project.Operations.Setups.SetupWeightTableConstruction}" />
    public class becGlobalWeightTableConstructionExtension : instanceLoadSaveExtension<SetupWeightTableConstruction>
    {
        private becWeightingModelExtension _weighting;

        public becWeightingModelExtension weighting
        {
            get
            {
                return _weighting;
            }

            set
            {
                _weighting = value;
                OnPropertyChanged(nameof(weighting));
            }
        }

        private becDocumentRenderingExtension _rendering;

        public becDocumentRenderingExtension rendering
        {
            get { return _rendering; }
            set
            {
                _rendering = value;
                OnPropertyChanged(nameof(rendering));
            }
        }

        public becGlobalWeightTableConstructionExtension(folderNode __folder, IAceOperationSetExecutor __parent) : base(__folder, __parent)
        {
            _weighting = new becWeightingModelExtension(__folder, this);


            _rendering = new becDocumentRenderingExtension(this, __folder);
            SetSubBinding();
        }

        public override void SetSubBinding()
        {
            _weighting.SetBinding(data, nameof(data.corpusMethod), true);
            _rendering.SetBinding(data, nameof(data.entityMethod), true);
        }
    }
}