using imbACE.Core.operations;
using imbACE.Services.consolePlugins;
using imbNLP.Project.Operations;
using imbNLP.Project.Operations.Setups;
using imbSCI.Core.extensions.data;
using imbSCI.Core.files.folders;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace imbNLP.Project.Extensions
{
    public class becFeatureCWPAnalysisExtension : instanceLoadSaveExtension<SetupFeatureCWPAnalysis>
    {



        public override void SetSubBinding()
        {
            _rendering.SetBinding(data, nameof(data.renderForEvaluation), true);
            _features.SetBinding(data, nameof(data.featureMethod), true);
            _weight.SetBinding(data, nameof(data.corpusForEvaluation), true);

        }

        public becFeatureCWPAnalysisExtension(folderNode __folder, IAceOperationSetExecutor __parent) : base(__folder, __parent)
        {
            _rendering = new becDocumentRenderingExtension(__parent, __folder);
            _features = new becFeatureVectorExtension(__folder, __parent);
            _weight = new becWeightingModelExtension(__folder, __parent);
            SetSubBinding();
        }

        private becDocumentRenderingExtension _rendering;

        [Display(GroupName = "Setup", Name = "render", Description = "Document rendering")]
        public becDocumentRenderingExtension rendering
        {
            get
            {
                return _rendering;
            }
            set
            {
                _rendering = value;
                OnPropertyChanged(nameof(rendering));
            }
        }

        private becWeightingModelExtension _weight;

        [Display(GroupName = "Setup", Name = "weight", Description = "Term weighting")]
        public becWeightingModelExtension weight
        {
            get
            {
                return _weight;
            }
            set
            {
                _weight = value;
                OnPropertyChanged(nameof(weight));
            }
        }



        private becFeatureVectorExtension _features;


        [Display(GroupName = "Setup", Name = "fv", Description = "feature vector constructor")]
        public becFeatureVectorExtension features
        {
            get
            {
                return _features;
            }
            set
            {
                _features = value;
                OnPropertyChanged(nameof(features));
            }
        }
    }
}